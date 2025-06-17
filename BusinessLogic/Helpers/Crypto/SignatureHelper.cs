using BusinessLogic.Helpers.Crypto.Interfaces;
using Sodium;

namespace BusinessLogic.Helpers.Crypto;

public class SignatureHelper : ISignatureHelper
{
    public int PublicKeyBytesSize => PublicKeyAuth.PublicKeyBytes;
    public int PublicKeyBase64Length => (int)(4 * Math.Ceiling((double)PublicKeyBytesSize / 3));
    public int SignatureBytesSize =>  PublicKeyAuth.SignatureBytes;
    public int SignatureBase64Length => (int)(4 * Math.Ceiling((double)SignatureBytesSize / 3));

    public bool VerifyDetachedSignature(byte[] signatureBytes, byte[] messageBytes, byte[] publicKeyBytes)
    {
        return PublicKeyAuth.VerifyDetached(signatureBytes, messageBytes, publicKeyBytes);
    }
}