using AutoMapper;
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
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ILogger<UserService>> _mockLogger = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ValidUser_ReturnsSuccessCreated()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Created);
    }

    [Fact]
    public async Task AddAsync_UsernameExists_ReturnsConflictFailure()
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(It.IsAny<string>()))
            .ReturnsAsync(true); //Mocking username already existing

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Conflict);
    }

    [Fact]
    public async Task AddAsync_InvalidSignatureSaltBase64_ReturnsBadRequestFailure() =>
        await AddAsync_InvalidBase64Field_ReturnsBadRequestFailure(nameof(UserDto.SignatureSaltBase64));

    [Fact]
    public async Task AddAsync_InvalidEncryptionSaltBase64_ReturnsBadRequestFailure() =>
        await AddAsync_InvalidBase64Field_ReturnsBadRequestFailure(nameof(UserDto.EncryptionSaltBase64));

    [Fact]
    public async Task AddAsync_InvalidPublicKeyBase64_ReturnsBadRequestFailure() =>
        await AddAsync_InvalidBase64Field_ReturnsBadRequestFailure(nameof(UserDto.PublicKeyBase64));

    private async Task AddAsync_InvalidBase64Field_ReturnsBadRequestFailure(string invalidField)
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "test-username",
            SignatureSaltBase64 = nameof(UserDto.SignatureSaltBase64) == invalidField
                ? TestDataProvider.GetInvalidBase64Value()
                : TestDataProvider.GetValidBase64Value(),
            EncryptionSaltBase64 = nameof(UserDto.EncryptionSaltBase64) == invalidField
                ? TestDataProvider.GetInvalidBase64Value()
                : TestDataProvider.GetValidBase64Value(),
            PublicKeyBase64 = nameof(UserDto.PublicKeyBase64) == invalidField
                ? TestDataProvider.GetInvalidBase64Value()
                : TestDataProvider.GetValidBase64Value()
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.BadRequest);
    }
}