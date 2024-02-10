namespace QsHfs.Hfs;

internal delegate void OptimizeCallback(OptimizeData data);

internal class OptimizeData
{
	public string? FileName { get; set; }
	public OptimizeCallback? OptimizeEvent;

	public int Count { get; set; }
	public int Stack { get; set; }
	public QsHfs.QnHfs? Input { get; set; }
	public QsHfs.QnHfs? Output { get; set; }
}
