using Microsoft.AspNetCore.Mvc;
using SystemReqsHandlerApi.Models;
using SystemReqsHandlerApi.Services;

namespace SystemReqsHandlerApi.Controllers;

[Route("reqs")]
public class ReqsHandlerController(IReqsAnalyzer analyzer) : ControllerBase
{
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

	[HttpGet("analyze")]
	public ActionResult<string> Analyze(string text)
	{
		return Ok(analyzer.BuildUml(text));
	}

    [HttpGet("diagram/{text}")]
    public ActionResult<ReqsDiagramResponse> GetDiagram(string text)
    {
        return Ok(analyzer.GetDiagram(text));
    }
}