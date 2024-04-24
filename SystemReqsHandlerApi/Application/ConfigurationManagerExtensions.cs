using Microsoft.IdentityModel.Tokens;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Database;
using ReqsHandler.Core.Services;

namespace SystemReqsHandlerApi.Application;

public static class ConfigurationManagerExtensions
{
	public static DataBaseConnection GetDbConnection(this ConfigurationManager configurationManager)
	{
		var dbConnection = configurationManager.GetConnectionString("DefaultConnection");
		if (dbConnection == null || dbConnection.IsNullOrEmpty())
		{
			throw new ArgumentException("DefaultConnection should be specified in appsettings.json");
		}

		return new DataBaseConnection(dbConnection);
	}

	public static AppConfiguration GetPythonConfiguration(this ConfigurationManager configuration)
	{
		var settings = configuration?.GetSection("nlpSettings");
		if (settings == null)
		{
			throw new ArgumentException("nlpSettings should be configured in appsettings.json");
		}

		var langModel = settings["LangModel"];
		if (langModel == null || langModel.IsNullOrEmpty())
		{
			throw new ArgumentException("LangModel should be specified in appsettings.json");
		}

		var virtualEnv = settings["PythonVirtualEnvironment"];
		if (virtualEnv == null || virtualEnv.IsNullOrEmpty())
		{
			throw new ArgumentException("PythonVirtualEnvironment should be specified in appsettings.json");
		}

		var remoteUmlServer = settings["RemoteUmlServerUrl"];
		if (remoteUmlServer == null || remoteUmlServer.IsNullOrEmpty())
		{
			throw new ArgumentException("RemoteUmlServerUrl should be specified in appsettings.json");
		}

		return new AppConfiguration(virtualEnv, langModel, remoteUmlServer);
	}

	public static ClientSystemConfig GetClientSystemConfig(this ConfigurationManager configuration)
	{
		var columnPrefix = configuration?.GetSection("clientSystemCfg")["ColumnIdentifierPrefix"];
		if (columnPrefix == null || columnPrefix.IsNullOrEmpty())
		{
			throw new ArgumentException("ColumnIdentifierPrefix should be specified in appsettings.json");
		}

		return new ClientSystemConfig
		{
			ColumnIdentifierPrefix = columnPrefix
		};
	}
}