using System;
using System.Threading.Tasks;
using Nethereum.Signer;
using UnityEngine;
using Nethereum.ABI;
using Nethereum.Util;
using System.Linq;

/*
 * PaymasterDataGenerator Class
 *
 * This static class is responsible for generating paymaster data required for transaction processing.
 * It constructs and encodes the necessary data, signs the message, and returns the final encoded result.
 *
 * Features:
 * - Generates paymaster data using clientSWA, private key, nonce, and validity timestamps.
 * - Converts timestamps to Unix format and encodes them into a byte array.
 * - Cleans and formats nonce values to ensure they match the expected 32-byte format.
 * - Computes the Keccak256 hash of the packed data for cryptographic security.
 * - Signs the hashed message using cryptographic signing methods.
 * - Encodes the final paymaster data using ABI encoding for compatibility with smart contracts.
 *
 * Methods:
 * - Generate: Main function to generate paymaster data.
 * - SignMessage: Signs a message using the provided Ethereum private key.
 * - HexToByteArray: Converts a hexadecimal string into a byte array.
 */
namespace OktoSDK
{
    public static class PaymasterDataGenerator
    {


        public static async Task<string> Generate(
        string clientSWA,
        string clientPrivateKey,
        string nonce,
        DateTime validUntil,
        DateTime? validAfter = null)
        {
            Debug.Log($"Generating paymaster data with:");
            Debug.Log($"clientSWA: {clientSWA}");
            Debug.Log($"nonce: {nonce}");
            Debug.Log($"validUntil: {validUntil}");
            Debug.Log($"validAfter: {validAfter}");
            Debug.Log($"clientPrivateKey: {clientPrivateKey}");

            // Convert validUntil to Unix timestamp
            var validUntilTimestamp = ((DateTimeOffset)validUntil).ToUnixTimeSeconds();

            // Convert validAfter to Unix timestamp (default to 0 if not provided)
            var validAfterTimestamp = validAfter.HasValue ?
                ((DateTimeOffset)validAfter.Value).ToUnixTimeSeconds() : 0;

            // Clean and format the nonce (remove hyphens and convert to hex)
            string cleanNonce = nonce.Replace("-", "").ToLowerInvariant();
            var nonceBytes32 = cleanNonce.PadLeft(64, '0');
            byte[] nonceEncoded = HexToByteArray(nonceBytes32);

            Debug.Log($"Formatted nonce: 0x{nonceBytes32}");

            // Convert address to bytes (20 bytes)
            byte[] addressEncoded = HexToByteArray(clientSWA.Replace("0x", "").PadLeft(40, '0'));

            // Convert timestamps to big-endian bytes and ensure exactly 6 bytes
            byte[] validUntilEncoded = new byte[6];
            byte[] tempUntil = BitConverter.GetBytes(validUntilTimestamp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempUntil);
            Array.Copy(tempUntil, tempUntil.Length - 6, validUntilEncoded, 0, 6);

            byte[] validAfterEncoded = new byte[6];
            byte[] tempAfter = BitConverter.GetBytes(validAfterTimestamp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempAfter);
            Array.Copy(tempAfter, tempAfter.Length - 6, validAfterEncoded, 0, 6);

            // Concatenate all components in the correct order
            byte[] packedMessage = nonceEncoded
                .Concat(addressEncoded)
                .Concat(validUntilEncoded)
                .Concat(validAfterEncoded)
                .ToArray();

            // Calculate keccak256 hash
            var sha3 = new Sha3Keccack();
            var paymasterDataHash = sha3.CalculateHash(packedMessage);

            // Convert hash to lowercase hex string first
            string hashHex = "0x" + BitConverter.ToString(paymasterDataHash).Replace("-", "").ToLowerInvariant();
            byte[] messageToSign = HexToByteArray(hashHex);
            Debug.Log($"Message to sign (hex): {hashHex}");

            string signature = await SignMessage(messageToSign, clientPrivateKey);

            // Create ABI encoder instance
            var abiEncoder = new ABIEncode();

            // Encode the final paymaster data
            byte[] finalData = abiEncoder.GetABIEncoded(
                new[] {
                        new ABIValue("address", clientSWA),
                        new ABIValue("uint48", validUntilTimestamp),
                        new ABIValue("uint48", validAfterTimestamp),
                        new ABIValue("bytes", HexToByteArray(signature))
                }
            );

            string result = "0x" + BitConverter.ToString(finalData).Replace("-", "").ToLowerInvariant();
            Debug.Log($"Final paymaster data: {result}");

            return result;

        }

        private static Task<string> SignMessage(byte[] message, string privateKey)
        {
            var signer = new EthereumMessageSigner();
            var signature = signer.Sign(message, privateKey);
            return Task.FromResult(signature.StartsWith("0x") ? signature : "0x" + signature);
        }

        private static byte[] HexToByteArray(string hex)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }


} 
