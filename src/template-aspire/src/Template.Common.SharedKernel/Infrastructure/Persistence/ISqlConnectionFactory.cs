using System.Data;

namespace Template.Common.SharedKernel.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
