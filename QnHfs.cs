#if DEBUG
#define DEBUG_TRACE
#endif
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using QsHfs.Hfs;
using Debug = System.Diagnostics.Debug;

namespace QsHfs;

internal class QnHfs : IDisposable
{
	const uint HFS_HEADER = 0x00534648;
	const ushort HFS_VERSION = 200;

	internal static readonly char[] directory_separator = ['\\', '/', '\n', '\r', '\t'];

	private readonly nint HFSAT_ROOT;
	private readonly nint HFSAT_NEXT;
	private readonly uint HFSFILE_SIZE;

	private readonly string _name;
	private readonly FileStream _fs;
	private uint _touch = 0;

	private string _desc = string.Empty;
	private string _path = string.Empty;
	private readonly List<Hfs.Info> _infos = [];

	private readonly Random _rand = new();

	public QnHfs(string path)
		: this(path, false)
	{
	}

	public QnHfs(string path, bool create)
	{
		HFSAT_ROOT = Marshal.SizeOf<Hfs.Header>();
		HFSAT_NEXT = Marshal.OffsetOf<Hfs.FileContent>(nameof(Hfs.FileContent.next));
		HFSFILE_SIZE = (uint)Marshal.SizeOf<Hfs.FileContent>();

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

		if (ReadHeader() == false)
			throw new InvalidDataException();

		_name = Path.GetFileName(path);
		ChDir("/");
	}

	// 소멸자
	public void Dispose()
	{
		_fs.Close();
	}

	// 닫기
	public void Close()
	{
		_fs.Close();
	}

	// 파일 목록
	public List<Hfs.Info> GetFiles()
	{
		return _infos;
	}

	// 이름
	public string Name => _name;

	// 경로
	public string CurrentDirectory => _path;

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

		var len = QsSupp.Slen(name);
		var next = (uint)_fs.Position;
		WriteDirectory(name, 0, (uint)(next + HFSFILE_SIZE + len), 0);
		var curr = (uint)_fs.Position;
		WriteDirectory(".", (uint)(curr + HFSFILE_SIZE + 1), (uint)(next + HFSFILE_SIZE + len), 0);
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
			Hfs.Info first = _infos.First();
			_fs.Seek(first.file.source.seek, SeekOrigin.Begin);
		}

		if (directory[0] == '/')
		{
			_fs.Seek(HFSAT_ROOT, SeekOrigin.Begin);
			_path = "/";
		}

		var info = new Hfs.Info();
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

		var (len, hash) = QsSupp.StringLenAndHash(name);
		Hfs.Info? found = null;
		int i = 0;
		for (i = 0; i < _infos.Count; i++)
		{
			var info = _infos[i];
			if (hash == info.file.hash && len == info.file.source.len &&
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

		uint next = found.file.next;
		Hfs.Info prev = _infos[i - 1];
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
		byte[] buffer = new byte[Marshal.SizeOf<Hfs.Header>()];
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
		Hfs.Header header = Marshal.PtrToStructure<Hfs.Header>(handle.AddrOfPinnedObject());
		handle.Free();

		if (header.header != HFS_HEADER || header.version != HFS_VERSION)
			return false;

		_desc = Encoding.UTF8.GetString(header.desc).TrimEnd('\0');
		return true;
	}

	// HFS 파일 읽기
	private bool ReadHfsFile(out Hfs.FileContent file)
	{
		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileContent>()];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			file = new Hfs.FileContent();
			return false;
		}
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		file = Marshal.PtrToStructure<Hfs.FileContent>(handle.AddrOfPinnedObject());
		handle.Free();
		return true;
	}

	// HFS 파일 읽기 (파일 이름 포함)
	private bool ReadHfsFile(out Hfs.Info info)
	{
		info = new Hfs.Info();

		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileContent>()];
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
		info.file = Marshal.PtrToStructure<Hfs.FileContent>(handle.AddrOfPinnedObject());
		handle.Free();
		buffer = new byte[info.file.source.len];
		try
		{
			if (_fs.Read(buffer, 0, buffer.Length) != buffer.Length)
				throw new Exception();
		}
		catch (Exception)
		{
			info = new Hfs.Info();
			return false;
		}

		info.name = Encoding.UTF8.GetString(buffer);
		info.stc = QsSupp.ToDateTime(info.file.stc);
		return true;
	}

	// 디렉토리 찾기
	private bool FindDirectory(ref Hfs.Info info, string name, uint hash)
	{
		var len = QsSupp.Slen(name);
		while (ReadHfsFile(out info.file))
		{
			if (info.file.source.attr == 2 && info.file.source.len == len && info.file.hash == hash)
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
				if (name.Equals(info.name, StringComparison.InvariantCultureIgnoreCase))
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

	// 디렉토리 쓰기
	private bool WriteDirectory(string name, uint next, uint meta, long stc)
	{
		if (stc == 0)
			stc = QsSupp.ToStamp(DateTime.Now);
		Hfs.FileContent file = new Hfs.FileContent
		{
			source = new Hfs.Source
			{
				type = (byte)Hfs.FileType.System,
				attr = (byte)Hfs.FileAttr.Dir,
				size = (uint)_rand.Next(),
				cmpr = (uint)_rand.Next(),
				seek = (uint)_rand.Next()
			},
			meta = meta,
			next = next,
			stc = stc,
		};
		return WriteFileHeader(ref file, name);
	}

	// 파일 헤더 쓰기
	private bool WriteFileHeader(ref Hfs.FileContent file, string name)
	{
		byte[] namebuf = Encoding.UTF8.GetBytes(name);
		var (len, hash) = QsSupp.StringLenAndHash(name);
		file.source.len = (ushort)len;
		file.hash = hash;

		byte[] buffer = new byte[Marshal.SizeOf<Hfs.FileContent>()];
		GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		Marshal.StructureToPtr(file, handle.AddrOfPinnedObject(), false);
		handle.Free();

		try
		{
			_fs.Write(buffer, 0, buffer.Length);
			_fs.Write(namebuf, 0, namebuf.Length);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	// 소스 읽기
	public byte[] SourceRead(Hfs.Source source)
	{
		try
		{
			_fs.Seek(source.seek + HFSFILE_SIZE + source.len, SeekOrigin.Begin);

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
			return Array.Empty<byte>();
		}
	}

	// 파일 읽기
	public byte[] Read(string filename, out int size)
	{
		var (dir, name) = QsSupp.DivPath(filename);
		if (name[0] == '.' ||
			name.Length >= 260 ||
			SaveDirectory(dir, out var save) == false)
		{
			size = 0;
			return Array.Empty<byte>();
		}

		var (len, hash) = QsSupp.StringLenAndHash(name);
		Hfs.Info? found = null;
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && len == info.file.source.len &&
				name.Equals(info.name, StringComparison.InvariantCultureIgnoreCase))
			{
				found = info;
				break;
			}
		}
		if (found == null)
		{
			RestoreDirectory(save);
			size = 0;
			return Array.Empty<byte>();
		}

		byte[] buffer = SourceRead(found.file.source);
		RestoreDirectory(save);
		size = buffer.Length;
		return buffer;
	}

	// 버퍼 저장
	private bool StoreBuffer(string filename, byte[] data)
	{
		var (dir, name) = QsSupp.DivPath(filename);
		if (name.Length >= 260)
			return false;
		if (SaveDirectory(dir, out var save) == false)
			return false;

		uint hash = QsSupp.Shash(name);
		foreach (var info in _infos)
		{
			if (hash == info.file.hash && name.Equals(info.name, StringComparison.InvariantCultureIgnoreCase))
			{
				RestoreDirectory(save);
				return false;
			}
		}

		var type = ExtensionSupp.GetType(name);
		var rate = ExtensionSupp.GetRate(type);

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
			return false;
		}

		uint next;
		Hfs.Info file;
		try
		{
			_fs.Seek(0, SeekOrigin.End);
			next = (uint)_fs.Position;

			file = new Hfs.Info
			{
				file = new Hfs.FileContent
				{
					source = new Hfs.Source
					{
						type = (byte)type,
						attr = (byte)Hfs.FileAttr.File,
						size = (uint)bufsize,
						cmpr = 0,
						seek = 0,
					},
					meta = 0,
					next = 0,
					stc = QsSupp.ToStamp(DateTime.Now),
				},
				name = name,
			};
			if (cmpr_size > 0)
			{
				file.file.source.attr |= (byte)Hfs.FileAttr.Compressed;
				file.file.source.cmpr = (uint)cmpr_size;
				if (WriteFileHeader(ref file.file, name) == false)
					throw new Exception();
				_fs.Write(buffer, 0, cmpr_size);
			}
			else
			{
				if (WriteFileHeader(ref file.file, name) == false)
					throw new Exception();
				_fs.Write(buffer, 0, bufsize);
			}
		}
		catch (Exception)
		{
			RestoreDirectory(save);
			return false;
		}

		Hfs.Info last = _infos.Last();
		last.file.next = (uint)_fs.Position;
		_fs.Seek(HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
		_fs.Write(BitConverter.GetBytes(next), 0, sizeof(uint));

		file.file.source.seek = next;
		_infos.Add(file);

		RestoreDirectory(save);
		return true;
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

	// 최적화용 파일 읽기
	private byte[] OptimizedRead(ref Hfs.Source source, out uint size)
	{
		try
		{
			_fs.Seek(source.seek + HFSFILE_SIZE + source.len, SeekOrigin.Begin);
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

				var file = new Hfs.Info();
				file.file = info.file;

				if (output.WriteFileHeader(ref file.file, info.name) == false)
					return false;
				stream.Write(buffer, 0, buffer.Length);

				Hfs.Info last = output._infos.Last();
				last.file.next = next;
				stream.Seek(input.HFSAT_NEXT + last.file.source.seek, SeekOrigin.Begin);
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
}

