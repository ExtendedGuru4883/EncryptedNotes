namespace BusinessLogic.Helpers.Crypto.Interfaces;

public interface ISignatureHelper
{
    public int PublicKeyBytesSize { get; }
    public int PublicKeyBase64Length { get; }
    public int SignatureBytesSize { get; }
    public int SignatureBase64Length { get; }
    
    bool VerifyDetachedSignature (byte[] signatureBytes, byte[] messageBytes, byte[] publicKeyBytes);
}