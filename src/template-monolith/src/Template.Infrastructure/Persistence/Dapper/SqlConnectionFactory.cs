using System.Data;
using Npgsql;
using Template.Common.SharedKernel.Infrastructure;
using Template.Common.SharedKernel.Infrastructure.Dapper;

namespace Template.Infrastructure.Persistence.Dapper;

internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        return connection;
    }
}
