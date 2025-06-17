using Application.Abstractions.Authentication;
using Application.Users.Login;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Moq.EntityFrameworkCore;

namespace Template.UnitTests;

[TestFixture]
public class LoginCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenProvider> _tokenProviderMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private FakeTimeProvider _timeProvider;
    private Mock<IApplicationDbContext> _dbContextMock;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider();
        _timeProvider.SetUtcNow(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        _dbContextMock = new Mock<IApplicationDbContext>();

        var user = User.Create("test@test.com", "John", "Doe", "PasswordHash");
        var users = new List<User> { user };

        _dbContextMock.Setup(x => x.Users).ReturnsDbSet(users);
    }

    [Test]
    public async Task HandleAsync_UserNotFound_Returns404NotFound()
    {
        // Arrange
        var command = new LoginUserCommand("test@example.com", "password");
        _dbContextMock.Setup(x => x.Users).ReturnsDbSet(new List<User>());
        var handler = new LoginUserCommandHandler(_passwordHasherMock.Object, _tokenProviderMock.Object, _configurationMock.Object, _timeProvider,
            _dbContextMock.Object);

        // Act
        Result<LoginResponse> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }
}
