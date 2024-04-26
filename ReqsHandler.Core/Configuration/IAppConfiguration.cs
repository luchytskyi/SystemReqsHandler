namespace ReqsHandler.Core.Configuration;

public interface IAppConfiguration
{
	public string PythonVirtualEnvironment { get; set; }

	public string PythonInterpreter { get; set; }
}