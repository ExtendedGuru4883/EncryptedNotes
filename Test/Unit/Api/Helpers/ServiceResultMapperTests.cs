using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Http;
using Shared.Dto;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;
using Test.TestHelpers;

namespace Test.Unit.Api.Helpers;

public class ServiceResultMapperTests
{
    [Fact]
    public static void ToActionResult_SuccessOK_Returns200AndContent() =>
        ToActionResult_Success_ReturnsStatusCodeAndData(ServiceResultSuccessType.Ok, StatusCodes.Status200OK);

    [Fact]
    public static void ToActionResult_SuccessCreated_Returns201AndContent() =>
        ToActionResult_Success_ReturnsStatusCodeAndData(ServiceResultSuccessType.Created, StatusCodes.Status201Created);

    [Fact]
    public void ToActionResult_FailureBadRequest_Returns400AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.BadRequest,
            StatusCodes.Status400BadRequest);
    
    [Fact]
    public void ToActionResult_FailureUnauthorized_Returns401AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.Unauthorized,
            StatusCodes.Status401Unauthorized);
    
    [Fact]
    public void ToActionResult_FailureForbidden_Returns403AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.Forbidden,
            StatusCodes.Status403Forbidden);
    
    [Fact]
    public void ToActionResult_FailureNotFound_Returns404AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.NotFound,
            StatusCodes.Status404NotFound);
    
    [Fact]
    public void ToActionResult_FailureConflict_Returns409AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.Conflict,
            StatusCodes.Status409Conflict);
    
    [Fact]
    public void ToActionResult_FailureInternalServerError_Returns500AndErrorResponseDto() =>
        ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType.InternalServerError,
            StatusCodes.Status500InternalServerError);

    private static void ToActionResult_Success_ReturnsStatusCodeAndData(ServiceResultSuccessType successType,
        int statusCode)
    {
        //Arrange
        var userDto = TestDataProvider.GetUserDto();
        var serviceResult = ServiceResult<UserDto>.Success(userDto, successType);

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        CommonAssertions.AssertActionResultWithJsonResponse(actionResult, userDto, statusCode);
    }

    private static void ToActionResult_Failure_ReturnsStatusCodeAndErrorResponseDto(ServiceResultErrorType errorType,
        int statusCode)
    {
        //Arrange
        var errorResponseDto = TestDataProvider.GetErrorResponseDto();
        var serviceResult = ServiceResult<ErrorResponseDto>.Failure(errorResponseDto.ErrorMessage, errorType);

        //Act
        var actionResult = ServiceResultMapper.ToActionResult(serviceResult);

        //Assert
        CommonAssertions.AssertActionResultWithJsonResponse(actionResult, errorResponseDto, statusCode);
    }
}