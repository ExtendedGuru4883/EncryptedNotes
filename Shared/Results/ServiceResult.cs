using Shared.Enums;

namespace Shared.Responses;

public record ServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public ServiceResponseSuccessType? SuccessType { get; init; }
    public ServiceResponseErrorType? ErrorType { get; init; }

    //Success
    public static ServiceResult<T> Success(T data, ServiceResponseSuccessType successType) => new ServiceResult<T>()
    {
        IsSuccess = true,
        Data = data,
        SuccessType = successType
    };
    
    public static ServiceResult<T> SuccessOk(T data) => new ServiceResult<T>()
    {
        IsSuccess = true,
        Data = data,
        SuccessType = ServiceResponseSuccessType.Ok
    };
    
    //Failure
    public static ServiceResult<T> Failure(string errorMessage, ServiceResponseErrorType errorType) => new ServiceResult<T>()
    {
        IsSuccess = false,
        ErrorType = errorType,
        ErrorMessage = errorMessage
    };
}