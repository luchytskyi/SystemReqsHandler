using Microsoft.SqlServer.Management.Smo;

namespace ReqsHandler.Core.Services.Models;

public class ReqsTable : ReqsEntityBase
{
	public Table BaseEntity { get; init; } = new();

	public IEnumerable<ReqsColumn> Columns { get; init; } = Enumerable.Empty<ReqsColumn>();
}