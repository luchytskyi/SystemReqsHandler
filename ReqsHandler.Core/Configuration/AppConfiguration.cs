namespace ReqsHandler.Core.Configuration;

public class AppConfiguration(string virtualEnv, string pythonInterpreter) : IAppConfiguration
{
	public string PythonVirtualEnvironment { get; set; } = virtualEnv;

	public string PythonInterpreter { get; set; } = pythonInterpreter;
}