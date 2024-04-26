using Microsoft.IdentityModel.Tokens;
using ReqsHandler.Core.Configuration;

namespace SystemReqsHandlerApi.Application;

public static class ConfigurationManagerExtensions
{
	private const string ErrorMessage = "should be specified in appsettings.json";

	public static AppConfiguration GetPythonConfiguration(this ConfigurationManager configuration)
	{
		var settings = configuration?.GetSection("nlpSettings");
		if (settings == null)
		{
			throw new ArgumentException($"nlpSettings {ErrorMessage}");
		}

		var virtualEnv = settings["PythonVirtualEnvironment"];
		if (virtualEnv == null || virtualEnv.IsNullOrEmpty())
		{
			
			throw new ArgumentException($"PythonVirtualEnvironment {ErrorMessage}");
		}

		var interpreter = settings["PythonInterpreter"];
		if (interpreter == null || interpreter.IsNullOrEmpty())
		{
			throw new ArgumentException($"PythonInterpreter {ErrorMessage}");
		}

		return new AppConfiguration(virtualEnv, interpreter);
	}

	public static ClientSystemConfig GetClientSystemConfig(this ConfigurationManager configuration)
	{
		var clientSystemCfg = configuration.GetSection("ClientSystemConfig");
		if (clientSystemCfg == null)
		{
			throw new ArgumentException($"ClientSystemConfig  {ErrorMessage}");
		}

		var config = new ClientSystemConfig();
		clientSystemCfg.Bind(config);
		return config;
	}
}