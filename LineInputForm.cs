namespace QsHfs;

public partial class LineInputForm : Form
{
	public LineInputForm()
	{
		InitializeComponent();
	}

	public string LineText
	{
		get => TextLine.Text;
		set => TextLine.Text = value;
	}

	public bool EnableCancel
	{
		get => BtnCancel.Enabled;
		set => BtnCancel.Enabled = value;
	}
}
