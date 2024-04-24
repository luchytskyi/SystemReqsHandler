using ReqsHandler.Core.Database;
using ReqsHandler.Core.Services.Models;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public class ReqsService(IReqsDbProvider dbProvider, ISpacyInstance spacyInstance, IDbEntitiesLemmatizer lemmatizer) : IReqsService
{
	public Doc GetDocument(string text)
	{
		return spacyInstance.LangDocument.GetDocument(text);
	}

	public IEnumerable<ReqsTable> GetTables()
	{
		var database = dbProvider.GetDbSchema();
		return lemmatizer.MapTablesLemma(database?.Tables);
	}
}