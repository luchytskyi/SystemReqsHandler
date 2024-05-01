/* 
 * System Requirements Handler
 *
 * Version 1.0
 * Contact: luchitskyi@gmail.com
 * Source code: https://github.com/luchytskyi/SystemReqsHandler.git 
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ReqsHandler.Core.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using SystemReqsHandlerApi.Models;
using SystemReqsHandlerApi.Services;

namespace SystemReqsHandlerApi.Controllers;


[Route("reqs")]
public class ReqsHandlerController(IReqsAnalyzer analyzer, IClientSystemConfig config) : ControllerBase
{
	[SwaggerOperation(Summary = "Retrieves configured list of data schema.")]
	[HttpGet("config")]
	public ActionResult<IEnumerable<DataSetDto>> Get()
	{
		return Ok(config.DataSet.Keys.Select(key => new DataSetDto
		{
			Lang = key,
			Schema = GetSchemaName(key)
		}));
	}

	[SwaggerOperation(Summary = "The action takes text as input and uses an NLP model to process the text, dividing it into tokens. Each token is analyzed, and the method returns detailed information about each token, including its text, lemma, part of speech (POS), and detailed POS tag.")]
	[SwaggerResponse(200, "A list of `Token` objects, where each object contains information about the token: `text` (token text), `lemma` (token lemma), `pos` (part of speech), `tag` (detailed part of speech tag).", typeof(string[]))]
	[HttpGet("tokenize")]
	public ActionResult<string[]> Tokenize(string text)
	{
		return Ok(analyzer.Tokenize(text));
	}

	[SwaggerOperation(Summary = "Based on setting DefaultDataSet in appsettings returns decorated table schema with lemma for each entities (table, column)")]
	[SwaggerResponse(200, "A list of `Tables` objects, where each object contains information about the tables: `table` (keys, name, lemmas), `columns` (name, lemmas, data types).", typeof(List<TableDto>))]
	[HttpGet("schema")]
	public ActionResult<IEnumerable<TableDto>> GetSchema()
	{
		return Ok(analyzer.GetSchema());
	}

	[SwaggerOperation(Summary = "This action retrieves string UML for PlantUML service. It builds based on tokenize and decorate schema modules. ")]
	[SwaggerResponse(200,"To see the image based on generated UML code visit online PlantUML server and put it there `https://www.plantuml.com/plantuml/uml/`", typeof(string))]
	[HttpGet("uml")]
	public ActionResult<string> GetDiagramUml(string text)
	{
		return Ok(analyzer.GetDiagram(text).Uml);
	}

	[SwaggerOperation(Summary = "This action is used by the client application to get all data for the detailed visualization based on user input.")]
	[SwaggerResponse(200, "Returns the model with collected data from all modules: `tokenize`, `uml`, `schema` ", typeof(ReqsDiagramResponse))]
	[HttpGet("diagram/{text}")]
	public ActionResult<ReqsDiagramResponse> GetDiagram(string text)
	{
		return Ok(analyzer.GetDiagram(text));
	}

	private string GetSchemaName(string key)
	{
		if (!config.DataSet.TryGetValue(key, out var dataSet))
		{
			return string.Empty;
		}

		var connection = new SqlConnectionStringBuilder(dataSet.ConnectionDb);
		return (string)(connection.TryGetValue("Initial Catalog", out var schema) && schema is string
			? schema
			: string.Empty);
	}
}