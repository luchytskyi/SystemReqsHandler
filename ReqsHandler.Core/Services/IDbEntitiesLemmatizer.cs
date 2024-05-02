using Microsoft.SqlServer.Management.Smo;
using ReqsHandler.Core.Services.Models;

namespace ReqsHandler.Core.Services;

public interface IDbEntitiesLemmatizer
{
	public IEnumerable<ReqsTable> DecorateEntitiesWithLemma(TableCollection collection);
}