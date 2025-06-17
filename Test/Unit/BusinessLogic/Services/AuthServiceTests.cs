using System.Runtime.InteropServices;
using BusinessLogic.Helpers.Crypto.Interfaces;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Enums;
using Test.TestHelpers;

namespace Test.Unit.BusinessLogic.Services;

[Trait("Category", "Unit")]
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
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetValidBase64Value()); //Mocking successful retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task GenerateChallenge_InexistentUsername_ReturnsFailureNotFound()
    {
        //Arrange
        const string username = "inexistent-username";

        //Mock
        string? nullString = null;
        _mockUserRepository.Setup(r => r.GetSignatureSaltByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(nullString); //Mocking failure in retrieval of signature salt

        //Act
        var serviceResult = await _authService.GenerateChallenge(username);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }

    [Fact]
    public async Task Login_ValidLogin_ReturnsSuccessOK()
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));

        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length);

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetUserEntity); //Mocking successful user retrieval

        _mockSignatureHelper.Setup(s =>
                s.VerifyDetachedSignature(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true); //Mocking valid signature for the challenge

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task Login_UserNotFound_ReturnsFailureNotFound()
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));

        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length);

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(null as UserEntity); //Mocking unsuccessful user retrieval

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }

    [Fact]
    public async Task Login_InvalidSignature_ReturnsFailureUnauthorized()
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock
        //Setting nonce in cache for successful retrieval during service execution
        _realCache.Set($"nonceBase64:{loginRequest.Username}", loginRequest.NonceBase64, TimeSpan.FromMinutes(2));

        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length);

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(TestDataProvider.GetUserEntity); //Mocking successful user retrieval

        _mockSignatureHelper.Setup(s =>
                s.VerifyDetachedSignature(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(false); //Mocking invalid signature for the challenge

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Theory]
    [InlineData(false, null)]
    [InlineData(true, null)]
    [InlineData(true, "")]
    public async Task Login_MissingOrInvalidCachedNonce_ReturnsFailureUnauthorized(bool cacheHasValue, string? cachedNonce)
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock
        if (cacheHasValue) _realCache.Set($"nonceBase64:{loginRequest.Username}", cachedNonce, TimeSpan.FromMinutes(2));

        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length);

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Login_RequestNonceNotEqualToCached_ReturnsFailureUnauthorized()
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock

        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length);

        //Setting nonce in cache for successful retrieval during service execution, with value not equal to request nonce
        _realCache.Set($"nonceBase64:{loginRequest.Username}", TestDataProvider.GetRandomValidBase64(),
            TimeSpan.FromMinutes(2));

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Login_InvalidSignatureLength_ReturnsFailureBadRequest()
    {
        //Arrange
        var loginRequest = TestDataProvider.GetValidLoginRequest();

        //Mock
        _mockSignatureHelper.Setup(s => s.SignatureBase64Length)
            .Returns(loginRequest.NonceSignatureBase64.Length + 1);

        //Act
        var serviceResult = await _authService.Login(loginRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.BadRequest);
    }
}