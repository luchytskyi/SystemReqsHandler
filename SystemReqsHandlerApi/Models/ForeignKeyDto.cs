namespace SystemReqsHandlerApi.Models;

public class ForeignKeyDto
{
	public string Name { get; set; } = string.Empty;

	public string ReferencedTable { get; set; } = string.Empty;

	public string ReferencedTableSchema { get; set; } = string.Empty;

	public IEnumerable<ForeignKeyColumnDto> Columns { get; set; } = Enumerable.Empty<ForeignKeyColumnDto>();
}