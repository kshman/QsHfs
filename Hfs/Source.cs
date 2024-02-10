using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QsHfs.Hfs;

[StructLayout(LayoutKind.Sequential)]
internal struct Source
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

	public bool IsDirectory => ((FileAttr)attr & FileAttr.Dir) == FileAttr.Dir;

	public bool IsCompressed => ((FileAttr)attr & FileAttr.Compressed) == FileAttr.Compressed;

	public FileType FileType => (FileType)type;
}
