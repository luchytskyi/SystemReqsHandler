using ReqsHandler.Core.Configuration;
using SpaCyDotNet.api;

namespace ReqsHandler.Core.Services;

public class SpacyInstance : ISpacyInstance
{
	private static ICurrentContext _context = null!;
	private Lang? _lang;

	public SpacyInstance(ICurrentContext context)
	{
		_context = context;
	}

	public Lang LangDocument => _lang ??= Spacy.Load(_context.DataSet.LangModel);
}