using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using UnityEngine;
using Nethereum.ABI;
using Nethereum.Util;
using System.Numerics;
using UserOpType = OktoSDK.UserOp.UserOp;
using OktoSDK.UserOp;
using OktoSDK.Auth;

/*
 * UserOpSign Class
 * 
 * This class is responsible for signing a User Operation (UserOp) in the OktoSDK.
 * It ensures the transaction is properly formatted, hashed, and signed using the 
 * Ethereum signing mechanism. The primary functionality includes:
 * 
 * 1. SignUserOp:
 *    - Takes a UserOp object, private key, entry point address, and chain ID.
 *    - Packs the UserOp data and generates a hash.
 *    - Signs the hash using the EthereumMessageSigner.
 *    - Returns the signed UserOp object with the generated signature.
 * 
 * 2. GeneratePackedUserOp:
 *    - Validates the UserOp parameters.
 *    - Packs various fields (gas limits, gas fees, paymaster data) with proper padding.
 *    - Returns a PackedUserOp structure for further processing.
 * 
 * 3. GenerateUserOpHash:
 *    - Hashes and encodes the UserOp fields using Keccak256.
 *    - Packs the hash, entry point, and chain ID for final transaction hashing.
 *    - Returns the final Keccak256 hash of the packed transaction.
 * 
 * 4. Keccak256:
 *    - Computes a Keccak256 hash for a given hexadecimal input.
 * 
 * 5. HexToByteArray:
 *    - Converts a hexadecimal string to a byte array for cryptographic operations.
 * 
 * 6. HexToBigInteger:
 *    - Converts a hexadecimal string to a BigInteger for numerical calculations.
 * 
 * 7. PadHex:
 *    - Ensures that a hexadecimal string has the proper padding for encoding.
 * 
 * The class provides extensive logging to debug and verify transaction signing.
 */

namespace OktoSDK.Features.Transaction
{
    public class UserOpSign
    {
        public static UserOpType SignUserOp(UserOpType userOp, string privateKey, string entryPointAddress, int chainId)
        {

            CustomLogger.Log("entryPointAddress "+ entryPointAddress);
            CustomLogger.Log("privateKey " + privateKey);

            var packedUserOp = GeneratePackedUserOp(userOp);
            CustomLogger.Log($"packedUserOp: {JsonConvert.SerializeObject(packedUserOp, Formatting.Indented)}");

            var hash = GenerateUserOpHash(packedUserOp, entryPointAddress, chainId);
            CustomLogger.Log($"hash: {JsonConvert.SerializeObject(hash, Formatting.Indented)}");

            // Convert hash to bytes
            byte[] messageBytes = HexToByteArray(hash);

            var signer = new EthereumMessageSigner();

            CustomLogger.Log("privateKey ========================================" + privateKey);
            string signature = signer.Sign(messageBytes, privateKey);

            // Ensure 0x prefix
            signature = signature.StartsWith("0x") ? signature : "0x" + signature;

            var signedUserOp = userOp.Clone();
            signedUserOp.signature = signature;

            CustomLogger.Log($"UserOp signed: {JsonConvert.SerializeObject(signedUserOp, Formatting.Indented)}");

            return signedUserOp;
        }

        private static OktoSDK.UserOp.PackedUserOp GeneratePackedUserOp(UserOpType userOp)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(userOp.sender) ||
                string.IsNullOrEmpty(userOp.nonce) ||
                string.IsNullOrEmpty(userOp.callData))
            {
                throw new ArgumentException("Invalid UserOp");
            }

            // Pack gas limits with proper padding
            string accountGasLimits = "0x" +
                PadHex(userOp.verificationGasLimit, 16).Substring(2) +
                PadHex(userOp.callGasLimit, 16).Substring(2);

            // Pack gas fees with proper padding
            string gasFees = "0x" +
                PadHex(userOp.maxFeePerGas, 16).Substring(2) +
                PadHex(userOp.maxPriorityFeePerGas, 16).Substring(2);

            // Pack paymaster data with proper padding
            string paymasterAndData = userOp.paymaster != null ?
                userOp.paymaster +
                PadHex(userOp.paymasterVerificationGasLimit, 16).Substring(2) +
                PadHex(userOp.paymasterPostOpGasLimit, 16).Substring(2) +
                userOp.paymasterData.Substring(2) :
                "0x";

            return new OktoSDK.UserOp.PackedUserOp
            {
                sender = userOp.sender,
                nonce = userOp.nonce,
                initCode = "0x",
                callData = userOp.callData,
                preVerificationGas = userOp.preVerificationGas,
                accountGasLimits = accountGasLimits,
                gasFees = gasFees,
                paymasterAndData = paymasterAndData
            };
        }


        private static string GenerateUserOpHash(OktoSDK.UserOp.PackedUserOp packedUserOp, string entryPointAddress, int chainId)
        {
            var abiEncoder = new ABIEncode();

            string sender = packedUserOp.sender.ToLowerInvariant();
            string nonce = PadHex(packedUserOp.nonce, 32).ToLowerInvariant();
            string initCodeHash = Keccak256(packedUserOp.initCode);
            string callDataHash = Keccak256(packedUserOp.callData);
            string accountGasLimits = PadHex(packedUserOp.accountGasLimits, 32).ToLowerInvariant();
            string preVerificationGas = packedUserOp.preVerificationGas;
            string gasFees = PadHex(packedUserOp.gasFees, 32).ToLowerInvariant();
            string paymasterAndDataHash = Keccak256(packedUserOp.paymasterAndData);

            CustomLogger.Log($"Formatted values:");
            CustomLogger.Log($"sender: {sender}");
            CustomLogger.Log($"nonce: {nonce}");
            CustomLogger.Log($"initCodeHash: {initCodeHash}");
            CustomLogger.Log($"callDataHash: {callDataHash}");
            CustomLogger.Log($"accountGasLimits: {accountGasLimits}");
            CustomLogger.Log($"preVerificationGas: {preVerificationGas}");
            CustomLogger.Log($"gasFees: {gasFees}");
            CustomLogger.Log($"paymasterAndDataHash: {paymasterAndDataHash}");
            CustomLogger.Log($"entryPointAddress: {entryPointAddress}");
            CustomLogger.Log($"chainId: {chainId}");


            byte[] firstPack = abiEncoder.GetABIEncoded(
                new[] {
                    new ABIValue("address", sender),
                    new ABIValue("bytes32", HexToByteArray(nonce)), // Use raw nonce value
                    new ABIValue("bytes32", HexToByteArray(initCodeHash)),
                    new ABIValue("bytes32", HexToByteArray(callDataHash)),
                    new ABIValue("bytes32", HexToByteArray(accountGasLimits)),
                    new ABIValue("uint256", HexToBigInteger(preVerificationGas)),
                    new ABIValue("bytes32", HexToByteArray(gasFees)),
                    new ABIValue("bytes32", HexToByteArray(paymasterAndDataHash))
                }
            );

            // First pack all parameters in the exact order as JavaScript
            string firstPackHex = "0x" + BitConverter.ToString(firstPack).Replace("-", "").ToLowerInvariant();
            CustomLogger.Log($"First pack hex: {firstPackHex}");

            string firstPackHash = Keccak256(firstPackHex);
            CustomLogger.Log($"First pack hash: {firstPackHash}");
            BigInteger chainIdValue = new BigInteger(OktoAuthManager.GetOktoClient().Env.ChainId);
            // Then create the final pack with the hash, entry point, and chain ID
            byte[] finalPack = abiEncoder.GetABIEncoded(
                new[] {
                    new ABIValue("bytes32", HexToByteArray(firstPackHash)),
                    new ABIValue("address", entryPointAddress),
                    new ABIValue("uint256", chainIdValue)
                }
            );

            string finalPackHex = "0x" + BitConverter.ToString(finalPack).Replace("-", "").ToLowerInvariant();
            CustomLogger.Log($"Final pack before hash: {finalPackHex}");
            CustomLogger.Log($"chainIdValue: {chainIdValue}");

            string finalHash = Keccak256(finalPackHex);
            CustomLogger.Log($"Final hash: {finalHash}");
            return finalHash;
        }

        private static string Keccak256(string hex)
        {
            if (hex.StartsWith("0x")) hex = hex.Substring(2);
            byte[] bytes = HexToByteArray(hex);
            var sha3 = new Sha3Keccack();
            var hash = sha3.CalculateHash(bytes);
            return "0x" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
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

        private static BigInteger HexToBigInteger(string hex)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;

            BigInteger result = BigInteger.Zero;
            foreach (char c in hex)
            {
                result *= 16;
                if (c >= '0' && c <= '9')
                    result += c - '0';
                else if (c >= 'a' && c <= 'f')
                    result += c - 'a' + 10;
                else if (c >= 'A' && c <= 'F')
                    result += c - 'A' + 10;
            }
            return result;
        }

        private static string PadHex(string hex, int size)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            // Ensure the hex string is properly padded to the required size
            string paddedHex = hex.PadLeft(size * 2, '0');
            return "0x" + paddedHex;
        }

    }
}
