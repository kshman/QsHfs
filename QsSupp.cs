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

	public static uint Shash(string s)
	{
		uint hash = 0, cnt = 0;
		var u8 = System.Text.Encoding.UTF8.GetBytes(s.ToLower());
		int len = Math.Min(u8.Length, 256);
		for (int i = 0; i < len; i++, cnt++)
			hash = (hash << 5) - hash + u8[i];
		hash = (hash << 5) - hash + cnt - 1;
		return hash;
	}

	public static DateTime ToDateTime(long stamp)
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

	public static long ToStamp(DateTime dt)
	{
		long stamp = dt.Millisecond;
		stamp = (stamp << 8) + dt.Second;
		stamp = (stamp << 8) + dt.Minute;
		stamp = (stamp << 6) + dt.Hour;
		stamp = (stamp << 4) + (int)dt.DayOfWeek;
		stamp = (stamp << 8) + dt.Day;
		stamp = (stamp << 6) + dt.Month;
		stamp = (stamp << 14) + dt.Year;
		return stamp;
	}
}
