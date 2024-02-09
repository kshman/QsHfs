namespace QsHfs;

partial class MainForm
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
		TscMain = new ToolStripContainer();
		SplitMain = new SplitContainer();
		TreeDirectory = new TreeView();
		ListFiles = new ListView();
		ColumnName = new ColumnHeader();
		ColumnType = new ColumnHeader();
		ColumnDateTime = new ColumnHeader();
		ColumnSize = new ColumnHeader();
		ColumnCompr = new ColumnHeader();
		TsMain = new ToolStrip();
		TsBtnOpen = new ToolStripButton();
		TsBtnOptimize = new ToolStripButton();
		toolStripSeparator3 = new ToolStripSeparator();
		TsBtnMkDir = new ToolStripButton();
		TsBtnRemove = new ToolStripButton();
		TsBtnSubFile = new ToolStripDropDownButton();
		TsMiFileExport = new ToolStripMenuItem();
		TsMiFileRemove = new ToolStripMenuItem();
		TscMain.ContentPanel.SuspendLayout();
		TscMain.TopToolStripPanel.SuspendLayout();
		TscMain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)SplitMain).BeginInit();
		SplitMain.Panel1.SuspendLayout();
		SplitMain.Panel2.SuspendLayout();
		SplitMain.SuspendLayout();
		TsMain.SuspendLayout();
		SuspendLayout();
		// 
		// TscMain
		// 
		// 
		// TscMain.ContentPanel
		// 
		TscMain.ContentPanel.Controls.Add(SplitMain);
		TscMain.ContentPanel.Margin = new Padding(4);
		TscMain.ContentPanel.Size = new Size(884, 516);
		TscMain.Dock = DockStyle.Fill;
		TscMain.Location = new Point(0, 0);
		TscMain.Margin = new Padding(4);
		TscMain.Name = "TscMain";
		TscMain.Size = new Size(884, 561);
		TscMain.TabIndex = 0;
		TscMain.Text = "TscMain";
		// 
		// TscMain.TopToolStripPanel
		// 
		TscMain.TopToolStripPanel.Controls.Add(TsMain);
		// 
		// SplitMain
		// 
		SplitMain.Dock = DockStyle.Fill;
		SplitMain.FixedPanel = FixedPanel.Panel1;
		SplitMain.Location = new Point(0, 0);
		SplitMain.Margin = new Padding(4);
		SplitMain.Name = "SplitMain";
		// 
		// SplitMain.Panel1
		// 
		SplitMain.Panel1.Controls.Add(TreeDirectory);
		// 
		// SplitMain.Panel2
		// 
		SplitMain.Panel2.Controls.Add(ListFiles);
		SplitMain.Size = new Size(884, 516);
		SplitMain.SplitterDistance = 260;
		SplitMain.SplitterWidth = 5;
		SplitMain.TabIndex = 0;
		// 
		// TreeDirectory
		// 
		TreeDirectory.Dock = DockStyle.Fill;
		TreeDirectory.Location = new Point(0, 0);
		TreeDirectory.Margin = new Padding(4);
		TreeDirectory.Name = "TreeDirectory";
		TreeDirectory.Size = new Size(260, 516);
		TreeDirectory.TabIndex = 0;
		// 
		// ListFiles
		// 
		ListFiles.AllowColumnReorder = true;
		ListFiles.AllowDrop = true;
		ListFiles.AutoArrange = false;
		ListFiles.Columns.AddRange(new ColumnHeader[] { ColumnName, ColumnType, ColumnDateTime, ColumnSize, ColumnCompr });
		ListFiles.Dock = DockStyle.Fill;
		ListFiles.FullRowSelect = true;
		ListFiles.GridLines = true;
		ListFiles.Location = new Point(0, 0);
		ListFiles.Margin = new Padding(4);
		ListFiles.Name = "ListFiles";
		ListFiles.Size = new Size(619, 516);
		ListFiles.TabIndex = 0;
		ListFiles.UseCompatibleStateImageBehavior = false;
		ListFiles.View = View.Details;
		ListFiles.ColumnClick += ListFiles_ColumnClick;
		ListFiles.ItemSelectionChanged += ListFiles_ItemSelectionChanged;
		ListFiles.DragDrop += ListFiles_DragDrop;
		ListFiles.DragEnter += ListFiles_DragEnter;
		ListFiles.Layout += ListFiles_Layout;
		ListFiles.MouseDoubleClick += ListFiles_MouseDoubleClick;
		ListFiles.MouseUp += ListFiles_MouseUp;
		// 
		// ColumnName
		// 
		ColumnName.Text = "이름";
		// 
		// ColumnType
		// 
		ColumnType.Text = "형식";
		// 
		// ColumnDateTime
		// 
		ColumnDateTime.Text = "날짜시간";
		// 
		// ColumnSize
		// 
		ColumnSize.Text = "크기";
		// 
		// ColumnCompr
		// 
		ColumnCompr.Text = "압축";
		// 
		// TsMain
		// 
		TsMain.AutoSize = false;
		TsMain.Dock = DockStyle.None;
		TsMain.ImageScalingSize = new Size(32, 32);
		TsMain.Items.AddRange(new ToolStripItem[] { TsBtnOpen, TsBtnOptimize, toolStripSeparator3, TsBtnMkDir, TsBtnRemove, TsBtnSubFile });
		TsMain.Location = new Point(3, 0);
		TsMain.Name = "TsMain";
		TsMain.Size = new Size(416, 45);
		TsMain.TabIndex = 0;
		TsMain.Text = "TsMain";
		// 
		// TsBtnOpen
		// 
		TsBtnOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
		TsBtnOpen.Image = (Image)resources.GetObject("TsBtnOpen.Image");
		TsBtnOpen.ImageTransparentColor = Color.Magenta;
		TsBtnOpen.Name = "TsBtnOpen";
		TsBtnOpen.Size = new Size(36, 42);
		TsBtnOpen.Text = "toolStripButton2";
		TsBtnOpen.ToolTipText = "HFS 열기";
		TsBtnOpen.Click += TsBtnOpen_Click;
		// 
		// TsBtnOptimize
		// 
		TsBtnOptimize.DisplayStyle = ToolStripItemDisplayStyle.Image;
		TsBtnOptimize.Image = (Image)resources.GetObject("TsBtnOptimize.Image");
		TsBtnOptimize.ImageTransparentColor = Color.Magenta;
		TsBtnOptimize.Name = "TsBtnOptimize";
		TsBtnOptimize.Size = new Size(36, 42);
		TsBtnOptimize.Text = "toolStripButton3";
		TsBtnOptimize.ToolTipText = "최적화";
		TsBtnOptimize.Click += TsBtnOptimize_Click;
		// 
		// toolStripSeparator3
		// 
		toolStripSeparator3.Name = "toolStripSeparator3";
		toolStripSeparator3.Size = new Size(6, 45);
		// 
		// TsBtnMkDir
		// 
		TsBtnMkDir.DisplayStyle = ToolStripItemDisplayStyle.Image;
		TsBtnMkDir.Image = (Image)resources.GetObject("TsBtnMkDir.Image");
		TsBtnMkDir.ImageTransparentColor = Color.Magenta;
		TsBtnMkDir.Name = "TsBtnMkDir";
		TsBtnMkDir.Size = new Size(36, 42);
		TsBtnMkDir.Text = "toolStripButton1";
		TsBtnMkDir.ToolTipText = "디렉토리 만들기";
		TsBtnMkDir.Click += TsBtnMkDir_Click;
		// 
		// TsBtnRemove
		// 
		TsBtnRemove.DisplayStyle = ToolStripItemDisplayStyle.Image;
		TsBtnRemove.Image = (Image)resources.GetObject("TsBtnRemove.Image");
		TsBtnRemove.ImageTransparentColor = Color.Magenta;
		TsBtnRemove.Name = "TsBtnRemove";
		TsBtnRemove.Size = new Size(36, 42);
		TsBtnRemove.Text = "toolStripButton2";
		TsBtnRemove.ToolTipText = "파일 개체 삭제";
		TsBtnRemove.Click += TsBtnRemove_Click;
		// 
		// TsBtnSubFile
		// 
		TsBtnSubFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
		TsBtnSubFile.DropDownItems.AddRange(new ToolStripItem[] { TsMiFileExport, TsMiFileRemove });
		TsBtnSubFile.Image = (Image)resources.GetObject("TsBtnSubFile.Image");
		TsBtnSubFile.ImageTransparentColor = Color.Magenta;
		TsBtnSubFile.Name = "TsBtnSubFile";
		TsBtnSubFile.Size = new Size(45, 42);
		TsBtnSubFile.Text = "toolStripDropDownButton1";
		TsBtnSubFile.ToolTipText = "파일 메뉴";
		// 
		// TsMiFileExport
		// 
		TsMiFileExport.Name = "TsMiFileExport";
		TsMiFileExport.Size = new Size(180, 22);
		TsMiFileExport.Text = "파일 추출";
		TsMiFileExport.Click += TsMiFileExport_Click;
		// 
		// TsMiFileRemove
		// 
		TsMiFileRemove.Name = "TsMiFileRemove";
		TsMiFileRemove.Size = new Size(180, 22);
		TsMiFileRemove.Text = "파일 삭제";
		TsMiFileRemove.Click += TsMiFileRemove_Click;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(9F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(884, 561);
		Controls.Add(TscMain);
		Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
		Icon = (Icon)resources.GetObject("$this.Icon");
		Margin = new Padding(4);
		MinimumSize = new Size(640, 300);
		Name = "MainForm";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "QsHfs";
		FormClosing += MainForm_FormClosing;
		FormClosed += MainForm_FormClosed;
		Load += MainForm_Load;
		TscMain.ContentPanel.ResumeLayout(false);
		TscMain.TopToolStripPanel.ResumeLayout(false);
		TscMain.ResumeLayout(false);
		TscMain.PerformLayout();
		SplitMain.Panel1.ResumeLayout(false);
		SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)SplitMain).EndInit();
		SplitMain.ResumeLayout(false);
		TsMain.ResumeLayout(false);
		TsMain.PerformLayout();
		ResumeLayout(false);
	}

	#endregion

	private ToolStripContainer TscMain;
	private ToolStrip TsMain;
	private SplitContainer SplitMain;
	private TreeView TreeDirectory;
	private ListView ListFiles;
	private ColumnHeader ColumnName;
	private ColumnHeader ColumnType;
	private ColumnHeader ColumnDateTime;
	private ColumnHeader ColumnSize;
	private ColumnHeader ColumnCompr;
	private ToolStripButton TsBtnOpen;
	private ToolStripButton TsBtnOptimize;
	private ToolStripSeparator toolStripSeparator3;
	private ToolStripButton TsBtnMkDir;
	private ToolStripButton TsBtnRemove;
	private ToolStripDropDownButton TsBtnSubFile;
	private ToolStripMenuItem TsMiFileExport;
	private ToolStripMenuItem TsMiFileRemove;
}
