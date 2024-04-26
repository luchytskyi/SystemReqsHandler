using Microsoft.IdentityModel.Tokens;
using Python.Runtime;
using ReqsHandler.Core.Configuration;
using SpacyDotNet;

namespace SystemReqsHandlerApi.Application;

public class PythonEnvironmentMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext ctx, IAppConfiguration configuration, ICurrentContext context, IClientSystemConfig systemConfig)
	{
		if (!PythonEngine.IsInitialized)
		{
			PythonRuntimeUtils.Init(configuration.PythonInterpreter, configuration.PythonVirtualEnvironment);
			PythonEngine.Initialize();
			PythonEngine.BeginAllowThreads();
		}

		if (ctx.Request.Cookies["lang"] is { } lang && !lang.IsNullOrEmpty() && !context.CurrentLang.Equals(lang))
		{
			context.Set(lang);
		}
		else
		{
			ctx.Response.Cookies.Append("lang", systemConfig.DefaultDataSet);
		}

		await next.Invoke(ctx);
	}
}