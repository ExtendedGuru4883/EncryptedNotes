using AutoMapper;
using BusinessLogic.Helpers.Crypto.Interfaces;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Test.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto;
using Shared.Enums;

namespace Test.Unit.BusinessLogic.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ISignatureHelper> _mockSignatureHelper = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ILogger<UserService>> _mockLogger = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_mockUserRepository.Object, _mockSignatureHelper.Object,_mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ValidUser_ReturnsSuccessCreated()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockSignatureHelper.Setup(s => s.PublicKeyBytesSize)
            .Returns((Convert.FromBase64String(userDto.PublicKeyBase64).Length));
        _mockUserRepository.Setup(r => r.UsernameExists(It.IsAny<string>()))
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
        _mockSignatureHelper.Setup(s => s.PublicKeyBytesSize)
            .Returns((Convert.FromBase64String(userDto.PublicKeyBase64).Length));
        _mockUserRepository.Setup(r => r.UsernameExists(It.IsAny<string>()))
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
        _mockSignatureHelper.Setup(s => s.PublicKeyBytesSize)
            .Returns((Convert.FromBase64String(userDto.PublicKeyBase64).Length + 1));

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.BadRequest);
    }
}