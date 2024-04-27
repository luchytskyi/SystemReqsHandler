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

		var lang = ctx.Request.Cookies["lang"];
		if (lang is null)
		{
			ctx.Response.Cookies.Append("lang", systemConfig.DefaultDataSet);
		}
		else if (!lang.IsNullOrEmpty() && !context.CurrentLang.Equals(lang))
		{
			context.Set(lang);
		}

		await next.Invoke(ctx);
	}
}