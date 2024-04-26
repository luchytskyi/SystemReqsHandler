using ReqsHandler.Core.Services.Models;
using SystemReqsHandlerApi.Models;

namespace SystemReqsHandlerApi.Services;

public interface IReqsAnalyzer
{
	IEnumerable<TableDto> GetSchema();

	string Tokenize(string text);

	ReqsDiagramResponse GetDiagram(string text);
}