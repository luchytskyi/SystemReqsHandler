using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ReqsHandler.Core.Configuration;
using SystemReqsHandlerApi.Models;
using SystemReqsHandlerApi.Services;

namespace SystemReqsHandlerApi.Controllers;

[Route("reqs")]
public class ReqsHandlerController(IReqsAnalyzer analyzer, IClientSystemConfig config) : ControllerBase
{
	[HttpGet("config")]
	public ActionResult<IEnumerable<DataSetDto>> Get()
	{
		return Ok(config.DataSet.Keys.Select(key => new DataSetDto
		{
			Lang = key,
			Schema = GetSchemaName(key)
		}));
	}

	private string GetSchemaName(string key)
	{
		if (config.DataSet.TryGetValue(key, out var dataSet))
		{
			var connection = new SqlConnectionStringBuilder(dataSet.ConnectionDb);
			return (string)(connection.TryGetValue("Initial Catalog", out var schema) && schema is string
				? schema
				: string.Empty);
		}

		return string.Empty;
	}

	[HttpGet("tokenize")]
	public ActionResult<string[]> Tokenize(string text)
	{
		return Ok(analyzer.Tokenize(text));
	}
	
	[HttpGet("schema")]
	public ActionResult<IEnumerable<TableDto>> GetSchema()
	{
		return Ok(analyzer.GetSchema());
	}

	[HttpGet("uml")]
	public ActionResult<string> GetDiagramUml(string text)
	{
		return Ok(analyzer.GetDiagram(text).Uml);
	}

    [HttpGet("diagram/{text}")]
    public ActionResult<ReqsDiagramResponse> GetDiagram(string text)
    {
        return Ok(analyzer.GetDiagram(text));
    }
}