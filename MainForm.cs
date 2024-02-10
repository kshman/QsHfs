namespace QsHfs;

public partial class MainForm : Form
{
	private bool _escape = false;
	private string _last_directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
	private string _last_extract = string.Empty;

	private QnHfs? _hfs = null;
	private List<string> _history = [];

	public MainForm()
	{
		InitializeComponent();

#if DEBUG
		_last_directory = "G:\\";
#endif
	}

	private void MainForm_Load(object sender, EventArgs e)
	{

	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{

	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
	{

	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
			case Keys.Escape:
				if (_escape)
					Close();
				_escape = true;
				return;

			case Keys.O:
				if (e.Control)
					OpenHfs(false);
				break;

			case Keys.N:
				if (e.Control)
					OpenHfs(true);
				break;

			case Keys.W:
				if (e.Control)
					CloseHfs();
				break;

			case Keys.F7:
			case Keys.Insert:
				MakeDirectory();
				break;

			case Keys.F8:
			case Keys.Delete:
				if (BtnRemove.Enabled)
					RemoveFiles();
				break;

			case Keys.X:
				if (!e.Control)
					ExtractFiles();
				break;

			case Keys.F5:
				ExtractFiles();
				break;

			case Keys.Enter:
				ExecFiles();
				break;

			case Keys.D0:
				if (e.Control)
					Optimize();
				break;
		}

		_escape = false;
	}

	private void BtnNewHfs_Click(object sender, EventArgs e)
	{
		OpenHfs(true);
	}

	private void BtnOpenHfs_Click(object sender, EventArgs e)
	{
		OpenHfs(false);
	}

	private void BtnMkDir_Click(object sender, EventArgs e)
	{
		MakeDirectory();
	}

	private void BtnRemove_Click(object sender, EventArgs e)
	{
		RemoveFiles();
	}

	private void BtnExtract_Click(object sender, EventArgs e)
	{
		ExtractFiles();
	}

	private void BtnHfsOptimize_Click(object sender, EventArgs e)
	{
		Optimize();
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

		if (files.Length == 1)
		{
			var file = files[0];
			if (file.EndsWith(".hfs", StringComparison.OrdinalIgnoreCase))
			{
				OpenHfs(file, false);
				return;
			}
		}

		StoreFiles(files);
	}

	private void ListFiles_Layout(object sender, LayoutEventArgs e)
	{
		//LayoutListFiles();
	}

	private void ListFiles_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (ListFiles.GetItemAt(e.X, e.Y) is null)
			return;
		ExecFiles();
	}

	private void ListFiles_MouseUp(object sender, MouseEventArgs e)
	{
		if (_hfs == null)
			return;

		int cnt = _history.Count;
		if (cnt == 0)
			return;

		if (e.Button == MouseButtons.XButton1)
		{
			string s = _history[cnt - 1];
			_history.RemoveAt(cnt - 1);
			_hfs.ChDir(s);
			UpdateFiles();
		}
	}

	private void ListFiles_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (ListFiles.Items.Count == 0)
			return;

		ListFiles.ListViewItemSorter = new ListFilesComparer(e.Column);
		ListFiles.Sort();
	}

	private void ListFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	{
		if (ListFiles.SelectedItems.Count == 0)
		{
			BtnRemove.Enabled = false;
			BtnExtract.Enabled = false;
			return;
		}

		BtnRemove.Enabled = true;
		BtnExtract.Enabled = true;

		if (ListFiles.SelectedItems.Count == 1)
		{
			if (ListFiles.SelectedItems[0].Tag is not Hfs.Info file ||
				file.file.IsDirectory)
				BtnExtract.Enabled = false;
		}
	}

	// 리스트 레이아웃
	private void LayoutListFiles()
	{
		ListFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		ColumnName.Width = Math.Max(ColumnName.Width, 100);
		ColumnType.Width = Math.Max(ColumnType.Width, 80);
		ColumnDateTime.Width = Math.Max(ColumnDateTime.Width, 100);
		ColumnSize.Width = Math.Max(ColumnSize.Width, 80);
		ColumnCmpr.Width = Math.Max(ColumnCmpr.Width, 80);
	}

	// HFS 열기
	private void OpenHfs(bool create)
	{
		string filename;

		if (create)
		{
			var dlg = new SaveFileDialog()
			{
				Title = "HFS 만들기",
				Filter = "HFS 파일 (*.hfs)|*.hfs|모든 파일 (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
				ShowHelp = false,
				OverwritePrompt = true,
				InitialDirectory = _last_directory,
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;
			filename = dlg.FileName;
		}
		else
		{
			var dlg = new OpenFileDialog()
			{
				Title = "HFS 만들기",
				Filter = "HFS 파일 (*.hfs)|*.hfs|모든 파일 (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
				CheckFileExists = false,
				CheckPathExists = false,
				ShowReadOnly = false,
				ReadOnlyChecked = false,
				ShowHelp = false,
				InitialDirectory = _last_directory,
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;
			filename = dlg.FileName;
		}

		OpenHfs(filename, create);
	}

	private void OpenHfs(string filename, bool create)
	{
		var directory = Path.GetDirectoryName(filename);
		if (string.IsNullOrEmpty(directory) == false)
			_last_directory = directory;

		try
		{
			_hfs?.Close();
			_hfs = new QnHfs(filename, create);
		}
		catch (Exception ex)
		{
			MesgBox.Show(this, "HFS 열기 오류", ex.Message, MessageBoxIcon.Error);
			_hfs = null;
		}

		UpdateFiles();

		if (_hfs != null)
		{
			BtnHfsOptimize.Enabled = true;
			BtnMkDir.Enabled = true;
			_history.Clear();
		}

		_last_extract = string.Empty;
	}

	// HFS 닫기
	private void CloseHfs()
	{
		if (_hfs == null)
			return;

		if (MesgBox.Show(this, "HFS 닫기", "진짜 닫을거예요?", MessageBoxIcon.Question, MessageBoxButtons.YesNo) == DialogResult.No)
			return;

		_hfs.Close();
		_hfs = null;

		UpdateFiles();

		BtnMkDir.Enabled = false;
		BtnHfsOptimize.Enabled = false;
	}

	// 파일 업데이트
	private void UpdateFiles()
	{
		ListFiles.Items.Clear();

		BtnRemove.Enabled = false;
		BtnExtract.Enabled = false;

		if (_hfs == null)
		{
			Text = "QsHfs";
			return;
		}

		Text = $"QsHfs - \"{_hfs.Name}\" <{_hfs.CurrentDirectory}>";
		var files = _hfs.GetFiles();

		ListFiles.BeginUpdate();

		foreach (var f in files)
		{
			var item = new ListViewItem(f.name);
			item.Tag = f;
			item.ToolTipText = f.name;
			if (f.file.IsDirectory)
			{
				item.SubItems.Add("[디렉토리]");
				item.SubItems.Add(QsSupp.ToDateTime(f.file.stc).ToString());
				item.SubItems.Add("");
				item.SubItems.Add("");
			}
			else
			{
				item.SubItems.Add(ExtensionSupp.GetTypeName(f.file.FileType));
				item.SubItems.Add(QsSupp.ToDateTime(f.file.stc).ToString());
				item.SubItems.Add(QsSupp.SizeString(f.file.source.size));
				item.SubItems.Add(f.file.IsCompressed ? QsSupp.SizeString(f.file.source.cmpr) : "<압축안됨>");
			}
			ListFiles.Items.Add(item);
		}
		ListFiles.ListViewItemSorter = new ListFilesComparer(0); // 이름
		ListFiles.Sort();
		ListFiles.ListViewItemSorter = new ListFilesComparer(1); // 타입
		ListFiles.Sort();

		LayoutListFiles();
		ListFiles.EndUpdate();
	}

	// 디렉토리 만들기
	private void MakeDirectory()
	{
		if (_hfs == null)
			return;

		var dlg = new LineInputForm()
		{
			Text = "디렉토리 만들기",
			LineText = "",
			EnableCancel = true,
		};

		if (dlg.ShowDialog() != DialogResult.OK)
			return;

		var name = dlg.LineText;
		if (string.IsNullOrEmpty(name))
			return;

		if (_hfs.MkDir(name) == false)
			MesgBox.Show(this, "디렉토리 만들기", "디렉토리를 만들 수 없어요", MessageBoxIcon.Error);

		UpdateFiles();
	}

	// 파일 삭제
	private void RemoveFiles()
	{
		if (_hfs == null)
			return;

		var items = ListFiles.SelectedItems;
		if (items.Count == 0)
			return;

		var box = new MesgBox()
		{
			Text = "파일 삭제",
			Message = $"{items.Count}개 파일, 진짜 삭제할거예요?",
			BoxIcon = MessageBoxIcon.Question,
			BoxButtons = MessageBoxButtons.YesNo,
		};
		foreach (ListViewItem item in items)
			box.AddItem(item.Text);
		if (box.ShowDialog() == DialogResult.No)
			return;

		List<string> failed = [];
		foreach (ListViewItem item in items)
		{
			if (item.Tag is not Hfs.Info file)
				continue;
			if (_hfs.Remove(file.name) == false)
				failed.Add(file.name);
		}
		if (failed.Count > 0)
			MesgBox.Show(this, "파일 삭제", $"{failed.Count}개 파일 삭제 실패", failed, MessageBoxIcon.Error);

		UpdateFiles();
	}

	// 파일 수행
	private void ExecFiles()
	{
		if (_hfs == null)
			return;

		var items = ListFiles.SelectedItems;
		if (items.Count == 0)
			return;

		if (items.Count == 1)
		{
			if (items[0].Tag is not Hfs.Info file)
				return;
			if (file.file.IsDirectory)
			{
				if (file.name == ".")
					return;

				_history.Add(_hfs.CurrentDirectory);
				_hfs.ChDir(file.name);
				UpdateFiles();
				return;
			}

			var ext = Path.GetExtension(file.name).ToLower();
			var temp = Path.GetTempFileName() + ext;

			var data = _hfs.Read(file.name, out var size);
			if (data == null || data.Length == 0)
				return;

			File.WriteAllBytes(temp, data);
			var ps = new System.Diagnostics.Process();
			ps.StartInfo.FileName = temp;
			ps.StartInfo.UseShellExecute = true;
			ps.StartInfo.Verb = "open";
			ps.StartInfo.CreateNoWindow = true;
			ps.EnableRaisingEvents = true;
			ps.Exited += (_, _) => ExecFileWait(temp);
			ps.Start();
		}
	}

	private void ExecFileWait(string temp)
	{
		File.Delete(temp);
	}

	// 풀기
	private void ExtractFiles()
	{
		if (_hfs == null)
			return;

		var items = ListFiles.SelectedItems;
		if (items.Count == 0)
			return;


		if (string.IsNullOrEmpty(_last_extract) == false)
		{
			var box = new MesgBox()
			{
				Text = "풀기 위치",
				Message = "이전에 풀었던 위치를 사용할까요?",
				BoxIcon = MessageBoxIcon.Question,
				BoxButtons = MessageBoxButtons.YesNoCancel,
			};
			box.AddItem(_last_extract);
			var res = box.ShowDialog();
			if (res == DialogResult.Cancel)
				return;
			if (res == DialogResult.No)
				_last_extract = string.Empty;
		}

		if (string.IsNullOrEmpty(_last_extract))
		{
			var dlg = new FolderBrowserDialog()
			{
				Description = "풀기 위치",
				ShowNewFolderButton = true,
				SelectedPath = _last_directory,
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			_last_extract = dlg.SelectedPath;
		}

		List<string> failed = [];
		List<string> success = [];
		foreach (ListViewItem item in items)
		{
			if (item.Tag is not Hfs.Info file)
				continue;
			if (file.file.IsDirectory)
				continue;

			var data = _hfs.Read(file.name, out var size);
			if (data == null || data.Length == 0)
			{
				failed.Add(file.name);
				continue;
			}

			string path = Path.Combine(_last_extract, file.name);
			File.WriteAllBytes(path, data);
			success.Add(file.name);
		}
		if (failed.Count > 0)
			MesgBox.Show(this, "파일 풀기", $"{failed.Count}개 파일 풀기 실패", failed, MessageBoxIcon.Error);
		MesgBox.Show(this, "파일 풀기", $"{success.Count}개 파일 풀기 완료", success, MessageBoxIcon.Information);
	}

	private void StoreFiles(string[] files)
	{
		if (_hfs == null)
			return;

		List<string> failed = [];
		List<string> success = [];
		foreach (var f in files)
		{
			if (_hfs.StoreFile(null, f) == false)
				failed.Add(f);
			else
				success.Add(f);
		}

		UpdateFiles();
		if (failed.Count > 0)
			MesgBox.ShowCenter("파일 저장", $"{failed.Count}개 파일 저장 실패", failed, MessageBoxIcon.Error);
		MesgBox.ShowCenter("파일 저장", $"{success.Count}개 파일 저장 완료", success, MessageBoxIcon.Information);
	}

	//
	private void OptimizeCallback(Hfs.OptimizeData data)
	{
		LabelInfo.Text = $"진행중... {data.FileName} ({data.Count} : {data.Stack}";
		Thread.Sleep(10);
	}

	//
	private void Optimize()
	{
		if (_hfs == null)
			return;

		var dlg = new OpenFileDialog()
		{
			Title = "HFS 최적화",
			Filter = "HFS 파일 (*.hfs)|*.hfs|모든 파일 (*.*)|*.*",
			FilterIndex = 1,
			RestoreDirectory = true,
			CheckFileExists = false,
			CheckPathExists = false,
			ShowReadOnly = false,
			ReadOnlyChecked = false,
			ShowHelp = false,
			InitialDirectory = _last_directory,
		};

		if (dlg.ShowDialog() != DialogResult.OK)
			return;

		LabelInfo.Text = "최적화 중...";
		LabelInfo.Visible = true;
		Enabled = false;
		_hfs.Optimize(dlg.FileName, OptimizeCallback);
		Enabled = true;
		LabelInfo.Visible = false;

		MesgBox.Show(this, "HFS 최적화", "최적화가 끝났어요!", MessageBoxIcon.Information);
	}
}
