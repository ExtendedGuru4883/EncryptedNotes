using Shared.Enums;

namespace Shared.Results;

public record ServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public ServiceResultSuccessType? SuccessType { get; init; }
    public ServiceResultErrorType? ErrorType { get; init; }

    //Success
    public static ServiceResult<T> Success(T data, ServiceResultSuccessType successType) => new ServiceResult<T>()
    {
        IsSuccess = true,
        Data = data,
        SuccessType = successType
    };
    
    public static ServiceResult<T> SuccessOk(T data) => new ServiceResult<T>()
    {
        IsSuccess = true,
        Data = data,
        SuccessType = ServiceResultSuccessType.Ok
    };
    
    //Failure
    public static ServiceResult<T> Failure(string errorMessage, ServiceResultErrorType errorType) => new ServiceResult<T>()
    {
        IsSuccess = false,
        ErrorType = errorType,
        ErrorMessage = errorMessage
    };
}