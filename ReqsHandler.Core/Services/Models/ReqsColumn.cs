using Microsoft.SqlServer.Management.Smo;

namespace ReqsHandler.Core.Services.Models;

public class ReqsColumn : ReqsEntityBase
{
	public Column BaseEntity { get; set; } = new();
}