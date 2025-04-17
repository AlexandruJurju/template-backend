using System.Linq.Expressions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Users.Register;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests.Users;

public class RegisterUserTests
{
    private readonly RegisterUserCommandHandler _handler;
    private readonly IApplicationDbContext _dbContextMock;
    private readonly IPasswordHasher _passwordHasherMock;

    private readonly RegisterUserCommand _validCommand = new(
        "test@example.com",
        "John",
        "Doe",
        "Password123!");

    public RegisterUserTests()
    {
        var existingUsers = new List<User>
        {
            User.Create("test@example.com", "John", "Doe", "Password123!")
        };

        DbSet<User>? mockDbSet = existingUsers.AsQueryable().BuildMockDbSet();

        _dbContextMock = Substitute.For<IApplicationDbContext>();
        _dbContextMock.Users.Returns(mockDbSet);

        _passwordHasherMock = Substitute.For<IPasswordHasher>();

        _handler = new RegisterUserCommandHandler(_dbContextMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailIsUsed()
    {
        // Act
        Result<Guid> result = await _handler.Handle(_validCommand, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(UserErrors.EmailNotUnique);
    }
}
