using System.Runtime.InteropServices;

namespace QsHfs.Hfs;

[StructLayout(LayoutKind.Sequential)]
internal struct FileSource
{
	public byte attr;
	public byte type;
	public ushort len;
	public uint size;
	public uint cmpr;
	public uint seek;

	override public string ToString()
	{
		FileType t = (FileType)type;
		if (t == FileType.Dir)
			return $"[디렉토리] <{(FileAttr)attr}/{(FileType)type}>";
		else
			return $"크기: {size}({cmpr}) <{(FileAttr)attr}/{(FileType)type}>";
	}

	public bool IsDirectory => ((FileAttr)attr & FileAttr.Dir) == FileAttr.Dir;

	public bool IsCompressed => ((FileAttr)attr & FileAttr.Compress) == FileAttr.Compress;

	public FileAttr Attr => (FileAttr)attr;

	public FileType Type => (FileType)type;

}
