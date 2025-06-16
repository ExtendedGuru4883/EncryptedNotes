using Shared.Enums;

namespace Shared.Results;

public record ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public string ErrorMessage { get; private init; } = string.Empty;
    public ServiceResultSuccessType? SuccessType { get; private init; }
    public ServiceResultErrorType? ErrorType { get; private init; }

    //Success
    private static ServiceResult<T> Success(T data, ServiceResultSuccessType successType) => new()
    {
        IsSuccess = true,
        Data = data,
        SuccessType = successType
    };
    
    public static ServiceResult<T> SuccessOk(T data) => Success(data, ServiceResultSuccessType.Ok);
    
    public static ServiceResult<T> SuccessCreated(T data) => Success(data, ServiceResultSuccessType.Created);
    
    //Failure
    public static ServiceResult<T> Failure(string errorMessage, ServiceResultErrorType errorType) => new()
    {
        IsSuccess = false,
        ErrorType = errorType,
        ErrorMessage = errorMessage
    };
}

public record ServiceResult
{
    public bool IsSuccess { get; private init; }
    public string ErrorMessage { get; private init; } = string.Empty;
    public ServiceResultSuccessType? SuccessType { get; private init; }
    public ServiceResultErrorType? ErrorType { get; private init; }

    //Success
    public static ServiceResult SuccessNoContent() => new()
    {
        IsSuccess = true,
        SuccessType = ServiceResultSuccessType.NoContent
    };
    
    //Failure
    public static ServiceResult Failure(string errorMessage, ServiceResultErrorType errorType) => new()
    {
        IsSuccess = false,
        ErrorType = errorType,
        ErrorMessage = errorMessage
    };
}