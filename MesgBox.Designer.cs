namespace QsHfs;

partial class MesgBox
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
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
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		PictIcon = new PictureBox();
		LabelMesg = new Label();
		ListItems = new ListBox();
		BtnRight = new Button();
		BtnMiddle = new Button();
		BtnLeft = new Button();
		((System.ComponentModel.ISupportInitialize)PictIcon).BeginInit();
		SuspendLayout();
		// 
		// PictIcon
		// 
		PictIcon.Location = new Point(12, 12);
		PictIcon.Name = "PictIcon";
		PictIcon.Size = new Size(50, 50);
		PictIcon.TabIndex = 0;
		PictIcon.TabStop = false;
		PictIcon.Visible = false;
		// 
		// LabelMesg
		// 
		LabelMesg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		LabelMesg.ImageAlign = ContentAlignment.TopLeft;
		LabelMesg.Location = new Point(68, 12);
		LabelMesg.Name = "LabelMesg";
		LabelMesg.Size = new Size(404, 50);
		LabelMesg.TabIndex = 1;
		LabelMesg.Text = "알려드려요!";
		LabelMesg.TextAlign = ContentAlignment.MiddleLeft;
		// 
		// ListItems
		// 
		ListItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		ListItems.FormattingEnabled = true;
		ListItems.ItemHeight = 20;
		ListItems.Location = new Point(12, 68);
		ListItems.Name = "ListItems";
		ListItems.Size = new Size(460, 124);
		ListItems.TabIndex = 2;
		// 
		// BtnRight
		// 
		BtnRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		BtnRight.Location = new Point(402, 198);
		BtnRight.Name = "BtnRight";
		BtnRight.Size = new Size(70, 30);
		BtnRight.TabIndex = 3;
		BtnRight.Text = "button1";
		BtnRight.UseVisualStyleBackColor = true;
		BtnRight.Visible = false;
		// 
		// BtnMiddle
		// 
		BtnMiddle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		BtnMiddle.Location = new Point(326, 198);
		BtnMiddle.Name = "BtnMiddle";
		BtnMiddle.Size = new Size(70, 30);
		BtnMiddle.TabIndex = 4;
		BtnMiddle.Text = "button2";
		BtnMiddle.UseVisualStyleBackColor = true;
		BtnMiddle.Visible = false;
		// 
		// BtnLeft
		// 
		BtnLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		BtnLeft.Location = new Point(250, 198);
		BtnLeft.Name = "BtnLeft";
		BtnLeft.Size = new Size(70, 30);
		BtnLeft.TabIndex = 5;
		BtnLeft.Text = "button3";
		BtnLeft.UseVisualStyleBackColor = true;
		BtnLeft.Visible = false;
		// 
		// MesgBox
		// 
		AutoScaleDimensions = new SizeF(9F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(484, 236);
		Controls.Add(BtnLeft);
		Controls.Add(BtnMiddle);
		Controls.Add(BtnRight);
		Controls.Add(ListItems);
		Controls.Add(LabelMesg);
		Controls.Add(PictIcon);
		Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
		Margin = new Padding(4);
		MaximizeBox = false;
		MinimizeBox = false;
		MinimumSize = new Size(300, 150);
		Name = "MesgBox";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		Text = "알려드려요";
		((System.ComponentModel.ISupportInitialize)PictIcon).EndInit();
		ResumeLayout(false);
	}

	#endregion

	private PictureBox PictIcon;
	private Label LabelMesg;
	private ListBox ListItems;
	private Button BtnRight;
	private Button BtnMiddle;
	private Button BtnLeft;
}