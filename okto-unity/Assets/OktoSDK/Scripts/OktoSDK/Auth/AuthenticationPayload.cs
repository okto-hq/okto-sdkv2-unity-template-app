using System;
using System.Threading.Tasks;
using Nethereum.Signer;
using UnityEngine;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;

/*
 * AuthenticationPayload Class
 *
 * This class is responsible for generating an authentication payload used for user session authentication
 * and validation in the OktoSDK. It constructs session data, signs messages using cryptographic methods, 
 * and ensures secure communication between the client and the authentication system.
 *
 * Features:
 * - Generates a nonce (random unique identifier) for session authentication.
 * - Constructs authentication session data with paymaster details, session keys, and gas fees.
 * - Uses Keccak-256 hashing to create a secure message payload.
 * - Signs the authentication data using Ethereum message signing.
 * - Generates two signatures:
 *   1. `SessionPkClientSignature` - Signed using the client’s private key.
 *   2. `SessionDataUserSignature` - Signed using the user's session private key.
 *
 * Methods:
 * - Generate(): Creates and returns an authentication payload with signed session data.
 * - SignMessage(): Signs a given message using Ethereum's cryptographic signing process.
 * - StringToByteArray(): Converts a hexadecimal string to a byte array.
 *
 * Usage:
 * - This class is used to create a secure authentication payload for blockchain-based authentication.
 * - It integrates cryptographic functions such as Keccak hashing and Ethereum digital signatures.
 * - The generated payload is sent to the authentication server for validation.
 *
 * Debugging:
 * - Multiple debug logs are added to trace nonce generation, session data, and final payload.
 * - Ensure `clientPriv` and `sessionKey.PrivateKeyHexWith0x` are securely managed to prevent exposure.
 */

namespace OktoSDK
{
    public class AuthenticationPayload
    {
        public static async Task<AuthenticatePayloadParam> Generate(
            OktoClient client,
            AuthData authData,
            SessionKey sessionKey,
            string clientSWA,
            string clientPriv)
        {
            var nonce = Guid.NewGuid().ToString("D");
            Debug.Log($"Generated nonce: {nonce}");
            Debug.Log($"client: {client}");
            Debug.Log($"clientSWA: {clientSWA}");
            Debug.Log($"client.Env.PaymasterAddress: {client.Env.PaymasterAddress}");
            Debug.Log($"clientPriv: {client.Env.PaymasterAddress}");

            var sessionData = new AuthSessionData
            {
                Nonce = nonce,
                ClientSWA = clientSWA,
                SessionPk = sessionKey.UncompressedPublicKeyHexWith0x,
                MaxPriorityFeePerGas = "0xBA43B7400",
                MaxFeePerGas = "0xBA43B7400",
                Paymaster = client.Env.PaymasterAddress,
                PaymasterData = await PaymasterDataGenerator.Generate(
                    clientSWA,
                    clientPriv,
                    nonce,
                    DateTime.UtcNow.AddHours(6)
                )
            };

            Debug.Log($"Session Data: {JsonConvert.SerializeObject(sessionData)}");

            string address = sessionKey.EthereumAddress.ToLower().Replace("0x", "");
            string paddedAddress = address.PadLeft(64, '0');
            byte[] abiEncodedAddress = StringToByteArray(paddedAddress);

            // Then calculate Keccak hash
            var keccak = new KeccakDigest(256);
            byte[] messageBytes = new byte[32];
            keccak.BlockUpdate(abiEncodedAddress, 0, abiEncodedAddress.Length);
            keccak.DoFinal(messageBytes, 0);

            var message = new { raw = messageBytes };


            var payload = new AuthenticatePayloadParam
            {
                AuthData = authData,
                SessionData = sessionData,
                SessionPkClientSignature = await SignMessage(messageBytes, clientPriv),
                SessionDataUserSignature = await SignMessage(messageBytes, sessionKey.PrivateKeyHexWith0x)
            };

            Debug.Log($"Final payload: {JsonConvert.SerializeObject(payload)}");
            return payload;
        }

        private static Task<string> SignMessage(byte[] message, string privateKey)
        {
            var signer = new EthereumMessageSigner();
            var signature = signer.Sign(message, privateKey);
            return Task.FromResult(signature.StartsWith("0x") ? signature : "0x" + signature);
        }

        // Helper method to convert hex string to byte array
        private static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
}
}




