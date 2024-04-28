using Python.Runtime;
using ReqsHandler.Core.Configuration;
using SpacyDotNet;

namespace SystemReqsHandlerApi.Application;

public class PythonEnvironmentMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext ctx, IAppConfiguration configuration)
	{
		if (!PythonEngine.IsInitialized)
		{
			PythonRuntimeUtils.Init(configuration.PythonInterpreter, configuration.PythonVirtualEnvironment);
			PythonEngine.Initialize();
			PythonEngine.BeginAllowThreads();
		}

		await next.Invoke(ctx);
	}
}