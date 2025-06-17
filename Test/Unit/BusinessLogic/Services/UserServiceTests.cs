using AutoMapper;
using BusinessLogic.Helpers.Crypto.Interfaces;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Test.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Enums;

namespace Test.Unit.BusinessLogic.Services;

[Trait("Category", "Unit")]
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ISignatureHelper> _mockSignatureHelper = new();
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ILogger<UserService>> _mockLogger = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_mockUserRepository.Object, _mockSignatureHelper.Object,
            _mockCurrentUserService.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ValidUser_ReturnsSuccessCreated()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockSignatureHelper.Setup(s => s.PublicKeyBase64Length)
            .Returns(userDto.PublicKeyBase64.Length);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Created);
    }

    [Fact]
    public async Task AddAsync_UsernameExists_ReturnsFailureConflict()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockSignatureHelper.Setup(s => s.PublicKeyBase64Length)
            .Returns(userDto.PublicKeyBase64.Length);
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true); //Mocking username already existing

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Conflict);
    }

    [Fact]
    public async Task AddAsync_InvalidPublicKeyLength_ReturnsFailureBadRequest()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockSignatureHelper.Setup(s => s.PublicKeyBase64Length)
            .Returns(userDto.PublicKeyBase64.Length + 1);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.BadRequest);
    }

    [Fact]
    public async Task DeleteCurrentUserAsync_AuthAndDeleted_ReturnsSuccessNoContent()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid().ToString);
        _mockUserRepository.Setup(n => n.DeleteByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        //Act
        var serviceResult = await _userService.DeleteCurrentAsync();

        //Assert
        CommonAssertions.AssertServiceResultSuccessNoContent(serviceResult);
    }
    
    [Fact]
    public async Task DeleteCurrentUserAsync_AuthAndNotDeleted_ReturnsFailureNotFound()
    {
        //This can happen if the user has been deleted and a new delete request is received with the old still valid token
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid().ToString);
        _mockUserRepository.Setup(n => n.DeleteByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false); //Mock repository not finding user in db and so not deleting anything

        //Act
        var serviceResult = await _userService.DeleteCurrentAsync();

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }
    
    [Fact]
    public async Task DeleteCurrentUserAsync_NotAuth_ReturnsFailureUnauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
        var serviceResult = await _userService.DeleteCurrentAsync();

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }
}