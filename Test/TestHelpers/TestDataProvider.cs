using Shared.Dto;

namespace Test.TestHelpers;

public static class TestDataProvider
{
    public static string GetBase64Value(bool isValid)
    {
        return isValid ? "dmFsaWRCYXNlNjQ=" : "dmFsaWRCYXNlNjQ=!!";
    }

    public static UserDto GetUserDto()
    {
        return new UserDto
        {
            Username = "test-username",
            SignatureSaltBase64 = "dmFsaWRCYXNlNjQ=",
            EncryptionSaltBase64 = "dmFsaWRCYXNlNjQ=",
            PublicKeyBase64 = "dmFsaWRCYXNlNjQ="
        };
    }
}