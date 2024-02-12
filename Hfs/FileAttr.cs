namespace QsHfs.Hfs;

internal enum FileAttr
{
	#region HFS 사용값
	None = 0,
	File = 1 << 0,
	Dir = 1 << 1,
	Link = 1 << 2,
	Compress = 1 << 3,
	Encrypt = 1 << 4,
	Indirect = 1 << 5,
	#endregion

	#region 확장값 (16비트)
	System = 1 << 8,
	Hidden = 1 << 9,
	ReadOnly = 1 << 10,
	#endregion
}
