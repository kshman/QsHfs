namespace QsHfs;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();

#if DEBUG
		Test();
#endif
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		ListFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		//ListFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{

	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
	{

	}

	private void TsBtnOpen_Click(object sender, EventArgs e)
	{

	}

	private void TsBtnOptimize_Click(object sender, EventArgs e)
	{

	}

	private void TsBtnMkDir_Click(object sender, EventArgs e)
	{

	}

	private void TsBtnRemove_Click(object sender, EventArgs e)
	{

	}

	private void TsMiFileExport_Click(object sender, EventArgs e)
	{

	}

	private void TsMiFileRemove_Click(object sender, EventArgs e)
	{

	}

	private void ListFiles_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
			Cursor = Cursors.Hand;
		}
	}

	private void ListFiles_DragDrop(object sender, DragEventArgs e)
	{
		Cursor = Cursors.Default;
		if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
			return;
		var files = e.Data.GetData(DataFormats.FileDrop) as string[];
		if (files == null || files.Length == 0)
			return;
		foreach (var f in files)
			System.Diagnostics.Debug.WriteLine(f);
	}

	private void ListFiles_Layout(object sender, LayoutEventArgs e)
	{

	}

	private void ListFiles_MouseDoubleClick(object sender, MouseEventArgs e)
	{

	}

	private void ListFiles_MouseUp(object sender, MouseEventArgs e)
	{

	}

	private void ListFiles_ColumnClick(object sender, ColumnClickEventArgs e)
	{

	}

	private void ListFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	{

	}

	//
	private void Test()
	{
		QsHfs hfs = new QsHfs("G:\\test.hfs");
	}
}
