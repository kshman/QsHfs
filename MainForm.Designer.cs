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
		SplitMain = new SplitContainer();
		TreeDirectory = new TreeView();
		ListFiles = new ListView();
		ColumnName = new ColumnHeader();
		ColumnType = new ColumnHeader();
		ColumnDateTime = new ColumnHeader();
		ColumnSize = new ColumnHeader();
		ColumnCmpr = new ColumnHeader();
		TableMain = new TableLayoutPanel();
		PanelMenu = new Panel();
		BtnHfsOptimize = new Button();
		BtnExtract = new Button();
		BtnRemove = new Button();
		BtnMkDir = new Button();
		BtnOpenHfs = new Button();
		BtnNewHfs = new Button();
		((System.ComponentModel.ISupportInitialize)SplitMain).BeginInit();
		SplitMain.Panel1.SuspendLayout();
		SplitMain.Panel2.SuspendLayout();
		SplitMain.SuspendLayout();
		TableMain.SuspendLayout();
		PanelMenu.SuspendLayout();
		SuspendLayout();
		// 
		// SplitMain
		// 
		SplitMain.Dock = DockStyle.Fill;
		SplitMain.FixedPanel = FixedPanel.Panel1;
		SplitMain.Location = new Point(4, 49);
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
		SplitMain.Size = new Size(876, 516);
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
		ListFiles.Columns.AddRange(new ColumnHeader[] { ColumnName, ColumnType, ColumnDateTime, ColumnSize, ColumnCmpr });
		ListFiles.Dock = DockStyle.Fill;
		ListFiles.FullRowSelect = true;
		ListFiles.GridLines = true;
		ListFiles.Location = new Point(0, 0);
		ListFiles.Margin = new Padding(4);
		ListFiles.Name = "ListFiles";
		ListFiles.Size = new Size(611, 516);
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
		// ColumnCmpr
		// 
		ColumnCmpr.Text = "압축";
		// 
		// TableMain
		// 
		TableMain.ColumnCount = 1;
		TableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
		TableMain.Controls.Add(SplitMain, 0, 1);
		TableMain.Controls.Add(PanelMenu, 0, 0);
		TableMain.Dock = DockStyle.Fill;
		TableMain.Location = new Point(0, 0);
		TableMain.Name = "TableMain";
		TableMain.RowCount = 2;
		TableMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
		TableMain.RowStyles.Add(new RowStyle());
		TableMain.Size = new Size(884, 561);
		TableMain.TabIndex = 1;
		// 
		// PanelMenu
		// 
		PanelMenu.BackColor = SystemColors.ControlLightLight;
		PanelMenu.Controls.Add(BtnHfsOptimize);
		PanelMenu.Controls.Add(BtnExtract);
		PanelMenu.Controls.Add(BtnRemove);
		PanelMenu.Controls.Add(BtnMkDir);
		PanelMenu.Controls.Add(BtnOpenHfs);
		PanelMenu.Controls.Add(BtnNewHfs);
		PanelMenu.Dock = DockStyle.Fill;
		PanelMenu.Location = new Point(0, 0);
		PanelMenu.Margin = new Padding(0);
		PanelMenu.Name = "PanelMenu";
		PanelMenu.Size = new Size(884, 45);
		PanelMenu.TabIndex = 1;
		// 
		// BtnHfsOptimize
		// 
		BtnHfsOptimize.Enabled = false;
		BtnHfsOptimize.Image = (Image)resources.GetObject("BtnHfsOptimize.Image");
		BtnHfsOptimize.Location = new Point(255, 1);
		BtnHfsOptimize.Margin = new Padding(0);
		BtnHfsOptimize.Name = "BtnHfsOptimize";
		BtnHfsOptimize.Size = new Size(40, 40);
		BtnHfsOptimize.TabIndex = 5;
		BtnHfsOptimize.UseVisualStyleBackColor = true;
		BtnHfsOptimize.Click += BtnHfsOptimize_Click;
		// 
		// BtnExtract
		// 
		BtnExtract.Enabled = false;
		BtnExtract.Image = (Image)resources.GetObject("BtnExtract.Image");
		BtnExtract.Location = new Point(184, 1);
		BtnExtract.Margin = new Padding(0);
		BtnExtract.Name = "BtnExtract";
		BtnExtract.Size = new Size(40, 40);
		BtnExtract.TabIndex = 4;
		BtnExtract.UseVisualStyleBackColor = true;
		BtnExtract.Click += BtnExtract_Click;
		// 
		// BtnRemove
		// 
		BtnRemove.Enabled = false;
		BtnRemove.Image = (Image)resources.GetObject("BtnRemove.Image");
		BtnRemove.Location = new Point(144, 1);
		BtnRemove.Margin = new Padding(0);
		BtnRemove.Name = "BtnRemove";
		BtnRemove.Size = new Size(40, 40);
		BtnRemove.TabIndex = 3;
		BtnRemove.UseVisualStyleBackColor = true;
		BtnRemove.Click += BtnRemove_Click;
		// 
		// BtnMkDir
		// 
		BtnMkDir.Enabled = false;
		BtnMkDir.Image = (Image)resources.GetObject("BtnMkDir.Image");
		BtnMkDir.Location = new Point(104, 1);
		BtnMkDir.Margin = new Padding(0);
		BtnMkDir.Name = "BtnMkDir";
		BtnMkDir.Size = new Size(40, 40);
		BtnMkDir.TabIndex = 2;
		BtnMkDir.UseVisualStyleBackColor = true;
		BtnMkDir.Click += BtnMkDir_Click;
		// 
		// BtnOpenHfs
		// 
		BtnOpenHfs.Image = (Image)resources.GetObject("BtnOpenHfs.Image");
		BtnOpenHfs.Location = new Point(40, 1);
		BtnOpenHfs.Margin = new Padding(0);
		BtnOpenHfs.Name = "BtnOpenHfs";
		BtnOpenHfs.Size = new Size(40, 40);
		BtnOpenHfs.TabIndex = 1;
		BtnOpenHfs.UseVisualStyleBackColor = true;
		BtnOpenHfs.Click += BtnOpenHfs_Click;
		// 
		// BtnNewHfs
		// 
		BtnNewHfs.Image = (Image)resources.GetObject("BtnNewHfs.Image");
		BtnNewHfs.Location = new Point(4, 3);
		BtnNewHfs.Margin = new Padding(0);
		BtnNewHfs.Name = "BtnNewHfs";
		BtnNewHfs.Size = new Size(36, 36);
		BtnNewHfs.TabIndex = 0;
		BtnNewHfs.UseVisualStyleBackColor = true;
		BtnNewHfs.Click += BtnNewHfs_Click;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(9F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(884, 561);
		Controls.Add(TableMain);
		DoubleBuffered = true;
		Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
		Icon = (Icon)resources.GetObject("$this.Icon");
		KeyPreview = true;
		Margin = new Padding(4);
		MinimumSize = new Size(640, 300);
		Name = "MainForm";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "QsHfs";
		FormClosing += MainForm_FormClosing;
		FormClosed += MainForm_FormClosed;
		Load += MainForm_Load;
		KeyDown += MainForm_KeyDown;
		SplitMain.Panel1.ResumeLayout(false);
		SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)SplitMain).EndInit();
		SplitMain.ResumeLayout(false);
		TableMain.ResumeLayout(false);
		PanelMenu.ResumeLayout(false);
		ResumeLayout(false);
	}

	#endregion
	private SplitContainer SplitMain;
	private TreeView TreeDirectory;
	private ListView ListFiles;
	private ColumnHeader ColumnName;
	private ColumnHeader ColumnType;
	private ColumnHeader ColumnDateTime;
	private ColumnHeader ColumnSize;
	private ColumnHeader ColumnCmpr;
	private TableLayoutPanel TableMain;
	private Panel PanelMenu;
	private Button BtnNewHfs;
	private Button BtnHfsOptimize;
	private Button BtnExtract;
	private Button BtnRemove;
	private Button BtnMkDir;
	private Button BtnOpenHfs;
}
