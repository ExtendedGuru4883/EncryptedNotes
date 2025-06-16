using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;

namespace EncryptedNotes.Helpers;

public static class ServiceResultMapper
{
    public static ActionResult<T> ToActionResult<T>(ServiceResult<T> serviceResult)
    {
        int statusCode;
        object? content;

        if (serviceResult.IsSuccess)
        {
            statusCode = serviceResult.SuccessType is null ? 200 : (int)serviceResult.SuccessType;

            if (statusCode == 204)
                throw new InvalidOperationException(
                    "SuccessType 'NoContent' is invalid for generic ServiceResult<T>. Use non-generic ServiceResult instead.");

            content = serviceResult.Data;
        }
        else
        {
            statusCode = serviceResult.ErrorType is null ? 500 : (int)serviceResult.ErrorType;

            content = new ErrorResponseDto(serviceResult.ErrorMessage);
        }

        return new JsonResult(content)
        {
            ContentType = "application/json",
            StatusCode = statusCode
        };
    }

    public static ActionResult ToActionResult(ServiceResult serviceResult)
    {
        if (serviceResult.IsSuccess)
        {
            if (serviceResult.SuccessType != ServiceResultSuccessType.NoContent)
                throw new InvalidOperationException(
                    $"SuccessType '{serviceResult.SuccessType}' for non-generic ServiceResult. Expected 'NoContent'. Use generic ServiceResult instead.");

            return new NoContentResult();
        }

        var statusCode = serviceResult.ErrorType is null ? 500 : (int)serviceResult.ErrorType;

        return new JsonResult(new ErrorResponseDto(serviceResult.ErrorMessage))
        {
            ContentType = "application/json",
            StatusCode = statusCode
        };
    }
}