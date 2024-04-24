namespace SystemReqsHandlerApi.Models;

public class ForeignKeyColumnDto
{
	public string Name { get; set; } = string.Empty;

	public string ReferencedColumn { get; set; } = string.Empty;
}