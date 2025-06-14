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
            statusCode = serviceResult.SuccessType switch
            {
                ServiceResultSuccessType.Created => StatusCodes.Status201Created,
                _ => StatusCodes.Status200OK
            };
            
            content = serviceResult.Data;
        }
        else
        {
            statusCode = serviceResult.ErrorType switch
            {
                ServiceResultErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ServiceResultErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ServiceResultErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ServiceResultErrorType.NotFound => StatusCodes.Status404NotFound,
                ServiceResultErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
            
            content = new ErrorResponseDto(serviceResult.ErrorMessage);
        }

        return new JsonResult(content)
        {
            ContentType = "application/json",
            StatusCode = statusCode
        };
    }
}