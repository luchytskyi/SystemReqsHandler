namespace SystemReqsHandlerApi.Models
{
	public class ReqsDiagramResponse
	{
		public string RemoteUrl { get; set; } = string.Empty;

		public string Uml { get; set; } = string.Empty;

		public string Tokens { get; set; } = string.Empty;

		public IEnumerable<TableDto> DataSetSchema { get; set; } = Enumerable.Empty<TableDto>();
	}
}
