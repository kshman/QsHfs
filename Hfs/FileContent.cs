using System.Runtime.InteropServices;

namespace QsHfs.Hfs;

[StructLayout(LayoutKind.Sequential)]
internal struct FileContent
{
	public Source source;
	public long stc;
	public uint hash;
	public uint meta;
	public uint next;

	public override string ToString()
	{
		return source.ToString();
	}

	public bool IsDirectory => ((FileAttr)source.attr & FileAttr.Dir) == FileAttr.Dir;

	public bool IsCompressed => ((FileAttr)source.attr & FileAttr.Compressed) == FileAttr.Compressed;

	public FileType FileType => (FileType)source.type;
}
