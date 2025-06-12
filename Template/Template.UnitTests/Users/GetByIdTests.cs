using Application.Users.GetById;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using NSubstitute;
using Shouldly;

namespace Template.UnitTests.Users;

public class GetByIdTests
{
    private readonly Guid _testUserId = Guid.NewGuid();
    private GetUserByIdQueryHandler _handler;
    private IUserRepository _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_userRepositoryMock);
    }

    [Test]
    public async Task Handle_Should_ReturnFailure_When_UserIsNull()
    {
        // Arrange
        var query = new GetUserByIdQuery(_testUserId);

        _userRepositoryMock
            .GetByIdAsync(query.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        Result<UserResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NotFound(query.UserId));
    }

    [Test]
    public async Task Handle_Should_ReturnSuccess_When_UserExists()
    {
        // Arrange
        var query = new GetUserByIdQuery(_testUserId);
        var user = User.Create(
            "john.doe@test.com",
            "John",
            "Doe",
            "password123");

        _userRepositoryMock
            .GetByIdAsync(query.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result<UserResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(user.Id);
        result.Value.FirstName.ShouldBe(user.FirstName);
        result.Value.LastName.ShouldBe(user.LastName);
        result.Value.Email.ShouldBe(user.Email);

        await _userRepositoryMock.Received(1)
            .GetByIdAsync(query.UserId, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_Should_CallRepositoryWithCorrectParameters()
    {
        // Arrange
        var query = new GetUserByIdQuery(_testUserId);
        _userRepositoryMock
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _userRepositoryMock.Received(1)
            .GetByIdAsync(query.UserId, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_Should_ReturnCorrectResponseStructure_When_UserExists()
    {
        // Arrange
        var query = new GetUserByIdQuery(_testUserId);
        var user = User.Create(
            "john.doe@test.com",
            "John",
            "Doe",
            "password123");

        _userRepositoryMock
            .GetByIdAsync(query.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result<UserResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeOfType<UserResponse>();
        result.Value.Id.ShouldBe(user.Id);
        result.Value.FirstName.ShouldBe("John");
        result.Value.LastName.ShouldBe("Doe");
        result.Value.Email.ShouldBe(user.Email);
    }
}
