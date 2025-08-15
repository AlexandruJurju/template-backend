using Template.Application.Features.Users;
using Template.Domain.Entities.Users;

namespace Template.Application.Mapping;

public static class Mapper
{
    public static UserResponse Map(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}
