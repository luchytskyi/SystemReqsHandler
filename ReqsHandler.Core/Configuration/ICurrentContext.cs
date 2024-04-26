namespace ReqsHandler.Core.Configuration;

public interface ICurrentContext
{
	string CurrentLang { get; }

	DataSet DataSet { get; }

	public void Set(string lang);
}