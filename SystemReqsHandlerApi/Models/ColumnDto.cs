using Microsoft.SqlServer.Management.Smo;

namespace SystemReqsHandlerApi.Models;

public class ColumnDto
{
	public string Name { get; init; } = string.Empty;

	public IEnumerable<string> Lemmas { get; init; } = Enumerable.Empty<string>();

	public bool IsSplitName { get; set; }

	public IEnumerable<string> SplitNames { get; set; } = Enumerable.Empty<string>();

	public bool IsPrimaryKey { get; set; }

	public string DataType { get; set; }

	public SqlDataType SqlDataType { get; set; }

	public bool Nullable { get; set; }

	public int MaximumLength { get; set; }

	public int NumericPrecision { get; set; }

	public int NumericScale { get; set; }
}