using BusinessLogic.Helpers.Crypto.Interfaces;
using BusinessLogic.Services;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Enums;
using Test.TestHelpers;

namespace Test.Unit.BusinessLogic.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly Mock<ICryptoHelper> _mockCryptoHelper = new();
    private readonly Mock<ISignatureHelper> _mockSignatureHelper = new();
    private readonly Mock<ILogger<AuthService>> _mockLogger = new();
    private readonly MemoryCache _realCache = new(new MemoryCacheOptions());

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService(_mockUserRepository.Object, _mockJwtService.Object, _mockCryptoHelper.Object,
            _mockSignatureHelper.Object, _realCache,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateChallenge_ExistingUsername_ReturnsSuccessOK()
    {
        //Arrange
        const string username = "existing-username";

        //Mock
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeTrue(
                "because if the username exists the signature salt is successfully retrieved and a challenge should be generated");
        serviceResult.ErrorMessage.Should().BeEmpty("because if IsSuccess is true there should be no error message");
        serviceResult.Data.Should()
            .BeOfType<ChallengeResponse>("because in case of success the service should return the challenge");
        serviceResult.SuccessType.Should().Be(ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task GenerateChallenge_InexistentUsername_ReturnsFailureNotFound()
    {
        //Arrange
        const string username = "inexistent-username";

        //Mock
        string? nullString = null;
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsername(It.IsAny<string>()))
            .ReturnsAsync(nullString); //Mocking failure in retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse(
                "because if the username doesn't exist the signature salt can't be retrieved and no challenge should be generated");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResultErrorType.NotFound);
    }

    [Fact]
    public async Task Login_ValidLogin_ReturnsSuccessOK()
    {
        //Arrange
        var loginRequest = new LoginRequest
        {
            Username = "username-with-challenge",
            NonceBase64 = TestDataProvider.GetValidBase64Value(),
            NonceSignatureBase64 = TestDataProvider.GetValidBase64Value()
        };

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));

        _mockUserRepository.Setup(r => r.GetPublicKeyByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful public key retrieval

        _mockSignatureHelper.Setup(s =>
                s.VerifyDetachedSignature(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true); //Mocking valid signature for the challenge
        
        _mockUserRepository.Setup(r => r.GetEncryptionSaltByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful encryption salt retrieval

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeTrue(
                "because if the login request is valid and the nonce is found in cache the login should succeed");
        serviceResult.ErrorMessage.Should().BeEmpty("because if IsSuccess is true there should be no error message");
        serviceResult.Data.Should()
            .BeOfType<LoginResponse>("because in case of success the service should return the login response");
        serviceResult.SuccessType.Should().Be(ServiceResultSuccessType.Ok);
    }
    
    [Fact]
    public async Task Login_InvalidSignature_ReturnsFailureUnauthorized()
    {
        //Arrange
        var loginRequest = new LoginRequest
        {
            Username = "username-with-challenge",
            NonceBase64 = TestDataProvider.GetValidBase64Value(),
            NonceSignatureBase64 = TestDataProvider.GetValidBase64Value()
        };

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));

        _mockUserRepository.Setup(r => r.GetPublicKeyByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful public key retrieval

        _mockSignatureHelper.Setup(s =>
                s.VerifyDetachedSignature(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(false); //Mocking invalid signature for the challenge

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse(
                "because if the signature is invalid the challenge should fail");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Login_NonceNotInCache_ReturnsFailureUnauthorized()
    {
        //Arrange
        var loginRequest = new LoginRequest
        {
            Username = "username-with-challenge",
            NonceBase64 = TestDataProvider.GetValidBase64Value(),
            NonceSignatureBase64 = TestDataProvider.GetValidBase64Value()
        };

        //Mock
        //Not setting the nonce in cache here means the service won't find it.
        //Attempt to retrieve nonce from cache should be the first thing done by the service. If it can't be found
        //The challenge should fail immediately, so nothing else needs to be mocked here

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse(
                "because if there is no nonce associated with the sent username the challenge should fail");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResultErrorType.Unauthorized);
    }
    
    [Fact]
    public async Task Login_RequestNonceNotEqualToCached_ReturnsFailureUnauthorized()
    {
        //Arrange
        var loginRequest = new LoginRequest
        {
            Username = "username-with-challenge",
            NonceBase64 = TestDataProvider.GetValidBase64Value(),
            NonceSignatureBase64 = TestDataProvider.GetValidBase64Value()
        };

        //Mock
        //Setting nonce in cache for successful retrieval during service execution, with value not equal to request nonce
        _realCache.Set($"nonceBase64:{loginRequest.Username}", TestDataProvider.GetRandomValidBase64(), TimeSpan.FromMinutes(2));
        //Checking that the request nonce is the same as the cached nonce should be the first thing done after retrieving
        //The nonce from cache. If they aren't equal the challenge should fail immediately, so nothing else needs to be mocked here

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse(
                "because if the request nonce is not equal to the cached nonce the challenge should fail");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResultErrorType.Unauthorized);
    }
    
    [Fact]
    public async Task Login_InvalidBase64RequestSignature_ReturnsFailureBadRequest()
    {
        //Arrange
        var loginRequest = new LoginRequest
        {
            Username = "username-with-challenge",
            NonceBase64 = TestDataProvider.GetValidBase64Value(),
            NonceSignatureBase64 = TestDataProvider.GetInvalidBase64Value()
        };

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));
        
        _mockUserRepository.Setup(r => r.GetPublicKeyByUsername(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful public key retrieval

        //Act
        var serviceResult = await _authService.Login(loginRequest);
        
        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse(
                "because if the request signatureBase64 is not in a valid base64 format the challenge can't be evaluated and should fail");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResultErrorType.BadRequest);
    }
}