using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Responses;

namespace EncryptedNotes.Helpers;

public static class ServiceResponseMapper
{
    public static ActionResult<T> ToActionResult<T>(ServiceResult<T> serviceResult)
    {
        if (serviceResult.IsSuccess)
        {
            var statusCode = serviceResult.SuccessType switch
            {
                ServiceResponseSuccessType.Created => StatusCodes.Status201Created,
                ServiceResponseSuccessType.NoContent => StatusCodes.Status204NoContent,
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
                ServiceResponseErrorType.Conflict => StatusCodes.Status409Conflict,
                ServiceResponseErrorType.NotFound => StatusCodes.Status404NotFound,
                ServiceResponseErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ServiceResponseErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ServiceResponseErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return new JsonResult(new ErrorResponseDto(serviceResult.ErrorMessage))
            {
                StatusCode = statusCode
            };
        }
    }
}