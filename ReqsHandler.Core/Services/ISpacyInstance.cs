using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public interface ISpacyInstance
{
	public Lang LangDocument { get; }
}