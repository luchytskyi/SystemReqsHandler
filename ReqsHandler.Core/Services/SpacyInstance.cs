using ReqsHandler.Core.Configuration;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public class SpacyInstance : ISpacyInstance
{
	private readonly Lazy<Lang> _lang = new(() => Spacy.Load(_configuration.SpacyLang));
	private static IAppConfiguration _configuration = null!;

	public SpacyInstance(IAppConfiguration configuration)
	{
		_configuration = configuration;
	}

	public Lang LangDocument => _lang.Value;
}