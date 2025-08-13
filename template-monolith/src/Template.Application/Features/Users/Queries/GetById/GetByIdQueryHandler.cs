using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetById;

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
