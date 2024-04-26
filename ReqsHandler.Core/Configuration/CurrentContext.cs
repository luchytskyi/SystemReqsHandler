namespace ReqsHandler.Core.Configuration;

public class CurrentContext(IClientSystemConfig clientSystemConfig) : ICurrentContext
{
	private DataSet? _dataSet;

	public DataSet DataSet => _dataSet ??= GetDefaultDateSet() ?? throw new ArgumentException($"{typeof(DataSet)} can be found.");

	public string CurrentLang { get; private set; } = clientSystemConfig.DefaultDataSet;

	private DataSet? GetDefaultDateSet()
	{
		return clientSystemConfig.DataSet.GetValueOrDefault(CurrentLang);
	}

	public void Set(string lang)
	{
		if (clientSystemConfig.DataSet.TryGetValue(lang, out var dataSet))
		{
			CurrentLang = lang;
			_dataSet = dataSet;
		}
	}
}