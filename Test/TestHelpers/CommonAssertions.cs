using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Results;

namespace Test.TestHelpers;

public static class CommonAssertions
{
    public static void AssertServiceResultSuccess<T>(ServiceResult<T> serviceResult, ServiceResultSuccessType expectedSuccessType)
    {
        serviceResult.IsSuccess.Should()
            .BeTrue();
        serviceResult.ErrorMessage.Should().BeEmpty("because there should be no error message when IsSuccess is true");
        serviceResult.Data.Should()
            .BeOfType<T>();
        serviceResult.SuccessType.Should().Be(expectedSuccessType);
    }
    
    public static void AssertServiceResultFailure<T>(ServiceResult<T> serviceResult, ServiceResultErrorType expectedErrorType)
    {
        serviceResult.IsSuccess.Should()
            .BeFalse();
        serviceResult.ErrorMessage.Should()
            .NotBeNullOrEmpty("because an error message should be present when IsSuccess is false");
        serviceResult.Data.Should().BeNull("because there should be no data when IsSuccess is false");
        serviceResult.ErrorType.Should().Be(expectedErrorType);
    }

    public static void AssertActionResultWithJsonResponse<T>(ActionResult<T> actionResult, T expectedContent,
        int expectedStatusCode)
    {
        var jsonResult = actionResult.Result.Should().BeOfType<JsonResult>().Which;
        jsonResult.StatusCode.Should().Be(expectedStatusCode);
        jsonResult.ContentType.Should().Be("application/json");
        jsonResult.Value.Should().BeEquivalentTo(expectedContent);
    }
}