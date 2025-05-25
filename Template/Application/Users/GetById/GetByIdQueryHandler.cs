using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;

namespace Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository
) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async ValueTask<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);

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
