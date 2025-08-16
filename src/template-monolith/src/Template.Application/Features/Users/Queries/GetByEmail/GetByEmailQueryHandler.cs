using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Common.SharedKernel.Infrastructure.Dapper;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(
    ISqlConnectionFactory connectionFactory
) : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();

        const string sql = $"""
                            SELECT 
                                id as {nameof(UserResponse.Id)},
                                first_name as {nameof(UserResponse.FirstName)},
                                last_name as {nameof(UserResponse.LastName)},
                                email as {nameof(UserResponse.Email)}
                            FROM Users
                            WHERE email = @Email;
                            """;

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(
            sql,
            new { query.Email }
        );

        if (user is null)
        {
            return UserErrors.NotFound(query.Email);
        }

        return user;
    }
}
