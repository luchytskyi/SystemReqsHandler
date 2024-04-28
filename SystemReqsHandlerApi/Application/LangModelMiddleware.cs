using Microsoft.IdentityModel.Tokens;
using ReqsHandler.Core.Configuration;

namespace SystemReqsHandlerApi.Application;

public class LangModelMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext ctx, ICurrentContext context, IClientSystemConfig systemConfig)
	{
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