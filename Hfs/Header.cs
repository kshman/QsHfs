using System.Runtime.InteropServices;
using System.Text;

namespace QsHfs.Hfs;

[StructLayout(LayoutKind.Sequential)]

internal struct Header
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
