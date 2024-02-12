using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QsHfs;
internal static class QsSupp
{
	public static (string, string) DivPath(string path)
	{
		int index = path.LastIndexOf('\\');
		if (index == -1)
			return (string.Empty, path);
		else
			return (path[..index], path[(index + 1)..]);
	}

	public static DateTime ToDateTime(ulong stamp)
	{
		var year = (int)(stamp & 0x3FFF);
		var month = (int)((stamp >> 14) & 0x3F);
		var day = (int)((stamp >> 20) & 0xFF);
		var dow = (int)((stamp >> 28) & 0xF);
		var hour = (int)((stamp >> 32) & 0x3F);
		var minute = (int)((stamp >> 38) & 0xFF);
		var second = (int)((stamp >> 46) & 0xFF);
		var millisecond = (int)((stamp >> 54) & 0x3FF);
		return new DateTime(year, month, day, hour, minute, second, millisecond);
	}

	public static ulong ToStamp(DateTime dt)
	{
		ulong stamp = (ulong)dt.Millisecond;
		stamp = (stamp << 8) + (ulong)dt.Second;
		stamp = (stamp << 8) + (ulong)dt.Minute;
		stamp = (stamp << 6) + (ulong)dt.Hour;
		stamp = (stamp << 4) + (ulong)dt.DayOfWeek;
		stamp = (stamp << 8) + (ulong)dt.Day;
		stamp = (stamp << 6) + (ulong)dt.Month;
		stamp = (stamp << 14) + (ulong)dt.Year;
		return stamp;
	}

	public static string SizeString(long size)
	{
		string sfx;
		double n;

		if (size < 1024)
		{
			sfx = "";
			n = size;
		}
		else if (size < 1024*1024)
		{
			sfx = "KB";
			n = size/1024.0;
		}
		else if (size < 1024*1024*1024)
		{
			sfx = "MB";
			n = size/1024.0/1024.0;
		}
		else
		{
			sfx = "GB";
			n = size/1024.0/1024.0/1024.0;
		}

		double t = n-(double)((int)n);
		if (t > 0.1)
			return $"{n:0.0} {sfx}";
		else
			return $"{n:0} {sfx}";
	}
}
