using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(
    IApplicationDbContext dbContext
) : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async ValueTask<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Where(x => x.Email == query.Email)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(query.Email);
        }

        var response = new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        return response;
    }
}