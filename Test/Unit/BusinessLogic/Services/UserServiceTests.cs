using AutoMapper;
using BusinessLogic.Services;
using Core.Entities;
using Core.Interfaces.DataAccess.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto;
using Shared.Enums;

namespace Test.Unit.BusinessLogic.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();

        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ValidUser_ReturnsSuccess()
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "new-username",
            SignatureSaltBase64 = "dmFsaWRCYXNlNjQ=",
            EncryptionSaltBase64 = "dmFsaWRCYXNlNjQ=",
            PublicKeyBase64 = "dmFsaWRCYXNlNjQ="
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(userDto.Username))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        serviceResult.IsSuccess.Should().BeTrue("because the user is valid and should be added successfully");
        serviceResult.ErrorMessage.Should().BeEmpty("because if IsSuccess is true there should be no error message");
        serviceResult.Data.Should()
            .BeEquivalentTo(userDto, "because in case of success the service should return the created user");
        serviceResult.SuccessType.Should().Be(ServiceResponseSuccessType.Created);
    }

    [Fact]
    public async Task AddAsync_UsernameExists_ReturnsConflictFailure()
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "existing-username",
            SignatureSaltBase64 = "test",
            EncryptionSaltBase64 = "test",
            PublicKeyBase64 = "test"
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(userDto.Username))
            .ReturnsAsync(true);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        serviceResult.IsSuccess.Should().BeFalse("because if the username already exists the user should not be added");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResponseErrorType.Conflict);
    }

    [Fact]
    public async Task AddAsync_InvalidSignatureSaltBase64_ReturnsBadRequestFailure()
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "test",
            SignatureSaltBase64 = "dmFsaWRCYXNlNjQ=!!", //Invalid
            EncryptionSaltBase64 = "dmFsaWRCYXNlNjQ=",
            PublicKeyBase64 = "dmFsaWRCYXNlNjQ="
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(userDto.Username))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse("because if SignatureSaltBase64 is not a valid base64 the user should not be added");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResponseErrorType.BadRequest);
    }

    [Fact]
    public async Task AddAsync_InvalidEncryptionSaltBase64_ReturnsBadRequestFailure()
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "test",
            SignatureSaltBase64 = "dmFsaWRCYXNlNjQ=",
            EncryptionSaltBase64 = "dmFsaWRCYXNlNjQ=!!", //Invalid
            PublicKeyBase64 = "dmFsaWRCYXNlNjQ="
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(userDto.Username))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse("because if EncryptionSaltBase64 is not a valid base64 the user should not be added");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResponseErrorType.BadRequest);
    }

    [Fact]
    public async Task AddAsync_InvalidPublicKeyBase64_ReturnsBadRequestFailure()
    {
        //Arrange
        var userDto = new UserDto
        {
            Username = "test",
            SignatureSaltBase64 = "dmFsaWRCYXNlNjQ=",
            EncryptionSaltBase64 = "dmFsaWRCYXNlNjQ=",
            PublicKeyBase64 = "dmFsaWRCYXNlNjQ=!!" //Invalid
        };

        //Mock
        _mockUserRepository.Setup(r => r.UsernameExists(userDto.Username))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _userService.AddAsync(userDto);

        //Assert
        serviceResult.IsSuccess.Should()
            .BeFalse("because if PublicKeyBase64 is not a valid base64 the user should not be added");
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because if IsSuccess is false there should be a error message");
        serviceResult.Data.Should().BeNull("because if IsSuccess is false there should be no data");
        serviceResult.ErrorType.Should().Be(ServiceResponseErrorType.BadRequest);
    }
}