using ReqsHandler.Core.Services.Models;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public interface IReqsService
{
	public Doc GetDocument(string text);

	public IEnumerable<ReqsTable> GetTables();
}