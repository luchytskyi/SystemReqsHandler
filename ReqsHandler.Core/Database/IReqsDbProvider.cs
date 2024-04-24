namespace ReqsHandler.Core.Database;

public interface IReqsDbProvider
{
	Microsoft.SqlServer.Management.Smo.Database? GetDbSchema();
}