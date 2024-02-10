namespace QsHfs;

internal static class ExtensionSupp
{
	private static Dictionary<string, Hfs.FileType> FileTypes = new Dictionary<string, Hfs.FileType>
	{
		{ ".3ds", Hfs.FileType.Model },
		{ ".7z", Hfs.FileType.Archive },
		{ ".asp", Hfs.FileType.Script },
		{ ".aspx", Hfs.FileType.Script },
		{ ".astc", Hfs.FileType.Texture },
		{ ".avi", Hfs.FileType.Video },
		{ ".bat", Hfs.FileType.Script },
		{ ".bc", Hfs.FileType.Texture },
		{ ".blend", Hfs.FileType.Model },
		{ ".bmp", Hfs.FileType.Image },
		{ ".bson", Hfs.FileType.Json },
		{ ".bz2", Hfs.FileType.Archive },
		{ ".c", Hfs.FileType.Code },
		{ ".cc", Hfs.FileType.Code },
		{ ".cmd", Hfs.FileType.Script },
		{ ".cmpr", Hfs.FileType.Archive },
		{ ".cpp", Hfs.FileType.Code },
		{ ".crn", Hfs.FileType.Texture },
		{ ".cs", Hfs.FileType.Code },
		{ ".css", Hfs.FileType.Text },
		{ ".csv", Hfs.FileType.DataText },
		{ ".dds", Hfs.FileType.Texture },
		{ ".dll", Hfs.FileType.Program },
		{ ".dwg", Hfs.FileType.Model },
		{ ".dxf", Hfs.FileType.Model },
		{ ".dylib", Hfs.FileType.Program },
		{ ".etc", Hfs.FileType.Texture },
		{ ".exe", Hfs.FileType.Program },
		{ ".exr", Hfs.FileType.Image },
		{ ".f", Hfs.FileType.Code },
		{ ".fbx", Hfs.FileType.Model },
		{ ".flv", Hfs.FileType.Video },
		{ ".gif", Hfs.FileType.Image },
		{ ".glb", Hfs.FileType.Model },
		{ ".gltf", Hfs.FileType.Model },
		{ ".gz", Hfs.FileType.Archive },
		{ ".h", Hfs.FileType.Code },
		{ ".hdr", Hfs.FileType.Image },
		{ ".hh", Hfs.FileType.Code },
		{ ".hpp", Hfs.FileType.Code },
		{ ".hra", Hfs.FileType.Animation },
		{ ".hrm", Hfs.FileType.Model },
		{ ".hs", Hfs.FileType.Script },
		{ ".htm", Hfs.FileType.MarkUp },
		{ ".html", Hfs.FileType.MarkUp },
		{ ".ico", Hfs.FileType.Image },
		{ ".iges", Hfs.FileType.Model },
		{ ".igs", Hfs.FileType.Model },
		{ ".java", Hfs.FileType.Code },
		{ ".jpeg", Hfs.FileType.Image },
		{ ".jpg", Hfs.FileType.Image },
		{ ".js", Hfs.FileType.Script },
		{ ".json", Hfs.FileType.Json },
		{ ".json5", Hfs.FileType.Json },
		{ ".jsp", Hfs.FileType.Script },
		{ ".ktx", Hfs.FileType.Texture },
		{ ".lnk", Hfs.FileType.System },
		{ ".lua", Hfs.FileType.Script },
		{ ".md", Hfs.FileType.MarkDown },
		{ ".mkv", Hfs.FileType.Video },
		{ ".mp3", Hfs.FileType.Sound },
		{ ".mp4", Hfs.FileType.Video },
		{ ".mtl", Hfs.FileType.Model },
		{ ".obj", Hfs.FileType.Model },
		{ ".ogg", Hfs.FileType.Sound },
		{ ".pdf", Hfs.FileType.Document },
		{ ".php", Hfs.FileType.Script },
		{ ".pkm", Hfs.FileType.Texture },
		{ ".ply", Hfs.FileType.Model },
		{ ".png", Hfs.FileType.Image },
		{ ".ps1", Hfs.FileType.Script },
		{ ".psd", Hfs.FileType.Image },
		{ ".pvr", Hfs.FileType.Texture },
		{ ".py", Hfs.FileType.Script },
		{ ".qra", Hfs.FileType.Animation },
		{ ".qrm", Hfs.FileType.Model },
		{ ".rar", Hfs.FileType.Archive },
		{ ".rb", Hfs.FileType.Script },
		{ ".s3tc", Hfs.FileType.Texture },
		{ ".sh", Hfs.FileType.Script },
		{ ".so", Hfs.FileType.Program },
		{ ".step", Hfs.FileType.Model },
		{ ".stl", Hfs.FileType.Model },
		{ ".stp", Hfs.FileType.Model },
		{ ".svg", Hfs.FileType.Image },
		{ ".tar", Hfs.FileType.Archive },
		{ ".tga", Hfs.FileType.Image },
		{ ".tif", Hfs.FileType.Image },
		{ ".tiff", Hfs.FileType.Image },
		{ ".toml", Hfs.FileType.DataText },
		{ ".ts", Hfs.FileType.Script },
		{ ".tsv", Hfs.FileType.DataText },
		{ ".txt", Hfs.FileType.Text },
		{ ".wasm", Hfs.FileType.Program  },
		{ ".wav", Hfs.FileType.SoundEffect },
		{ ".webm", Hfs.FileType.Video },
		{ ".webp", Hfs.FileType.Image },
		{ ".wmv", Hfs.FileType.Video },
		{ ".x", Hfs.FileType.Model },
		{ ".xht", Hfs.FileType.MarkUp },
		{ ".xhtml", Hfs.FileType.MarkUp },
		{ ".xml", Hfs.FileType.MarkUp },
		{ ".xz", Hfs.FileType.Archive },
		{ ".yaml", Hfs.FileType.DataText },
		{ ".yml", Hfs.FileType.DataText },
		{ ".zip", Hfs.FileType.Archive },
		{ ".iso", Hfs.FileType.Archive }
	};

	public static Hfs.FileType GetType(string filename)
	{
		var ext = Path.GetExtension(filename).ToLower();
		if (FileTypes.TryGetValue(ext, out var type))
			return type;
		return Hfs.FileType.Unknown;
	}

	public static double GetRate(Hfs.FileType type)
	{
		switch (type)
		{
			case Hfs.FileType.Image:
			case Hfs.FileType.Sound:
			case Hfs.FileType.SoundEffect:
			case Hfs.FileType.Texture:
				return 0.6;
			case Hfs.FileType.Video:
			case Hfs.FileType.Archive:
				return -1.0;
			default:
				return 0.96;
		}
	}
}
