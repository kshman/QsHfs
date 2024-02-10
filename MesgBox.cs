using System.Security.Policy;

namespace QsHfs;

public partial class MesgBox : Form
{
	private MessageBoxIcon _icon;
	private MessageBoxButtons _buttons;


	public MesgBox()
	{
		InitializeComponent();
	}

	public MessageBoxIcon BoxIcon
	{
		get => _icon;
		set
		{
			_icon = value;
			switch (value)
			{
				case MessageBoxIcon.Error:
					PictIcon.Image = SystemIcons.Error.ToBitmap();
					break;
				case MessageBoxIcon.Information:
					PictIcon.Image = SystemIcons.Information.ToBitmap();
					break;
				case MessageBoxIcon.Warning:
					PictIcon.Image = SystemIcons.Warning.ToBitmap();
					break;
				case MessageBoxIcon.Question:
					PictIcon.Image = SystemIcons.Question.ToBitmap();
					break;
				default:
					PictIcon.Visible = false;
					return;
			}
			PictIcon.Visible = true;
		}
	}

	public MessageBoxButtons BoxButtons
	{
		get => _buttons;
		set
		{
			_buttons = value;
			switch (value)
			{
				case MessageBoxButtons.OK:
					BtnRight.Text = "확인";
					BtnRight.Visible = true;
					BtnRight.DialogResult = DialogResult.OK;
					AcceptButton = BtnRight;
					break;
				case MessageBoxButtons.OKCancel:
					BtnMiddle.Text = "확인";
					BtnMiddle.Visible = true;
					BtnMiddle.DialogResult = DialogResult.OK;
					AcceptButton = BtnMiddle;
					BtnRight.Text = "취소";
					BtnRight.Visible = true;
					BtnRight.DialogResult = DialogResult.Cancel;
					CancelButton = BtnRight;
					break;
				case MessageBoxButtons.YesNo:
					BtnMiddle.Text = "예";
					BtnMiddle.Visible = true;
					BtnMiddle.DialogResult = DialogResult.Yes;
					AcceptButton = BtnMiddle;
					BtnRight.Text = "아니오";
					BtnRight.Visible = true;
					BtnRight.DialogResult = DialogResult.No;
					CancelButton = BtnRight;
					break;
				case MessageBoxButtons.YesNoCancel:
					BtnLeft.Text = "예";
					BtnLeft.Visible = true;
					BtnLeft.DialogResult = DialogResult.Yes;
					AcceptButton = BtnRight;
					BtnMiddle.Text = "아니오";
					BtnMiddle.Visible = true;
					BtnMiddle.DialogResult = DialogResult.No;
					BtnRight.Text = "취소";
					BtnRight.Visible = true;
					BtnRight.DialogResult = DialogResult.Cancel;
					CancelButton = BtnRight;
					break;
				default:
					BtnRight.Text = "확인";
					BtnRight.Visible = true;
					BtnRight.DialogResult = DialogResult.OK;
					AcceptButton = BtnRight;
					break;
			}
		}
	}

	public string Message
	{
		get => LabelMesg.Text;
		set => LabelMesg.Text = value;
	}

	public void AddItem(string mesg)
	{
		ListItems.Items.Add(mesg);
	}

	public new DialogResult ShowDialog()
	{
		if (ListItems.Items.Count == 0)
		{
			ListItems.Visible = false;
			Height -= ListItems.Height;
		}

		base.ShowDialog();
		return DialogResult;
	}

	public DialogResult ShowDialog(Form parent)
	{
		if (ListItems.Items.Count == 0)
		{
			ListItems.Visible = false;
			Height -= ListItems.Height;
		}

		base.ShowDialog(parent);
		return DialogResult;
	}

	public static DialogResult Show(Form parent, string mesg, 
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		return box.ShowDialog();
	}

	public static DialogResult Show(Form parent, string title, string mesg,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Text = title, Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		return box.ShowDialog();
	}

	public static DialogResult Show(Form parent, string mesg, IList<string> items,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		foreach (var item in items)
			box.ListItems.Items.Add(item);
		return box.ShowDialog();
	}

	public static DialogResult Show(Form parent, string title, string mesg, IList<string> items,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Text = title, Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		foreach (var item in items)
			box.ListItems.Items.Add(item);
		return box.ShowDialog();
	}

	public static DialogResult ShowCenter(string mesg,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		box.StartPosition = FormStartPosition.CenterScreen;
		return box.ShowDialog();
	}

	public static DialogResult ShowCenter(string title, string mesg,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Text = title, Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		box.StartPosition = FormStartPosition.CenterScreen;
		return box.ShowDialog();
	}

	public static DialogResult ShowCenter(string mesg, IList<string> items,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		foreach (var item in items)
			box.ListItems.Items.Add(item);
		box.StartPosition = FormStartPosition.CenterScreen;
		return box.ShowDialog();
	}

	public static DialogResult ShowCenter(string title, string mesg, IList<string> items,
		MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
	{
		using var box = new MesgBox { Text = title, Message = mesg, BoxIcon = icon, BoxButtons = buttons };
		foreach (var item in items)
			box.ListItems.Items.Add(item);
		box.StartPosition = FormStartPosition.CenterScreen;
		return box.ShowDialog();
	}
}
