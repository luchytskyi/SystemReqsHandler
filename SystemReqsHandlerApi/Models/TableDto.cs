namespace SystemReqsHandlerApi.Models;

public class TableDto
{
	public string Name { get; init; } = string.Empty;

	public IEnumerable<string> Lemmas { get; init; } = Enumerable.Empty<string>();

	public IEnumerable<ForeignKeyDto> ForeignKeyDto { get; init; } = Enumerable.Empty<ForeignKeyDto>();

	public IEnumerable<ColumnDto> Columns { get; init; } = Enumerable.Empty<ColumnDto>();

	public bool IsSplitName { get; set; }

	public IEnumerable<string> SplitNames { get; set; } = Enumerable.Empty<string>();
}