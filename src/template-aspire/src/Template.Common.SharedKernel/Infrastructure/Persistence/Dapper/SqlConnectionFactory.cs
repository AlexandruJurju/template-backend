using System.Data;
using Npgsql;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Dapper;

internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        return connection;
    }
}
