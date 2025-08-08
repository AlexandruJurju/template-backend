using System.Data;
using Dapper;
using Template.Domain.Users;
using Template.SharedKernel.Application.CustomResult;
using Template.SharedKernel.Application.Messaging;
using Template.SharedKernel.Infrastructure.Data;

namespace Template.Application.Users.Queries.GetById;

internal sealed class GetUserByIdQueryHandler(
    ISqlConnectionFactory connectionFactory
) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();

        const string sql = """

                                       SELECT 
                                           Id as Id,
                                           first_name as FirstName,
                                           last_name as LastName,
                                           Email as Email
                                       FROM Users
                                       WHERE Id = @UserId;
                                   
                           """;

        dynamic? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(
            sql,
            new { query.UserId }
        );

        if (user is null)
        {
            return UserErrors.NotFound(query.UserId);
        }

        return user;
    }
}
