namespace ReqsHandler.Core.Configuration;

public class AppConfiguration(string virtualEnv, string spacyLang, string remoteUmlServer) : IAppConfiguration
{
	public string PythonVirtualEnvironment { get; set; } = virtualEnv;

	public string SpacyLang { get; set; } = spacyLang;

	public string RemoteUmlServerUrl { get; set; } = remoteUmlServer;
}