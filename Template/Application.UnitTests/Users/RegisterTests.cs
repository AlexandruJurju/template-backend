using Application.Abstractions.Authentication;
using Application.Users.Register;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Shouldly;

namespace Application.UnitTests.Users;

public class RegisterUserTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserTests()
    {
        _handler = new RegisterUserCommandHandler(
            _userRepository,
            _unitOfWork,
            _passwordHasher
        );
    }


    [Fact]
    public async Task Handle_WhenEmailIsInUse_ShouldReturnError()
    {
        // Arrange
        string email = "test@example.com";
        var command = new RegisterUserCommand(
            email,
            "John",
            "Doe",
            "password");

        _userRepository.UserWithEmailExists(email, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.EmailNotUnique);
    }

    [Fact]
    public async Task Handle_WhenEmailIsUnique_ShouldCreateUser()
    {
        // Arrange
        string email = "test@example.com";
        string firstName = "John";
        string lastName = "Doe";
        string password = "password";
        string hashedPassword = "hashed_password";

        var command = new RegisterUserCommand(
            email,
            firstName,
            lastName,
            password);

        _userRepository.UserWithEmailExists(email, Arg.Any<CancellationToken>())
            .Returns(false);

        _passwordHasher.Hash(password).Returns(hashedPassword);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _userRepository.Received(1)
            .Add(Arg.Is<User>(u =>
                u.Email == email &&
                u.FirstName == firstName &&
                u.LastName == lastName &&
                u.PasswordHash == hashedPassword));

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
