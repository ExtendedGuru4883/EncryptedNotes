namespace Shared.Enums;

public enum ServiceResultErrorType
{
    Conflict = 409,
    NotFound = 404,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    InternalServerError = 500,
}