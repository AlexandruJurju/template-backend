using Template.Application.Features.Users.Queries;
using Template.Domain.Entities.Users;

namespace Template.Application.Mappers;

public static class UserMapper
{
    public static UserResponse Map(this User source)
    {
        return new UserResponse
        {
            Id = source.Id,
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName
        };
    }

    public static IEnumerable<UserResponse> Map(this IEnumerable<User> source)
    {
        return source.Select(Map);
    }
}
