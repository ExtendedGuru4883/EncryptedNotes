namespace BusinessLogic.Helpers.Crypto.Interfaces;

public interface ISignatureHelper
{
    public int PublicKeyBytesSize { get; }
    public int SignatureBytesSize { get; }
    
    bool VerifyDetachedSignature (byte[] signatureBytes, byte[] messageBytes, byte[] publicKeyBytes);
}