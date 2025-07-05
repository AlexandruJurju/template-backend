using Microsoft.EntityFrameworkCore;
using Template.Application.Abstractions.Messaging;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Abstractions.Result;
using Template.Domain.Users;

namespace Template.Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(
    IApplicationDbContext dbContext
) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async ValueTask<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .SingleOrDefaultAsync(user => user.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(query.UserId);
        }

        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}
