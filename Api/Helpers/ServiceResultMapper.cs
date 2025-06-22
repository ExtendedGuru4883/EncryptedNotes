using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;

namespace EncryptedNotes.Helpers;

public static class ServiceResultMapper
{
    public static ActionResult<T> ToActionResult<T>(ServiceResult<T> serviceResult)
    {
        if (!serviceResult.IsSuccess) return MapFailure(serviceResult.ErrorType, serviceResult.ErrorMessage);
        
        if (serviceResult.SuccessType is ServiceResultSuccessType.NoContent)
            throw new InvalidOperationException(
                "SuccessType 'NoContent' is invalid for generic ServiceResult<T>. Use non-generic ServiceResult instead.");
            
        return new JsonResult(serviceResult.Data)
        {
            ContentType = "application/json",
            StatusCode = serviceResult.SuccessType is null ? 200 : (int)serviceResult.SuccessType
        };

    }

    public static ActionResult ToActionResult(ServiceResult serviceResult)
    {
        if (!serviceResult.IsSuccess) return MapFailure(serviceResult.ErrorType, serviceResult.ErrorMessage);
        
        if (serviceResult.SuccessType != ServiceResultSuccessType.NoContent)
            throw new InvalidOperationException(
                $"SuccessType '{serviceResult.SuccessType}' for non-generic ServiceResult. Expected 'NoContent'. Use generic ServiceResult instead.");

        return new NoContentResult();

    }

    private static JsonResult MapFailure(ServiceResultErrorType? errorType, string errorMessage)
    {
        return new JsonResult(new ErrorResponseDto(errorMessage))
        {
            ContentType = "application/json",
            StatusCode = errorType is null ? 500 : (int)errorType
        };
    }
}