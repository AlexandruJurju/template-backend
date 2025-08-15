using System.Data;

namespace Template.SharedKernel.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
