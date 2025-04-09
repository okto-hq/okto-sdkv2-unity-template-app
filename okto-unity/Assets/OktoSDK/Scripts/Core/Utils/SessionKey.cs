using Nethereum.Signer;

/*
 * SessionKey Class
 *
 * This class generates a new Ethereum session key pair.
 * It includes a private key, an uncompressed public key, and an Ethereum address.
 *
 * Methods:
 * - Create: Generates a new session key using Nethereum's EthECKey.
 */

namespace OktoSDK
{
    public class SessionKey
    {
        public string PrivateKeyHexWith0x { get; private set; }
        public string UncompressedPublicKeyHexWith0x { get; private set; }
        public string EthereumAddress { get; private set; }

        public static SessionKey Create()
        {
            var ecKey = EthECKey.GenerateKey();

            return new SessionKey
            {
                PrivateKeyHexWith0x = ecKey.GetPrivateKeyAsBytes().ToHex(true).ToLower(),
                UncompressedPublicKeyHexWith0x = ecKey.GetPubKey().ToHex(true).ToLower(),
                EthereumAddress = ecKey.GetPublicAddress().ToLower()
            };
        }
    }

}