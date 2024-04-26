namespace ReqsHandler.Core.Configuration;

public interface IClientSystemConfig
{
	public string RemoteUmlServerUrl { get; set; }

	public Dictionary<string, DataSet> DataSet { get; set; }

	string DefaultDataSet { get; set; }
}