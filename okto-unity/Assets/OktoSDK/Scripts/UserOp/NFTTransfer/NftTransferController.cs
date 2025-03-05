using System;
using System.Numerics;
using System.Text;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Util;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using NethereumHex = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions;
using System.Collections.Generic;

/*
 * NftTransferController - A controller for encoding and managing transactions.
 * 
 * This class provides methods to:
 * 1. Create Ethereum transactions with specified parameters.
 * 2. Encode job parameters using JSON serialization and ABI encoding.
 * 3. Encode Gas Station Network (GSN) data for transaction sponsorship.
 * 4. Encode policy information for smart contract interactions.
 * 5. Generate calldata for contract execution using function selectors and encoded parameters.
 * 6. Create a UserOp (User Operation) for transaction execution with nonce handling.
 * 
 * Key Components:
 * - `CreateTransaction()`: Generates a transaction object.
 * - `EncodeJobParameters()`: Encodes transaction details into a structured byte array.
 * - `EncodeGSNData()`: Prepares data for gas sponsorship.
 * - `EncodePolicyInfo()`: Stores GSN and sponsorship enablement status.
 * - `GenerateCallData()`: Generates call data for contract execution.
 * - `CreateUserOp()`: Constructs a UserOp with calldata and paymaster details.
 * 
 * Helper Functions:
 * - `AddOffset()`: Adds an offset to encoded data for correct EVM structure.
 * - `HexToByteArray()`: Converts a hex string to a byte array.
 * - `PadHex()`: Ensures proper byte alignment for nonce formatting.
 * 
 * The class ensures transaction integrity and proper formatting for blockchain interactions.
 */
namespace OktoSDK
{
    public class NftTransferController : MonoBehaviour
    {
        // Define the ABI
        private static readonly Parameter[] INTENT_ABI = new[]
        {
        new Parameter("uint256", "_jobId"),
        new Parameter("address", "_clientSWA"),
        new Parameter("address", "_jobCreatorId"),
        new Parameter("bytes", "_policyInfo"),
        new Parameter("bytes", "_gsnData"),
        new Parameter("bytes", "_jobParameters"),
        new Parameter("string", "_intentType")
    };

        public static NftTransferController _instance;

        public NetworkData currentChain;

        public static void SetCurrentChain(NetworkData newChain)
        {
            _instance.currentChain = new NetworkData
            {
                caipId = newChain.caipId,
                networkName = newChain.networkName,
                chainId = newChain.chainId,
                logo = newChain.logo,
                sponsorshipEnabled = newChain.sponsorshipEnabled,
                gsnEnabled = newChain.gsnEnabled,
                type = newChain.type,
                networkId = newChain.networkId,
                onRampEnabled = newChain.onRampEnabled,
                whitelisted = newChain.whitelisted
            };
        }

        private void OnEnable()
        {
            _instance = this;
        }

        private readonly FunctionCallEncoder encoder;

        public NftTransferController()
        {
            encoder = new FunctionCallEncoder();
        }

        // Step 2: Encode Job Parameters
        public byte[] EncodeJobParameters(NFTTransferIntentParams transaction)
        {
            Debug.Log(JsonConvert.SerializeObject(transaction, Formatting.Indented));
            BigInteger bigNumber = transaction.amount;
            byte[] encodedJobParameters = encoder.EncodeParameters(
                new[] {
                new Parameter("string", "caip2Id", 1),
                new Parameter("string", "nftId", 2),
                new Parameter("string", "recipientWalletAddress", 3),
                new Parameter("string", "collectionAddress", 4),
                new Parameter("string", "nftType", 5),
                new Parameter("uint", "amount", 6)
                },
                new object[] { transaction.caip2Id, transaction.nftId, transaction.recipientWalletAddress, transaction.collectionAddress, transaction.nftType, bigNumber }
            );

            return AddOffset(encodedJobParameters);
        }

        // Step 3: Encode GSN Data
        public byte[] EncodeGSNData(bool isRequired = false, string[] requiredNetworks = null, byte[][] tokens = null)
        {
            requiredNetworks ??= Array.Empty<string>();
            tokens ??= Array.Empty<byte[]>();

            byte[] encodedGSNParams = encoder.EncodeParameters(
                new[] {
                new Parameter("bool", "isRequired"),
                new Parameter("string[]", "requiredNetworks"),
                new Parameter("bytes[]", "tokens")
                },
                new object[] { isRequired, requiredNetworks, tokens }
            );

            return AddOffset(encodedGSNParams);
        }

        // Step 4: Encode Policy Info
        public byte[] EncodePolicyInfo(bool gsnEnabled, bool sponsorshipEnabled)
        {
            return encoder.EncodeParameters(
                new[] {
                new Parameter("bool", "gsnEnabled"),
                new Parameter("bool", "sponsorshipEnabled")
                },
                new object[] { gsnEnabled, sponsorshipEnabled }
            );
        }

        private byte[] AddOffset(byte[] data)
        {
            byte[] result = new byte[data.Length + 32];
            byte[] offsetBytes = new byte[32];
            BitConverter.GetBytes(32).CopyTo(offsetBytes, 0);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(offsetBytes);

            Array.Copy(offsetBytes, 0, result, 0, 32);
            Array.Copy(data, 0, result, 32, data.Length);
            return result;
        }

        // Step 5: Generate Call Data
        public string GenerateCallData(
            string userSWA,
            string clientSWA,
            string nonce,
            NFTTransferIntentParams transaction = null,
            string intentType = "NFT_TRANSFER",
            Dictionary<string, object> intentData = null)
        {
            byte[] finalEncodedJobParameters = EncodeJobParameters(transaction);
            byte[] encodedGSNData = EncodeGSNData();
            byte[] encodedPolicyInfo;
            if (currentChain == null)
            {
                encodedPolicyInfo = EncodePolicyInfo(false, false);
            }
            else
            {
                encodedPolicyInfo = EncodePolicyInfo(currentChain.gsnEnabled, currentChain.sponsorshipEnabled);
            }

            string functionSignature = $"{Constants.FUNCTION_NAME}({string.Join(",", INTENT_ABI.Select(p => p.Type))})";
            byte[] hashBytes = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(functionSignature));
            string initiateJobSelector = "0x" + NethereumHex.ToHex(hashBytes).Substring(0, 8).ToLowerInvariant();

            string cleanNonce = nonce.StartsWith("0x") ? nonce.Substring(2) : nonce;
            byte[] nonceBytes = HexToByteArray(cleanNonce.PadLeft(64, '0'));
            BigInteger jobId = new BigInteger(nonceBytes.Reverse().ToArray(), isUnsigned: true);

            byte[] initiateJobParamsData = encoder.EncodeParameters(
                INTENT_ABI,
                new object[] {
                    jobId,
                    clientSWA,
                    userSWA,
                    encodedPolicyInfo,
                    encodedGSNData,
                    finalEncodedJobParameters,
                    intentType
                }
            );

            byte[] initiateJobData = HexToByteArray(initiateJobSelector).Concat(initiateJobParamsData).ToArray();

            byte[] executeUserOpParams = encoder.EncodeParameters(
                new[] {
                    new Parameter("bytes4", "functionSelector"),
                    new Parameter("address", "jobManager"),
                    new Parameter("uint256", "value"),
                    new Parameter("bytes", "data")
                },
                new object[] {
                    HexToByteArray(Constants.EXECUTE_USEROP_FUNCTION_SELECTOR),
                    OktoAuthExample.getOktoClient().Env.JobManagerAddress,
                    BigInteger.Zero,
                    initiateJobData
                }
            );

            return "0x" + BitConverter.ToString(executeUserOpParams).Replace("-", "").ToLowerInvariant();
        }

        private byte[] HexToByteArray(string hex)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        // Step 6: Create UserOp
        public UserOp CreateUserOp(
            string userSWA,
            string clientSWA,
            string nonce,
            string paymasterData,
            NFTTransferIntentParams transaction = null)
        {
            string callData = GenerateCallData(userSWA, clientSWA, nonce, transaction);

            string paddedNonce = nonce.StartsWith("0x") ? nonce : "0x" + nonce;
            paddedNonce = PadHex(paddedNonce, 32).ToLowerInvariant();

            return new UserOp
            {
                sender = userSWA,
                nonce = paddedNonce,
                paymaster = OktoAuthExample.getOktoClient().Env.PaymasterAddress,
                callData = callData,
                paymasterData = paymasterData
            };
        }

        private string PadHex(string hex, int size)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            return "0x" + hex.PadLeft(size * 2, '0');
        }

        public static string ToHex(object value)
        {
            if (value is int intValue)
            {
                return $"0x{intValue:X}";
            }
            else if (value is BigInteger bigIntValue)
            {
                return $"0x{bigIntValue.ToString("X")}";
            }
            else if (value is string strValue)
            {
                if (BigInteger.TryParse(strValue, out BigInteger bigIntParsed))
                {
                    return $"0x{bigIntParsed.ToString("X")}";
                }
                else
                {
                    throw new ArgumentException($"Invalid string input: {strValue}. It must be a valid number.");
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported type: {value.GetType()}. Use int, BigInteger, or a valid numeric string.");
            }
        }
    }
}