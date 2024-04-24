using Microsoft.AspNetCore.Components;
using ReqsHandler.Core.Configuration;
using ReqsHandler.Core.Database;
using ReqsHandler.Core.Services;
using SystemReqsHandlerApi.Services;

namespace SystemReqsHandlerApi.Application;

public static class DependencyInjection
{
	public static WebApplicationBuilder InjectDependencies(this WebApplicationBuilder builder)
	{
		var collection = builder.Services;
		var configurationManager = builder.Configuration;

		collection.AddSingleton<IDataBaseConnection>(_ => configurationManager.GetDbConnection());
		collection.AddSingleton<IAppConfiguration>(_ => configurationManager.GetPythonConfiguration());
		collection.AddSingleton<IClientSystemConfig>(_ => configurationManager.GetClientSystemConfig());
		collection.AddSingleton<ISpacyInstance, SpacyInstance>();
		collection.AddTransient<IReqsDbProvider, ReqsDbProvider>();
		collection.AddTransient<IDbEntitiesLemmatizer, DbEntitiesLemmatizer>();
		collection.AddTransient<IReqsAnalyzer, ReqsAnalyze>();
		collection.AddTransient<IReqsService, ReqsService>();

		return builder;
	}
}