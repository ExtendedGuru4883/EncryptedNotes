namespace BusinessLogic.Helpers.Crypto.Interfaces;

public interface ISignatureHelper
{
    bool VerifyDetachedSignature (byte[] signatureBytes, byte[] messageBytes, byte[] publicKeyBytes);
}