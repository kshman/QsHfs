namespace QsHfs;

internal static class ExtensionSupp
{
	private record FileTypeInfo
	{
		public string Name;
		public Hfs.FileType Type;
		public Hfs.FileAttr Attr;

		public FileTypeInfo(string name, Hfs.FileType type, Hfs.FileAttr attr)
		{
			Name = name;
			Type = type;
			Attr = attr;
		}
	}

	private static Dictionary<Hfs.FileType, FileTypeInfo> LocaleTypes = new Dictionary<Hfs.FileType, FileTypeInfo>
	{
		{ Hfs.FileType.Unknown, new FileTypeInfo("<모름>", Hfs.FileType.Unknown,Hfs.FileAttr.None) },
		{ Hfs.FileType.Model, new FileTypeInfo("모델" , Hfs.FileType.Model,Hfs.FileAttr.None)},
		{ Hfs.FileType.Archive, new FileTypeInfo("아카이브" , Hfs.FileType.Archive,Hfs.FileAttr.Indirect)},
		{ Hfs.FileType.Script, new FileTypeInfo("스크립트" , Hfs.FileType.Script,Hfs.FileAttr.None)},
		{ Hfs.FileType.Texture, new FileTypeInfo("텍스쳐" , Hfs.FileType.Texture,Hfs.FileAttr.None)},
		{ Hfs.FileType.Video, new FileTypeInfo("비디오" , Hfs.FileType.Video,Hfs.FileAttr.Indirect)},
		{ Hfs.FileType.Code, new FileTypeInfo("소스코드" , Hfs.FileType.Code,Hfs.FileAttr.None)},
		{ Hfs.FileType.Image, new FileTypeInfo("이미지" , Hfs.FileType.Image,Hfs.FileAttr.None)},
		{ Hfs.FileType.Json, new FileTypeInfo("JSON" , Hfs.FileType.Json,Hfs.FileAttr.None)},
		{ Hfs.FileType.Sound, new FileTypeInfo("소리" , Hfs.FileType.Sound,Hfs.FileAttr.None)},
		{ Hfs.FileType.SoundEffect, new FileTypeInfo("소리이펙트" , Hfs.FileType.SoundEffect,Hfs.FileAttr.None)},
		{ Hfs.FileType.Document, new FileTypeInfo("문서" , Hfs.FileType.Document,Hfs.FileAttr.None)},
		{ Hfs.FileType.MarkUp, new FileTypeInfo("마크업" , Hfs.FileType.MarkUp,Hfs.FileAttr.None)},
		{ Hfs.FileType.Animation, new FileTypeInfo("애니메이션" , Hfs.FileType.Animation,Hfs.FileAttr.None)},
		{ Hfs.FileType.Text, new FileTypeInfo("텍스트" , Hfs.FileType.Text,Hfs.FileAttr.None)},
		{ Hfs.FileType.DataText, new FileTypeInfo("데이터텍스트" , Hfs.FileType.DataText,Hfs.FileAttr.None)},
		{ Hfs.FileType.MarkDown, new FileTypeInfo("마크다운" , Hfs.FileType.MarkDown,Hfs.FileAttr.None)},
		{ Hfs.FileType.Program, new FileTypeInfo("응용프로그램" , Hfs.FileType.Program,Hfs.FileAttr.None)}
	};

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
		{ ".bin", Hfs.FileType.Archive },
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
		{ ".hxa", Hfs.FileType.Image },
		{ ".hxm", Hfs.FileType.Image },
		{ ".hxv", Hfs.FileType.Video },
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
		{ ".qoi", Hfs.FileType.Image },
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
		{ ".tgz", Hfs.FileType.Archive },
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

	public static string GetTypeName(Hfs.FileType type)
	{
		if (LocaleTypes.TryGetValue(type, out var info))
			return info.Name;
		return "<모름>";
	}

	public static Hfs.FileAttr GetTypeAttr(Hfs.FileType type)
	{
		if (LocaleTypes.TryGetValue(type, out var info))
			return info.Attr;
		return Hfs.FileAttr.None;
	}

	public static double GetTypeRate(Hfs.FileType type)
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
