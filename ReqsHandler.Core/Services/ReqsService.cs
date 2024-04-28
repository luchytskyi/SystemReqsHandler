using ReqsHandler.Core.Database;
using ReqsHandler.Core.Services.Models;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public class ReqsService(IReqsDbProvider dbProvider, ISpacyInstance spacyInstance, IDbEntitiesLemmatizer lemmatizer) : IReqsService
{
	public Doc GetDocument(string text)
	{
		return spacyInstance.GetDocument(text);
	}

	public IEnumerable<ReqsTable> GetDecoratedTables()
	{
		var table = dbProvider.GetDbSchema()?.Tables;
		if (table == null || table.Count == 0)
		{
			return Enumerable.Empty<ReqsTable>();
		}
		
		return lemmatizer.DecorateEntitiesWithLemma(table);
	}
}