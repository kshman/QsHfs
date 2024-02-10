namespace QsHfs.Hfs;

internal enum FileAttr
{
	None = 0,
	File = 1 << 0,
	Dir = 1 << 1,
	System = 1 << 2,
	Hidden = 1 << 3,
	ReadOnly = 1 << 4,
	Link = 1 << 5,
	Compressed = 1 << 6,
	Encrypt = 1 << 7,
}
