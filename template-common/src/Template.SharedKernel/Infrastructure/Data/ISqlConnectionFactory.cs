using System.Data;

namespace Template.SharedKernel.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
