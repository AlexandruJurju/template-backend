using Microsoft.EntityFrameworkCore;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Users;
using Ardalis.Result;
using Template.SharedKernel.Application.Messaging;

namespace Template.Application.Users.Queries.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(
    IApplicationDbContext dbContext
) : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
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
