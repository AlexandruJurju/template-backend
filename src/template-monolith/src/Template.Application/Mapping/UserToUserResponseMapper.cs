using Template.Application.Features.Users;
using Template.Application.Features.Users.Dto;
using Template.Common.SharedKernel.Application.Mapper;
using Template.Domain.Entities.Users;

namespace Template.Application.Mapping;

public sealed class UserToUserResponseMapper : IMapper<User, UserResponse>
{
    public UserResponse Map(User source)
    {
        return new UserResponse
        {
            Id = source.Id,
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName
        };
    }

    public IReadOnlyList<UserResponse> Map(IReadOnlyList<User> sources)
    {
        return sources.Count == 0 ? [] : [.. sources.Select(Map)];
    }
}
