using System.Collections;

namespace QsHfs;

internal class ListFilesComparer : IComparer<ListViewItem>, IComparer
{
	private int _column;

	public ListFilesComparer(int column = 0)
	{
		_column = column;
	}

	public int Compare(ListViewItem? x, ListViewItem? y)
	{
		return string.Compare(x?.SubItems[_column].Text, y?.SubItems[_column].Text);
	}

	public int Compare(object? x, object? y)
	{
		return Compare(x as ListViewItem, y as ListViewItem);
	}
}
