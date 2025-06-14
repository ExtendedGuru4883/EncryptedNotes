using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Responses;

namespace EncryptedNotes.Helpers;

public static class ServiceResultMapper
{
    public static ActionResult<T> ToActionResult<T>(ServiceResult<T> serviceResult)
    {
        if (serviceResult.IsSuccess)
        {
            var statusCode = serviceResult.SuccessType switch
            {
                ServiceResultSuccessType.Created => StatusCodes.Status201Created,
                ServiceResultSuccessType.NoContent => StatusCodes.Status204NoContent,
                _ => StatusCodes.Status200OK
            };

            return statusCode == StatusCodes.Status204NoContent
                ? new NoContentResult()
                : new JsonResult(serviceResult.Data)
                {
                    StatusCode = statusCode
                };
        }
        else
        {
            var statusCode = serviceResult.ErrorType switch
            {
                ServiceResultErrorType.Conflict => StatusCodes.Status409Conflict,
                ServiceResultErrorType.NotFound => StatusCodes.Status404NotFound,
                ServiceResultErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ServiceResultErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ServiceResultErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return new JsonResult(new ErrorResponseDto(serviceResult.ErrorMessage))
            {
                StatusCode = statusCode
            };
        }
    }
}