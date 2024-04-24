using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace ReqsHandler.Core.Database;

public class ReqsDbProvider(IDataBaseConnection connectionCfg) : IReqsDbProvider
{
	public Microsoft.SqlServer.Management.Smo.Database? GetDbSchema()
	{
		using var connection = new SqlConnection(connectionCfg?.DefaultConnection);
		var serverConnection = new ServerConnection(connection);
		var server = new Server(serverConnection);
		var database = server.Databases[connection.Database];

		return server.Databases.Contains(connection.Database) ? server.Databases[connection.Database] : null;
	}
}

