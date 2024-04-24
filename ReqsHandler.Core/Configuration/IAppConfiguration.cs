namespace ReqsHandler.Core.Configuration;

public interface IAppConfiguration
{
	public string PythonVirtualEnvironment { get; set; }

	public string SpacyLang { get; set; }

	public string RemoteUmlServerUrl { get; set; }
}