namespace ReqsHandler.Core.Configuration;

public class ClientSystemConfig : IClientSystemConfig
{
	public string RemoteUmlServerUrl { get; set; } = string.Empty;

	public Dictionary<string, DataSet> DataSet { get; set; } = new();

	public string DefaultDataSet { get; set; } = string.Empty;
}