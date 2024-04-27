using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public interface ISpacyInstance
{
	Doc GetDocument(string text);
}