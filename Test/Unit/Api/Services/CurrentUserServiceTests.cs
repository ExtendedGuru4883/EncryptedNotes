using System.Security.Claims;
using Core.Abstractions.Infrastructure;
using EncryptedNotes.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Test.Unit.Api.Services;

[Trait("Category", "Unit")]
public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

    private readonly ICurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _currentUserService = new CurrentUserService(_mockHttpContextAccessor.Object);
    }

    [Fact]
    public void UsernameUserId_ValidUser_ReturnUsernameUserId()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();
        const string username = "test-username";

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId),
            new (ClaimTypes.Name, username),
        };

        //Mock
        //Mocking an HttpContext with valid user data
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims))
        };
        _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

        //Act
        var currentUsername = _currentUserService.Username;
        var currentUserId = _currentUserService.UserId;

        //Assert
        currentUsername.Should().Be(username,
            "because if the HttpContext has a valid user the implementation of ICurrentUserService should return its username");
        currentUserId.Should().Be(userId,
            "because if the HttpContext has a valid user the implementation of ICurrentUserService should return its userId");
    }

    [Fact]
    public void UsernameUserId_InvalidUser_ReturnNull()
    {
        //Mock
        //Mocking an HttpContext without user data
        var httpContext = new DefaultHttpContext();
        _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

        //Act
        var currentUsername = _currentUserService.Username;
        var currentUserId = _currentUserService.UserId;

        //Assert
        currentUsername.Should()
            .BeNull(
                "because if the HttpContext doesn't have a valid user the implementation of ICurrentUserService should return null for the username");
        currentUserId.Should()
            .BeNull(
                "because if the HttpContext doesn't have a valid user the implementation of ICurrentUserService should return null for the userId");
    }
}