namespace ReqsHandler.Core.Configuration;

public class DataSet
{
	public string LangModel { get; set; } = string.Empty;

	public string ConnectionDb { get; set; } = string.Empty;

	public string ColumnIdentifierPrefix { get; set; } = string.Empty;

	public string ColumnSplitRegex { get; set; } = string.Empty;
}