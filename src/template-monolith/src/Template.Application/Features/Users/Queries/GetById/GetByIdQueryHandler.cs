using Template.Application.Features.Users.Dto;
using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Common.SharedKernel.Infrastructure;
using Template.Common.SharedKernel.Infrastructure.Persistence.Dapper;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetById;

internal sealed class GetUserByIdQueryHandler(
    ISqlConnectionFactory connectionFactory
) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();

        const string sql = $"""
                            SELECT 
                                id as {nameof(UserResponse.Id)},
                                first_name as {nameof(UserResponse.FirstName)},
                                last_name as {nameof(UserResponse.LastName)},
                                email as {nameof(UserResponse.Email)}
                            FROM Users
                            WHERE id = @UserId;
                            """;

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(
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
