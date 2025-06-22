using EncryptedNotes.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Enums;
using Shared.Results;
using Test.TestHelpers;

namespace Test.Unit.Api.Helpers;

[Trait("Category", "Unit")]
public class ServiceResultMapperTests
{
    [Theory]
    [InlineData(ServiceResultSuccessType.Ok, StatusCodes.Status200OK)]
    [InlineData(ServiceResultSuccessType.Created, StatusCodes.Status201Created)]
    public void ToActionResultGeneric_Success_ReturnsStatusCodeAndData(ServiceResultSuccessType successType,
        int statusCode)
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();
        var serviceResult = successType switch
        {
            ServiceResultSuccessType.Ok => ServiceResult<UserDto>.SuccessOk(userDto),
            ServiceResultSuccessType.Created => ServiceResult<UserDto>.SuccessCreated(userDto),
            _ => throw new ArgumentOutOfRangeException(nameof(successType), successType, null)
        };

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        CommonAssertions.AssertActionResultWithJsonResponse(actionResult, userDto, statusCode);
    }
    
    [Fact]
    public static void ToActionResult_SuccessNoContent_Returns204NoContent()
    {
        //Arrange
        var serviceResult = ServiceResult.SuccessNoContent();

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        actionResult.Should().BeOfType<NoContentResult>();
    }
    
    [Theory]
    [InlineData(ServiceResultErrorType.BadRequest, StatusCodes.Status400BadRequest)]
    [InlineData(ServiceResultErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ServiceResultErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ServiceResultErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ServiceResultErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ServiceResultErrorType.InternalServerError, StatusCodes.Status500InternalServerError)]
    public void ToActionResultGeneric_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType errorType,
        int statusCode)
    {
        //Arrange
        var errorResponseDto = TestDataProvider.GetErrorResponseDto();
        var serviceResult = ServiceResult<UserDto>.Failure(errorResponseDto.ErrorMessage, errorType);

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        CommonAssertions.AssertActionResultWithJsonResponse(actionResult, errorResponseDto, statusCode);
    }
    
    [Theory]
    [InlineData(ServiceResultErrorType.BadRequest, StatusCodes.Status400BadRequest)]
    [InlineData(ServiceResultErrorType.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ServiceResultErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ServiceResultErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ServiceResultErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ServiceResultErrorType.InternalServerError, StatusCodes.Status500InternalServerError)]
    public void ToActionResultNonGeneric_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType errorType,
        int statusCode)
    {
        //Arrange
        var errorResponseDto = TestDataProvider.GetErrorResponseDto();
        var serviceResult = ServiceResult.Failure(errorResponseDto.ErrorMessage, errorType);

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        CommonAssertions.AssertActionResultWithJsonResponse(actionResult, errorResponseDto, statusCode);
    }
}