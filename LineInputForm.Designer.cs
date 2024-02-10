namespace QsHfs;

partial class LineInputForm
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
		TextLine = new TextBox();
		BtnOk = new Button();
		BtnCancel = new Button();
		SuspendLayout();
		// 
		// TextLine
		// 
		TextLine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		TextLine.Location = new Point(12, 12);
		TextLine.Name = "TextLine";
		TextLine.Size = new Size(460, 27);
		TextLine.TabIndex = 0;
		// 
		// BtnOk
		// 
		BtnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		BtnOk.DialogResult = DialogResult.OK;
		BtnOk.Location = new Point(372, 45);
		BtnOk.Name = "BtnOk";
		BtnOk.Size = new Size(100, 36);
		BtnOk.TabIndex = 1;
		BtnOk.Text = "결정 (&O)";
		BtnOk.UseVisualStyleBackColor = true;
		// 
		// BtnCancel
		// 
		BtnCancel.DialogResult = DialogResult.Cancel;
		BtnCancel.Location = new Point(12, 45);
		BtnCancel.Name = "BtnCancel";
		BtnCancel.Size = new Size(100, 36);
		BtnCancel.TabIndex = 2;
		BtnCancel.Text = "취소 (&C)";
		BtnCancel.UseVisualStyleBackColor = true;
		// 
		// LineInputForm
		// 
		AcceptButton = BtnOk;
		AutoScaleDimensions = new SizeF(9F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		CancelButton = BtnCancel;
		ClientSize = new Size(484, 91);
		Controls.Add(BtnCancel);
		Controls.Add(BtnOk);
		Controls.Add(TextLine);
		Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
		Margin = new Padding(4);
		MaximizeBox = false;
		MaximumSize = new Size(10000, 130);
		MinimizeBox = false;
		MinimumSize = new Size(300, 130);
		Name = "LineInputForm";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		Text = "문장 입력";
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private TextBox TextLine;
	private Button BtnOk;
	private Button BtnCancel;
}