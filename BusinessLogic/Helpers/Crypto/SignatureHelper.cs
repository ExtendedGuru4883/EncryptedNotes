using BusinessLogic.Helpers.Crypto.Interfaces;
using Sodium;

namespace BusinessLogic.Helpers.Crypto;

public class SignatureHelper : ISignatureHelper
{
    public bool VerifyDetachedSignature(byte[] signatureBytes, byte[] messageBytes, byte[] publicKeyBytes)
    {
        return PublicKeyAuth.VerifyDetached(signatureBytes, messageBytes, publicKeyBytes);
    }
}