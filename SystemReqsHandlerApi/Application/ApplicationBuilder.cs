namespace SystemReqsHandlerApi.Application;

public static class ApplicationBuilderUseExtensions
{
	public static IApplicationBuilder UsePythonEnvironment(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<PythonEnvironmentMiddleware>();
	}
	
	public static IApplicationBuilder UseLangModel(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<LangModelMiddleware>();
	}
}