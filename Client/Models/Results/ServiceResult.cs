namespace Client.Models.Results;

public record ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public string ErrorMessage { get; private init; } = string.Empty;

    //Success
    public static ServiceResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data,
    };
    
    //Failure
    public static ServiceResult<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

public record ServiceResult
{
    public bool IsSuccess { get; private init; }
    public string ErrorMessage { get; private init; } = string.Empty;

    //Success
    public static ServiceResult Success() => new()
    {
        IsSuccess = true,
    };
    
    //Failure
    public static ServiceResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}