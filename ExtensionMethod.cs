using System.ComponentModel;

namespace QsHfs;

internal static class ExtensionMethod
{
	public static TResult SafeInvoke<T, TResult>(this T isi, Func<T, TResult> call) where T : ISynchronizeInvoke
	{
		if (isi.InvokeRequired)
		{
			IAsyncResult result = isi.BeginInvoke(call, new object[] { isi });
			var endResult = isi.EndInvoke(result); return (TResult)endResult;
		}
		else
			return call(isi);
	}

	public static void SafeInvoke<T>(this T isi, Action<T> call) where T : ISynchronizeInvoke
	{
		if (isi.InvokeRequired) isi.BeginInvoke(call, new object[] { isi });
		else
			call(isi);
	}
}
