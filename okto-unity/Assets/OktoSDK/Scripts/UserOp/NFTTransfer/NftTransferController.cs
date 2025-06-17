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
using System.Threading.Tasks;
using OktoSDK.BFF;
using OktoSDK.UserOp;
using UserOpType = OktoSDK.UserOp.UserOp;
using NFTTransferIntentParamsType = OktoSDK.UserOp.NFTTransferIntentParams;
using Nethereum.Util.Keccak;
using OktoSDK.Auth;

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
        public static NftTransferController _instance;

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
        public byte[] EncodeJobParameters(NFTTransferIntentParamsType transaction)
        {
            //BigInteger bigNumber = transaction.amount;
            byte[] encodedJobParameters = encoder.EncodeParameters(
                new[] {
                new Parameter("string", "caip2Id", 1),
                new Parameter("string", "nftId", 2),
                new Parameter("string", "recipientWalletAddress", 3),
                new Parameter("string", "collectionAddress", 4),
                new Parameter("string", "nftType", 5),
                new Parameter("uint", "amount", 6)
                },
                new object[] { transaction.caip2Id, transaction.nftId, transaction.recipientWalletAddress, transaction.collectionAddress, transaction.nftType, transaction.amount }
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
            NFTTransferIntentParamsType transaction = null,
            string intentType = "NFT_TRANSFER",
            Dictionary<string, object> intentData = null)
        {
            byte[] finalEncodedJobParameters = EncodeJobParameters(transaction);
            byte[] encodedGSNData = EncodeGSNData();
            byte[] encodedPolicyInfo;
            if (TransactionConstants.CurrentChain == null)
            {
                encodedPolicyInfo = EncodePolicyInfo(false, false);
            }
            else
            {
                encodedPolicyInfo = EncodePolicyInfo(TransactionConstants.CurrentChain.gsnEnabled, TransactionConstants.CurrentChain.sponsorshipEnabled);
            }

            string functionSignature = $"{Constants.FUNCTION_NAME}({string.Join(",", TransactionConstants.INTENT_ABI.Select(p => p.Type))})";
            byte[] hashBytes = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(functionSignature));
            string initiateJobSelector = "0x" + NethereumHex.ToHex(hashBytes).Substring(0, 8).ToLowerInvariant();

            string cleanNonce = nonce.StartsWith("0x") ? nonce.Substring(2) : nonce;
            byte[] nonceBytes = HexToByteArray(cleanNonce.PadLeft(64, '0'));
            BigInteger jobId = new BigInteger(nonceBytes.Reverse().ToArray(), isUnsigned: true);

            byte[] initiateJobParamsData = encoder.EncodeParameters(
                TransactionConstants.INTENT_ABI,
                new object[] {
                    jobId,
                    clientSWA,
                    userSWA,
                    TransactionConstants.FeePayerAddress,
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
                    OktoAuthManager.GetOktoClient().Env.JobManagerAddress,
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
        public async Task<UserOpType> CreateUserOp(
            string userSWA,
            string clientSWA,
            string nonce,
            string paymasterData,
            NFTTransferIntentParamsType transaction = null)
        {
            string callData = GenerateCallData(userSWA, clientSWA, nonce, transaction);

            string paddedNonce = nonce.StartsWith("0x") ? nonce : "0x" + nonce;
            paddedNonce = PadHex(paddedNonce, 32).ToLowerInvariant();

            // Fetch gas price info
            OktoSDK.UserOp.UserOperationGasPriceResult gasPriceResult = (await TransactionConstants.GetUserOperationGasPriceAsync()).data;

            return new UserOpType
            {
                sender = userSWA,
                nonce = paddedNonce,
                paymaster = EnvironmentHelper.GetPaymasterAddress(),
                callData = callData,
                paymasterData = paymasterData,
                maxFeePerGas = gasPriceResult.maxFeePerGas,
                maxPriorityFeePerGas = gasPriceResult.maxPriorityFeePerGas
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