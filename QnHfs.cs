#if DEBUG
#define DEBUG_TRACE
#endif
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace QsHfs;

internal class QnHfs : IDisposable
{
	const uint HFS_HEADER = 0x00534648;
	const ushort HFS_VERSION = 14;

	internal static readonly char[] directory_separator = ['\\', '/', '\n', '\r', '\t'];

	private static readonly int QN_MAX_PATH = 1024;
	private static readonly int HFS_MAX_NAME = 260;

	private static readonly nint HFSAT_ROOT;
	private static readonly nint HFSAT_ROOT_STW;
	private static readonly nint HFSAT_ROOT_REV;
	private static readonly nint HFSAT_NEXT;
	private static readonly int HFSSIZE_FILE;

	private readonly Random _rand = new();

	private readonly string _name;
	private readonly FileStream _fs;
	private Hfs.Header _header;
	private uint _touch = 0;

	private string _mesg = string.Empty;
	private string _desc = string.Empty;
	private string _path = string.Empty;
	private readonly List<Hfs.FileInfo> _infos = [];

	private bool _disposed;

	static QnHfs()
	{
		HFSAT_ROOT = Marshal.SizeOf<Hfs.Header>();
		HFSAT_ROOT_STW = Marshal.OffsetOf<Hfs.Header>(nameof(Hfs.Header.stw));
		HFSAT_ROOT_REV = Marshal.OffsetOf<Hfs.Header>(nameof(Hfs.Header.revision));
		HFSAT_NEXT = Marshal.OffsetOf<Hfs.FileData>(nameof(Hfs.FileData.next));

		HFSSIZE_FILE = Marshal.SizeOf<Hfs.FileData>();
	}

	public QnHfs(string path)
		: this(path, false)
	{
	}

	public QnHfs(string path, bool create)
	{
		if (create)
		{
			_fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
			WriteHeader();
			_fs.Seek(0, SeekOrigin.Begin);
		}
		else
		{
			if (File.Exists(path) == false)
				throw new FileNotFoundException();
			_fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
		}

		if (ReadHeader(out _header) == false)
			throw new InvalidDataException();

		_name = Path.GetFileName(path);
		ChDir("/");
	}

	// 소멸자
	public void Dispose() => Close();

	// 닫기
	public void Close()
	{
		if (_disposed)
			return;
		_disposed = true;
		CheckTouch();
		_fs.Close();
	}

	// 터치
	private void CheckTouch()
	{
		if (_touch == 0)
			return;

		var rev = _header.revision + 1;
		_fs.Seek(HFSAT_ROOT_REV, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(rev), 0, sizeof(uint));

		var now = QsSupp.ToStamp(DateTime.Now);
		_fs.Seek(HFSAT_ROOT_STW, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(now), 0, sizeof(ulong));

		_touch = 0;
	}

	// 파일 목록
	public List<Hfs.FileInfo> GetFiles()
	{
		return _infos;
	}

	// 이름
	public string Name => _name;

	// 경로
	public string CurrentDirectory => _path;

	// 메시지
	public string Message => _mesg;

	// 디렉토리 변경
	public bool ChDir(string directory)
	{
		if (string.IsNullOrEmpty(directory))
		{
			_mesg = "디렉토리가 비어있어요";
			return false;
		}
		if (_path.Equals(directory, StringComparison.OrdinalIgnoreCase))
			return true;

		if (_infos.Count > 0)
		{
			Hfs.FileInfo first = _infos.First();
			_fs.Seek(first.file.source.seek, SeekOrigin.Begin);
		}

		if (directory[0] == '/')
		{
			_fs.Seek(HFSAT_ROOT, SeekOrigin.Begin);
			_path = "/";
		}

		var info = new Hfs.FileInfo();
		var dirs = directory.Split(directory_separator, StringSplitOptions.RemoveEmptyEntries);
		foreach (var tok in dirs)
		{
			var name = new PathString(tok);
			if (FindDirectory(ref info, name) == false)
			{
				_mesg = $"<{tok}> 디렉토리가 없어요";
				return false;
			}
			_fs.Seek(info.file.subp, SeekOrigin.Begin);
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
			if (info.file.Type == Hfs.FileType.Dir)
				Debug.WriteLine($"\t\t{info.file.Attr}/{info.file.Type} | [{info.name}] ({info.file.subp})");
			else
				Debug.WriteLine($"\t\t{info.file.Attr}/{info.file.Type} | {info.name} <{info.file.source.seek}>");
#endif
		}
#if DEBUG_TRACE
		Debug.WriteLine($"\tHFS: 디렉토리 변경 완료 (파일: {_infos.Count}개)");
#endif

		return true;
	}

	// 디렉토리 만들기
	public bool MkDir(string directory)
	{
		if (directory[0] == '.')
		{
			_mesg = "'.'으로 시작하는 디렉토리는 만들 수 없어요";
			return false;
		}
		if (directory == "/")
		{
			_mesg = "루트 디렉토리는 만들 수 없어요";
			return false;
		}
		if (directory.Length >= QN_MAX_PATH - 1)
		{
			_mesg = $"전체 디렉토리 이름이 너무 길어요. 최대 {QN_MAX_PATH - 1} 글자까지만 지원해요";
			return false;
		}

		var (dir, filename) = QsSupp.DivPath(directory);
		var name = new PathString(filename);
		if (name.Length >= HFS_MAX_NAME)
		{
			_mesg = $"파일 이름이 너무 길어요. 최대 {HFS_MAX_NAME - 1} 글자까지만 지원해요 (UTF-8)";
			return false;
		}
		if (SaveDirectory(dir, out var save) == false)
		{
			_mesg = "디렉토리를 찾을 수 없어요";
			return false;
		}

		var (len, hash) = name.LenAndHash;
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && len == info.file.source.len && name.Equals(info.name))
			{
				RestoreDirectory(save);
				_mesg = "이미 있는 디렉토리에요";
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
			_mesg = "HFS 파일을 조작할 수 없어요";
			return false;
		}

		var next = (uint)_fs.Position;
		var subp = (uint)(next + HFSSIZE_FILE + len);
		var stc = QsSupp.ToStamp(DateTime.Now);
		WriteDirectory(name, stc, subp, 0);
		WriteDirectory(".", stc, subp, (uint)(subp + HFSSIZE_FILE + 1));

		var parent = _infos.First();
		WriteDirectory("..", parent.file.stc, parent.file.source.seek, 0);

		var last = _infos.Last();
		_fs.Seek(HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));

		if (string.IsNullOrEmpty(save))
			save = ".";
		RestoreDirectory(save);

		_touch++;
		return true;
	}

	// 제거
	public bool Remove(string path)
	{
		var (dir, filename) = QsSupp.DivPath(path);
		var name = new PathString(filename);
		if (name.Length >= QN_MAX_PATH - 1)
		{
			_mesg = $"전체 디렉토리 이름이 너무 길어요. 최대 {QN_MAX_PATH - 1} 글자까지만 지원해요";
			return false;
		}
		if (filename[0] == '.')
		{
			_mesg = "'.'으로 시작하는 파일은 지울 수 없어요";
			return false;
		}
		if (SaveDirectory(dir, out var save) == false)
		{
			_mesg = "디렉토리를 찾을 수 없어요";
			return false;
		}

		var (len, hash) = name.LenAndHash;
		Hfs.FileInfo? found = null;
		int i = 0;
		for (i = 0; i < _infos.Count; i++)
		{
			var info = _infos[i];
			if (hash == info.file.hash && len == info.file.source.len && name.Equals(info.name))
			{
				found = info;
				break;
			}
		}
		if (found == null)
		{
			RestoreDirectory(save);
			_mesg = "파일을 찾을 수 없어요";
			return false;
		}

		uint next = found.file.next;
		Hfs.FileInfo prev = _infos[i - 1];
		try
		{
			if (_fs.Seek(HFSAT_NEXT + prev.file.source.seek, SeekOrigin.Begin) <= 0)
				throw new Exception();
			_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			_mesg = "HFS 파일을 조작할 수 없어요";
			return false;
		}

		_infos.RemoveAt(i);
		RestoreDirectory(save);

		_touch++;
		return true;
	}

	// 파일 있나 확인
	public Hfs.FileAttr Exists(string path)
	{
		var (dir, filename) = QsSupp.DivPath(path);
		var name = new PathString(filename);
		if (name.Length >= HFS_MAX_NAME)
		{
			_mesg = $"파일 이름이 너무 길어요. 최대 {HFS_MAX_NAME - 1} 글자까지만 지원해요 (UTF-8)";
			return Hfs.FileAttr.None;
		}
		if (SaveDirectory(dir, out var save) == false)
		{
			_mesg = "디렉토리를 찾을 수 없어요";
			return Hfs.FileAttr.None;
		}

		var (len, hash) = name.LenAndHash;
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && len == info.file.source.len && name.Equals(info.name))
			{
				RestoreDirectory(save);
				return info.file.Attr;
			}
		}

		RestoreDirectory(save);
		return Hfs.FileAttr.None;
	}

	// 소스 읽기
	public byte[] SourceRead(Hfs.FileSource source)
	{
		try
		{
			_fs.Seek(source.seek + HFSSIZE_FILE + source.len, SeekOrigin.Begin);

			byte[] buffer;
			if (source.IsCompressed)
			{
				using var outstream = new MemoryStream();
				using var deflate = new DeflateStream(_fs, CompressionMode.Decompress, true);
				deflate.CopyTo(outstream);
				return outstream.ToArray();
			}
			else
			{
				buffer = new byte[source.size];
				_fs.Read(buffer, 0, buffer.Length);
			}
			return buffer;
		}
		catch (Exception)
		{
			_mesg = "HFS 파일을 읽을 수 없거나, 압축을 풀 수 없어요";
			return Array.Empty<byte>();
		}
	}

	// 파일 읽기
	public byte[] Read(string path, out int size)
	{
		if (path.Length >= QN_MAX_PATH - 1)
		{
			size = 0;
			_mesg = $"전체 파일 이름이 너무 길어요. 최대 {QN_MAX_PATH - 1} 글자까지만 지원해요";
			return Array.Empty<byte>();
		}

		var (dir, filename) = QsSupp.DivPath(path);
		if (filename[0] == '.')
		{
			size = 0;
			_mesg = "'.'으로 시작하는 파일은 읽을 수 없어요";
			return Array.Empty<byte>();
		}

		var name = new PathString(filename);
		if (name.Length >= HFS_MAX_NAME)
		{
			size = 0;
			_mesg = $"파일 이름이 너무 길어요. 최대 {HFS_MAX_NAME - 1} 글자까지만 지원해요 (UTF-8)";
			return Array.Empty<byte>();
		}
		if (SaveDirectory(dir, out var save) == false)
		{
			size = 0;
			_mesg = "디렉토리를 찾을 수 없어요";
			return Array.Empty<byte>();
		}

		var (len, hash) = name.LenAndHash;
		Hfs.FileInfo? found = null;
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && len == info.file.source.len && name.Equals(info.name))
			{
				found = info;
				break;
			}
		}
		if (found == null)
		{
			RestoreDirectory(save);
			size = 0;
			_mesg = "파일을 찾을 수 없어요";
			return Array.Empty<byte>();
		}

		byte[] buffer = SourceRead(found.file.source);
		RestoreDirectory(save);
		size = buffer.Length;
		return buffer;
	}

	// 파일 저장
	public bool StoreFile(string? filename, string path)
	{
		if (File.Exists(path) == false)
			return false;

		if (string.IsNullOrEmpty(filename))
			filename = Path.GetFileName(path);

		byte[] data = File.ReadAllBytes(path);
		return StoreBuffer(filename, data);
	}

	// 옵티마이즈
	public bool Optimize(string path, Hfs.OptimizeCallback? callback)
	{
		QnHfs output;
		try
		{
			output = new QnHfs(path, true);
		}
		catch (Exception)
		{
			return false;
		}

		Hfs.OptimizeData data = new Hfs.OptimizeData
		{
			Stack = 1,
			Input = this,
			Output = output,
			OptimizeEvent = callback,
		};

		SaveDirectory("/", out var save);
		bool ret = OptimizeProcess(ref data);
		RestoreDirectory(save);

		output.Dispose();
		return ret;
	}

	// HFS 헤더 읽고 확인
	private bool ReadHeader(out Hfs.Header retHeader)
	{
		byte[] buffer = new byte[Marshal.SizeOf<Hfs.Header>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			retHeader = new Hfs.Header();
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		Hfs.Header header = Marshal.PtrToStructure<Hfs.Header>(handle.AddrOfPinnedObject());
		handle.Free();

		if (header.header != HFS_HEADER || header.version != HFS_VERSION)
		{
			retHeader = new Hfs.Header();
			return false;
		}

		_desc = Encoding.UTF8.GetString(header.desc).TrimEnd('\0');
		retHeader = header;
		return true;
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

		byte[] buffer = new byte[Marshal.SizeOf<Hfs.Header>()];
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		Hfs.Header header = new Hfs.Header
		{
			header = HFS_HEADER,
			version = HFS_VERSION,
			notuse = 0,
			stc = QsSupp.ToStamp(now),
			stw = QsSupp.ToStamp(now),
			revision = 0,
			desc = new byte[64],
		};
		Encoding.UTF8.GetBytes(desc).CopyTo(header.desc, 0);
		Marshal.StructureToPtr(header, handle.AddrOfPinnedObject(), false);
		handle.Free();
		try
		{
			_fs.Write(buffer, 0, buffer.Length);
		}
		catch (Exception)
		{
			return false;
		}
		WriteDirectory(".", 0, (uint)HFSAT_ROOT, 0);
		return true;
	}

	// 파일 헤더 쓰기
	private bool WriteFileHeader(ref Hfs.FileData file, PathString name)
	{
		(file.source.len, file.hash) = name.LenAndHash;

		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileData>()];
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		Marshal.StructureToPtr(file, handle.AddrOfPinnedObject(), false);
		handle.Free();

		try
		{
			_fs.Write(buffer, 0, buffer.Length);
			_fs.Write(name.ToBytes(), 0, name.Length);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	// 디렉토리 쓰기
	private bool WriteDirectory(PathString name, ulong stc, uint subp, uint next)
	{
		if (stc == 0)
			stc = QsSupp.ToStamp(DateTime.Now);
		Hfs.FileData file = new Hfs.FileData
		{
			source = new Hfs.FileSource
			{
				attr = (byte)Hfs.FileAttr.Dir,
				type = (byte)Hfs.FileType.Dir,
				size = (uint)_rand.Next(),
				cmpr = (uint)_rand.Next(),
				seek = (uint)_rand.Next()
			},
			stc = stc,
			subp = subp,
			next = next,
		};
		return WriteFileHeader(ref file, name);
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

	// 디렉토리 찾기
	private bool FindDirectory(ref Hfs.FileInfo info, PathString name)
	{
		var (len, hash) = name.LenAndHash;

		while (ReadHfsFile(out info.file))
		{
			if (info.file.source.Type == Hfs.FileType.Dir && info.file.hash == hash && info.file.source.len == len)
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
				if (name.Equals(info.name))
					return true;
			}
			if (info.file.next == 0)
				break;
			if (_fs.Seek(info.file.next, SeekOrigin.Begin) != info.file.next)
				break;
		}
		return false;
	}

	// HFS 파일 읽기
	private bool ReadHfsFile(out Hfs.FileData file)
	{
		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileData>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			file = new Hfs.FileData();
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		file = Marshal.PtrToStructure<Hfs.FileData>(handle.AddrOfPinnedObject());
		handle.Free();
		return true;
	}

	// HFS 파일 읽기 (파일 이름 포함)
	private bool ReadHfsFile(out Hfs.FileInfo info)
	{
		info = new Hfs.FileInfo();

		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileData>()];
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
		info.file = Marshal.PtrToStructure<Hfs.FileData>(handle.AddrOfPinnedObject());
		handle.Free();
		buffer = new byte[info.file.source.len];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			info = new Hfs.FileInfo();
			return false;
		}

		info.name = Encoding.UTF8.GetString(buffer);
		info.stc = QsSupp.ToDateTime(info.file.stc);
		return true;
	}

	// 디렉터리 저장
	private bool SaveDirectory(string directory, out string save)
	{
		if (string.IsNullOrWhiteSpace(directory) || string.Equals(directory, _path, StringComparison.OrdinalIgnoreCase))
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

	// 버퍼 저장
	private bool StoreBuffer(string path, byte[] data)
	{
		if (path.Length >= QN_MAX_PATH - 1)
		{
			_mesg = $"전체 파일 이름이 너무 길어요. 최대 {QN_MAX_PATH - 1} 글자까지만 지원해요";
			return false;
		}

		var (dir, filename) = QsSupp.DivPath(path);
		if (filename[0] == '.')
		{
			_mesg = "'.'으로 시작하는 파일은 저장할 수 없어요";
			return false;
		}

		var name = new PathString(filename);
		if (name.Length >= HFS_MAX_NAME)
		{
			_mesg = $"파일 이름이 너무 길어요. 최대 {HFS_MAX_NAME - 1} 글자까지만 지원해요 (UTF-8)";
			return false;
		}
		if (SaveDirectory(dir, out var save) == false)
		{
			_mesg = "디렉토리를 찾을 수 없어요";
			return false;
		}

		var (len, hash) = name.LenAndHash;
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && len == info.file.source.len && name.Equals(info.name))
			{
				RestoreDirectory(save);
				_mesg = "이미 있는 파일에요";
				return false;
			}
		}

		var type = ExtensionSupp.GetType(filename);
		var rate = ExtensionSupp.GetTypeRate(type);
		var attr = ExtensionSupp.GetTypeAttr(type);

		var cmpr_size = 0;
		var bufsize = data.Length;
		var buffer = data;

		try
		{
			if (rate > 0.0)
			{
				using var mem = new MemoryStream();
				using var deflate = new DeflateStream(mem, CompressionLevel.SmallestSize, true);
				deflate.Write(data, 0, data.Length);
				deflate.Flush();
				deflate.Close();

				double d = bufsize * rate;
				if (mem.Length < d)
				{
					buffer = mem.ToArray();
					cmpr_size = (int)mem.Length;
				}
			}
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			_mesg = "압축을 할 수 없어요";
			return false;
		}

		uint next;
		Hfs.FileInfo file;
		try
		{
			_fs.Seek(0, SeekOrigin.End);
			next = (uint)_fs.Position;

			file = new Hfs.FileInfo
			{
				file = new Hfs.FileData
				{
					source = new Hfs.FileSource
					{
						attr = (byte)Hfs.FileAttr.File,
						type = (byte)type,
						size = (uint)bufsize,
						cmpr = 0,
						seek = 0,
					},
					stc = QsSupp.ToStamp(DateTime.Now),
					subp = 0,
					next = 0,
				},
				name = filename,
			};
			if (cmpr_size > 0)
			{
				file.file.source.attr |= (byte)Hfs.FileAttr.Compress;
				file.file.source.cmpr = (uint)cmpr_size;
				if (WriteFileHeader(ref file.file, name) == false)
					throw new Exception();
				_fs.Write(buffer, 0, cmpr_size);
			}
			else
			{
				file.file.source.attr |= (byte)attr;
				if (WriteFileHeader(ref file.file, name) == false)
					throw new Exception();
				_fs.Write(buffer, 0, bufsize);
			}
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			_mesg = "HFS 파일을 조작할 수 없어요";
			return false;
		}

		Hfs.FileInfo last = _infos.Last();
		last.file.next = (uint)_fs.Position;
		_fs.Seek(HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));

		file.file.source.seek = next;
		_infos.Add(file);

		RestoreDirectory(save);
		_touch++;
		return true;
	}

	// 최적화용 파일 읽기
	private byte[] OptimizedRead(ref Hfs.FileSource source, out uint size)
	{
		try
		{
			_fs.Seek(source.seek + HFSSIZE_FILE + source.len, SeekOrigin.Begin);
		}
		catch (Exception)
		{
			size = 0;
			return Array.Empty<byte>();
		}

		size = source.IsCompressed ? source.cmpr : source.size;
		byte[] buffer = new byte[size];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != size)
				throw new Exception();
		}
		catch (Exception)
		{
			size = 0;
			return Array.Empty<byte>();
		}
		return buffer;
	}

	// 최적화 진행
	private static bool OptimizeProcess(ref Hfs.OptimizeData data)
	{
		var input = data.Input!;
		var output = data.Output!;
		var infos = input.GetFiles().ToArray();

		data.FileName = input.CurrentDirectory;
		data.Count++;
		data.OptimizeEvent?.Invoke(data);

		foreach (var info in infos)
		{
			if (info.name[0] == '.')
				continue;

			if (info.file.IsDirectory)
			{
				output.MkDir(info.name);
				output.ChDir(info.name);
				input.ChDir(info.name);

				data.Stack++;
				if (OptimizeProcess(ref data) == false)
					return false;

				data.Stack--;
				output.ChDir("..");
				input.ChDir("..");

				continue;
			}

			data.FileName = info.name;
			data.Count++;
			data.OptimizeEvent?.Invoke(data);

			var buffer = input.OptimizedRead(ref info.file.source, out var size);
			if (buffer.Length == 0)
				return false;

			try
			{
				Stream stream = output._fs;
				stream.Seek(0, SeekOrigin.End);
				uint next = (uint)stream.Position;

				var file = new Hfs.FileInfo();
				file.file = info.file;

				if (output.WriteFileHeader(ref file.file, info.name) == false)
					return false;
				stream.Write(buffer, 0, buffer.Length);

				Hfs.FileInfo last = output._infos.Last();
				last.file.next = next;
				stream.Seek(HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
				stream.Write(BitConverter.GetBytes(next), 0, sizeof(uint));

				file.file.source.seek = next;
				file.name = info.name;
				output._infos.Add(file);
			}
			catch (Exception)
			{
				return false;
			}
		}

		return true;
	}
}

