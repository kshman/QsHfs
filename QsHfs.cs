#define DEBUG_TRACE
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace QsHfs;

internal class QsHfs
{
	const uint HFS_HEADER = 0x00534648;
	const ushort HFS_VERSION = 200;

	public enum FileType
	{
		Unknown = 0,
		System = 1,
		Text = 4,
		Picture = 5,
		Sound = 6,
		Video = 7,
		Code = 8,
		Program = 9,
		Script = 10,
		Archive = 11,
		MarkUp = 12,
		MarkDown = 13,
		Json = 14,
		Toml = 15,
	}

	public enum FileAttr
	{
		None = 0,
		File = 1 << 0,
		Dir = 1 << 1,
		System = 1 << 2,
		Hidden = 1 << 3,
		ReadOnly = 1 << 4,
		Link = 1 << 5,
		Compress = 1 << 6,
		Encrypt = 1 << 7,
	}

	[StructLayout(LayoutKind.Sequential)]
	struct HfsHeader
	{
		public uint header;
		public ushort version;
		public ushort notuse;
		public long stc;
		public long stw;
		public uint revision;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] desc;

		public override string ToString()
		{
			DateTime dt = QsSupp.ToDateTime(stc);
			string sdesc = Encoding.UTF8.GetString(desc).TrimEnd('\0');
			return $"HFS {version}.{revision} ({dt}) {sdesc}";
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	struct HfsSource
	{
		public byte type;
		public byte attr;
		public ushort len;
		public uint size;
		public uint cmpr;
		public uint seek;

		override public string ToString()
		{
			if ((attr & (int)FileAttr.Dir) == (int)FileAttr.Dir)
				return $"{(FileType)type}/{(FileAttr)attr} {seek}";
			else
				return $"{(FileType)type}/{(FileAttr)attr} {seek} ({size}/{cmpr})";
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	struct HfsFile
	{
		public HfsSource source;
		public long stc;
		public uint hash;
		public uint meta;
		public uint next;

		public override string ToString()
		{
			return source.ToString();
		}
	}
	struct HfsInfo
	{
		public HfsFile file;
		public string name;
		public DateTime stc;

		public HfsInfo()
		{
			file = new HfsFile();
			name = string.Empty;
			stc = DateTime.MinValue;
		}

		public override string ToString()
		{
			if ((file.source.attr & (int)FileAttr.Dir) == (int)FileAttr.Dir)
				return $"[{name}] ({stc})";
			return $"{name} ({file.source.size}/{file.source.cmpr}, {stc})";
		}
	}

	internal static readonly char[] directory_separator = ['\\', '/', '\n', '\r', '\t'];

	private nint HFSAT_ROOT;
	private nint HFSAT_NEXT;
	private uint HFSFILE_SIZE;

	private string _name;
	private FileStream _fs;
	private uint _touch = 0;

	private string _desc = string.Empty;
	private string _path = string.Empty;
	private List<HfsInfo> _infos = new List<HfsInfo>();

	private Random _rand = new();

	public QsHfs(string path)
		: this(path, false)
	{
	}

	public QsHfs(string path, bool create)
	{
		HFSAT_ROOT = Marshal.SizeOf<HfsHeader>();
		HFSAT_NEXT = Marshal.OffsetOf<HfsFile>(nameof(HfsFile.next));
		HFSFILE_SIZE = (uint)Marshal.SizeOf<HfsFile>();

		if (create)
		{
			_fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
			WriteHeader();
		}
		else
		{
			if (File.Exists(path) == false)
				throw new FileNotFoundException();
			_fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
		}

		if (ReadHeader() == false)
			throw new InvalidDataException();

		_name = path;
		ChDir("/");
		ChDir("test");
		ChDir("..");
	}

	// 디렉토리 만들기
	public bool MkDir(string directory)
	{
		if (directory[0] == '.')
			return false;
		if (directory == "/")
			return false;
		if (directory.Length >= 2048)
			return false;

		var (dir, name) = QsSupp.DivPath(directory);
		if (name.Length >= 260)
			return false;
		if (SaveDirectory(dir, out var save) == false)
			return false;

		var hash = QsSupp.Shash(name);
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && name.Equals(info.name, StringComparison.InvariantCultureIgnoreCase))
			{
				RestoreDirectory(save);
				return false;
			}
		}

		try
		{
			if (_fs.Seek(0, SeekOrigin.End) != _fs.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			return false;
		}

		var next = (uint)_fs.Position;
		WriteDirectory(name, 0, (uint)(next + HFSFILE_SIZE + name.Length), 0);
		var curr = (uint)_fs.Position;
		WriteDirectory(".", (uint)(curr + HFSFILE_SIZE + 1), (uint)(next + HFSFILE_SIZE + name.Length), 0);
		var first = _infos.First();
		WriteDirectory("..", 0, first.file.source.seek, first.file.stc);

		var last = _infos.Last();
		_fs.Seek(HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));

		if (string.IsNullOrEmpty(save))
			save = ".";
		RestoreDirectory(save);

		_touch++;
		return true;
	}

	// 디렉토리 변경
	public bool ChDir(string directory)
	{
		if (string.IsNullOrEmpty(directory))
			return false;
		if (_path.Equals(directory, StringComparison.OrdinalIgnoreCase))
			return true;

		if (_infos.Count > 0)
		{
			HfsInfo first = _infos.First();
			_fs.Seek(first.file.source.seek, SeekOrigin.Begin);
		}

		if (directory[0] == '/')
		{
			_fs.Seek(HFSAT_ROOT, SeekOrigin.Begin);
			_path = "/";
		}

		var info = new HfsInfo();
		var dirs = directory.Split(directory_separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (var tok in dirs)
		{
			uint hash = QsSupp.Shash(tok);
			if (FindDirectory(ref info, tok, hash) == false)
				return false;
			_fs.Seek(info.file.meta, SeekOrigin.Begin);
			MakeDirectoryName(tok);
		}
#if DEBUG_TRACE
		Debug.WriteLine($"\tHFS: 현재 디렉토리 {_path}");
#endif

		_infos.Clear();
		for (uint srt = (uint)_fs.Position; srt != 0; srt = info.file.next)
		{
			if (ReadHfsFile(out info) == false)
				break;
			_fs.Seek(info.file.next, SeekOrigin.Begin);
			info.file.source.seek = srt;
			_infos.Add(info);
#if DEBUG_TRACE
			Debug.WriteLine($"\t {info.file.source.type}/{info.file.source.attr} | {info.name} ({info.file.source.seek})");
#endif
		}
#if DEBUG_TRACE
		Debug.WriteLine($"\tHFS: 디렉토리 변경 완료 (파일: {_infos.Count}개)");
#endif

		return true;
	}

	// 제거
	public bool Remove(string path)
	{
		var (dir, name) = QsSupp.DivPath(path);
		if (name.Length >= 260)
			return false;
		if (name[0] == '.')
			return false;
		if (SaveDirectory(dir, out var save) == false)
			return false;

		uint hash = QsSupp.Shash(name);
		HfsInfo? found = null;
		int i = 0;
		for (i = 0; i < _infos.Count; i++)
		{
			var info = _infos[i];
			if (hash == info.file.hash && name.Length == info.file.source.len &&
				name.Equals(info.name, StringComparison.InvariantCultureIgnoreCase))
			{
				found = info;
				break;
			}
		}
		if (found == null)
		{
			RestoreDirectory(save);
			return false;
		}

		uint next = found.Value.file.next;
		HfsInfo prev = _infos[i - 1];
		try
		{
			if (_fs.Seek(HFSAT_NEXT + prev.file.source.seek, SeekOrigin.Begin) <= 0)
				throw new Exception();
			_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			return false;
		}

		_infos.RemoveAt(i);
		RestoreDirectory(save);

		_touch++;
		return true;
	}

	// HFS 헤더 읽고 확인
	private bool ReadHeader()
	{
		byte[] buffer = new byte[Marshal.SizeOf<HfsHeader>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		HfsHeader header = Marshal.PtrToStructure<HfsHeader>(handle.AddrOfPinnedObject());
		handle.Free();

		if (header.header != HFS_HEADER || header.version != HFS_VERSION)
			return false;

		_desc = Encoding.UTF8.GetString(header.desc).TrimEnd('\0');
		return true;
	}

	// HFS 파일 읽기
	private bool ReadHfsFile(out HfsFile file)
	{
		byte[] buffer = new byte[Marshal.SizeOf<HfsFile>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			file = new HfsFile();
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		file = Marshal.PtrToStructure<HfsFile>(handle.AddrOfPinnedObject());
		handle.Free();
		return true;
	}

	// HFS 파일 읽기 (파일 이름 포함)
	private bool ReadHfsFile(out HfsInfo info)
	{
		byte[] buffer = new byte[Marshal.SizeOf<HfsFile>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			info = new HfsInfo();
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		info.file = Marshal.PtrToStructure<HfsFile>(handle.AddrOfPinnedObject());
		handle.Free();
		buffer = new byte[info.file.source.len];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			info = new HfsInfo();
			return false;
		}
		info.name = Encoding.UTF8.GetString(buffer);
		info.stc = QsSupp.ToDateTime(info.file.stc);
		return true;
	}

	// 디렉토리 찾기
	private bool FindDirectory(ref HfsInfo info, string name, uint hash)
	{
		while (ReadHfsFile(out info.file))
		{
			if (info.file.source.attr == 2 && info.file.source.len == name.Length && info.file.hash == hash)
			{
				byte[] buffer = new byte[info.file.source.len];
				try
				{
					if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
						throw new Exception();
				}
				catch (Exception)
				{
					break;
				}
				info.name = Encoding.UTF8.GetString(buffer);
				return true;
			}
			if (info.file.next == 0)
				break;
			if (_fs.Seek(info.file.next, SeekOrigin.Begin) != info.file.next)
				break;
		}
		return false;
	}

	// 디렉토리 이름 만들기
	private void MakeDirectoryName(string name)
	{
		if (name.Equals("."))
			return;
		if (name.Equals(".."))
		{
			if (_path.Equals("/"))
				return;
			int index = _path.LastIndexOf('/', _path.Length - 2);
			if (index == -1)
			{
				_path = "/";
				return;
			}
			_path = _path[..(index + 1)];
			return;
		}
		_path += name + '/';
	}

	// 디렉터리 저장
	private bool SaveDirectory(string directory, out string save)
	{
		if (string.IsNullOrWhiteSpace(directory) || string.Equals(directory, _path, StringComparison.InvariantCultureIgnoreCase))
		{
			save = string.Empty;
			return true;
		}
		save = _path;
		return ChDir(directory);
	}

	// 디렉토리 복원
	private void RestoreDirectory(string save)
	{
		if (string.IsNullOrWhiteSpace(save))
			return;
		ChDir(save);
	}

	// HFS 파일 헤더 쓰기
	private bool WriteHeader(string? desc = null)
	{
		var now = DateTime.Now;
		if (string.IsNullOrWhiteSpace(desc))
		{
			var name = Environment.UserName;
			var computer = Environment.MachineName;
			desc = $"{name}@{computer} {now}";
		}

		byte[] buffer = new byte[Marshal.SizeOf<HfsHeader>()];
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		HfsHeader header = new HfsHeader
		{
			header = HFS_HEADER,
			version = HFS_VERSION,
			notuse = 0,
			stc = QsSupp.ToStamp(now),
			stw = QsSupp.ToStamp(now),
			revision = 0,
			desc = Encoding.UTF8.GetBytes(desc)
		};
		Marshal.StructureToPtr(header, handle.AddrOfPinnedObject(), false);
		handle.Free();
		try
		{
			_fs.Write(buffer, 0, buffer.Length);
			_fs.Seek(0, SeekOrigin.Begin);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	// 디렉토리 쓰기
	private bool WriteDirectory(string name, uint next, uint meta, long stc)
	{
		if (stc == 0)
			stc = QsSupp.ToStamp(DateTime.Now);
		HfsFile file = new HfsFile
		{
			source = new HfsSource
			{
				type = (byte)FileType.System,
				attr = (byte)FileAttr.Dir,
				size = (uint)_rand.Next(),
				cmpr = (uint)_rand.Next(),
				seek = (uint)_rand.Next()
			},
			meta = meta,
			next = next,
			stc = stc,
		};
		return WriteFileHeader(file, name);
	}

	// 파일 헤더 쓰기
	private bool WriteFileHeader(HfsFile file, string name)
	{
		file.source.len = (ushort)name.Length;
		file.hash = QsSupp.Shash(name);
		byte[] buffer = new byte[Marshal.SizeOf<HfsFile>()];
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		Marshal.StructureToPtr(file, handle.AddrOfPinnedObject(), false);
		handle.Free();
		try
		{
			_fs.Write(buffer, 0, buffer.Length);
			byte[] namebuf = Encoding.UTF8.GetBytes(name);
			_fs.Write(namebuf, 0, namebuf.Length);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}
}

