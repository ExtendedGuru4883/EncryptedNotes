using Core.Abstractions.BusinessLogic.Services;
using Sodium;

namespace BusinessLogic.Services;

public class SignatureService : ISignatureService
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