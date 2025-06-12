using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;

namespace Application.Users.GetAll;

public class GetAllUsersQueryHandler(
    IUserRepository userRepository
)
    : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async ValueTask<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await userRepository.GetAllAsync(cancellationToken);

        var userResponses = users.Select(u => new UserResponse
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        }).ToList();

        return userResponses;
    }
}
