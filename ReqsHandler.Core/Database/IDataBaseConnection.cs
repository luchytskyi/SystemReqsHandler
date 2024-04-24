namespace ReqsHandler.Core.Database;

public class DataBaseConnection(string connectionStr) : IDataBaseConnection
{
	public string DefaultConnection { get; set; } = connectionStr;
}

public interface IDataBaseConnection
{
	public string DefaultConnection { get; set; }
}