using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using ReqsHandler.Core.Configuration;

namespace ReqsHandler.Core.Database;

public class ReqsDbProvider(ICurrentContext currentContext) : IReqsDbProvider
{
	public Microsoft.SqlServer.Management.Smo.Database? GetDbSchema()
	{
		using var connection = new SqlConnection(currentContext.DataSet.ConnectionDb);
		var serverConnection = new ServerConnection(connection);
		var server = new Server(serverConnection);

		return server.Databases.Contains(connection.Database) ? server.Databases[connection.Database] : null;
	}
}

