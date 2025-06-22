namespace Client.Exceptions;

public class EncryptionKeyMissingException : Exception
{
    public EncryptionKeyMissingException()
        : base("Encryption key is missing") {}

    public EncryptionKeyMissingException(string message)
        : base(message) {}

    public EncryptionKeyMissingException(string message, Exception innerException)
        : base(message, innerException) {}
}