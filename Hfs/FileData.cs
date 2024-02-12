using System.Runtime.InteropServices;

namespace QsHfs.Hfs;

[StructLayout(LayoutKind.Sequential)]
internal struct FileData
{
	public FileSource source;
	public ulong stc;
	public uint hash;
	public uint notuse;
	public uint subp;
	public uint next;

	public override string ToString()
	{
		return source.ToString();
	}

	public bool IsDirectory => (source.Attr & FileAttr.Dir) == FileAttr.Dir;

	public bool IsCompressed => (source.Attr & FileAttr.Compress) == FileAttr.Compress;

	public FileAttr Attr => (FileAttr)source.attr;

	public FileType Type => (FileType)source.type;
}
