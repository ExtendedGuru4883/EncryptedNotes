using BusinessLogic.Services;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto.Responses;
using Shared.Enums;
using Test.TestHelpers;

namespace Test.Unit.BusinessLogic.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly Mock<ILogger<AuthService>> _mockLogger = new();
    private readonly MemoryCache _realCache = new(new MemoryCacheOptions());

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService(_mockUserRepository.Object, _mockJwtService.Object, _realCache,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateChallenge_ExistingUsername_ReturnsSuccessOK()
    {
        //Arrange
        const string username = "existing-username";

        //Mock
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetBase64Value(true)); //Mocking successful retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeTrue(
                "because if the username exists the signature salt is successfully retrieved and a challenge should be generated");
        serviceResult.ErrorMessage.Should().BeEmpty("because if IsSuccess is true there should be no error message");
        serviceResult.Data.Should()
            .BeOfType<ChallengeResponse>("because in case of success the service should return the challenge");
        serviceResult.SuccessType.Should().Be(ServiceResponseSuccessType.Ok);
    }

    [Fact]
    public async Task GenerateChallenge_InexistentUsername_ReturnsSuccessOK()
    {
        //Arrange
        const string username = "inexistent-username";

        //Mock
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsername(It.IsAny<string>()))
            .ReturnsAsync(string.Empty); //Mocking failure in retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse("because if the username doesn't exist the signature salt can't be retrieved and no challenge should be generated");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResponseErrorType.NotFound);
    }
}