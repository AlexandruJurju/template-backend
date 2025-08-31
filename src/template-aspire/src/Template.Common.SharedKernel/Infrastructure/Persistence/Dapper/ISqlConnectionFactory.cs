using System.Data;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Dapper;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
