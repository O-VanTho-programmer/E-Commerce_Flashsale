using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Auth.Commands.Login;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Tests.Common.Mocks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

    public LoginCommandHandlerTests()
    {
        _mockUow = MockUnitOfWork.GetMockUnitOfWork();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        _mockUow.Setup(u => u.Users).Returns(_mockUserRepo.Object);
    }

    [Fact]
    public async Task Handle_GivenValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "valid_password");
        var user = new User("test@test.com", "hashed_password", UserRole.Customer);
        
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("valid_password", "hashed_password")).Returns(true);
        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(user)).Returns("valid_jwt_token");

        var handler = new LoginCommandHandler(_mockUow.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("valid_jwt_token");
        _mockUserRepo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "wrong_password");
        var user = new User("test@test.com", "hashed_password", UserRole.Customer);
        
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("wrong_password", "hashed_password")).Returns(false);

        var handler = new LoginCommandHandler(_mockUow.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid email or password.");
        _mockJwtTokenGenerator.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_GivenNonExistentEmail_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand("notfound@test.com", "password");
        
        _mockUserRepo.Setup(r => r.GetByEmailAsync("notfound@test.com")).ReturnsAsync((User?)null);

        var handler = new LoginCommandHandler(_mockUow.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid email or password.");
        _mockPasswordHasher.Verify(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
