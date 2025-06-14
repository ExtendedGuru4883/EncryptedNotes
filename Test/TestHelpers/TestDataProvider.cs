using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace Test.TestHelpers;

public static class TestDataProvider
{
    public static string GetValidBase64Value()
    {
        return "MTIzNDU2Nzg5MGFiY2RlZg==";
    }
    
    public static string GetRandomValidBase64(int length = 16)
    {
        var randomBytes = new byte[length];
        var random = new Random();
        
        random.NextBytes(randomBytes);
        
        return Convert.ToBase64String(randomBytes);
    }

    public static string GetInvalidBase64Value()
    {
        return "MTIzNDU2Nzg5MGFiY2RlZg==!!";
    }

    public static UserDto GetUserDto()
    {
        return new UserDto
        {
            Username = "test-username",
            SignatureSaltBase64 = GetValidBase64Value(),
            EncryptionSaltBase64 = GetValidBase64Value(),
            PublicKeyBase64 = GetValidBase64Value()
        };
    }
    
    public static ErrorResponseDto GetErrorResponseDto()
    {
        return new ErrorResponseDto("test-error");
    }

    public static LoginRequest GetValidLoginRequest()
    {
        return new LoginRequest
        {
            Username = "username",
            NonceBase64 = GetValidBase64Value(),
            NonceSignatureBase64 = GetValidBase64Value()
        };
    }
}