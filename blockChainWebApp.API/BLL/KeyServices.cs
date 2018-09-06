using System.Text;
using blockChainWebApp.API.BLL.HelpClasses;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace blockChainWebApp.API.BLL
{
    public class KeyServices
    {
        public static bool VerifySignature(string message, string publicKey, string signature)
        {
            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var publicKeyBytes = Base58Encoding.Decode(publicKey);
            var q = curve.Curve.DecodePoint(publicKeyBytes);
            var keyParameters = new ECPublicKeyParameters(q,
                    domain);

            var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            signer.Init(false, keyParameters);
            signer.BlockUpdate(Encoding.ASCII.GetBytes(message), 0, message.Length);

            var signatureBytes = Base58Encoding.Decode(signature);
            return signer.VerifySignature(signatureBytes);
        }
    }
}