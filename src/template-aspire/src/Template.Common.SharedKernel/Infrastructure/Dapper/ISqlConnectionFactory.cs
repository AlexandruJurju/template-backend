using System.Data;

namespace Template.Common.SharedKernel.Infrastructure.Dapper;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
