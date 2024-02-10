namespace QsHfs.Hfs;

internal class Info
{
	public FileContent file;
	public string name;
	public DateTime stc;

	public Info()
	{
		file = new FileContent();
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
