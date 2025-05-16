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
using OktoSDK.UserOp;
using UserOpType = OktoSDK.UserOp.UserOp;


/*
 * EVMRawController - A controller for encoding and managing transactions.
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
    public class EVMRawController : MonoBehaviour
    {
        private readonly FunctionCallEncoder encoder;

        public EVMRawController()
        {
            encoder = new FunctionCallEncoder();
        }

        // Step 1: Create Transaction
        public OktoSDK.UserOp.Transaction CreateTransaction(string from, string to, string value, string data = "0x")
        {
            return new OktoSDK.UserOp.Transaction
            {
                From = from,
                To = to,
                Data = data,
                Value = value,
            };
        }

        public static EVMRawController _instance;

        private void OnEnable()
        {
            _instance = this;
        }


        // Step 2: Encode Job Parameters
        public byte[] EncodeJobParameters(OktoSDK.UserOp.Transaction transaction, string caip2Id)
        {
            // Ensure consistent JSON formatting
            var txParams = new
            {
                from = transaction.From,
                to = transaction.To,
                data = transaction.Data,
                value = transaction.Value
            };

            var transactionJson = JsonConvert.SerializeObject(txParams, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.Default,
                Formatting = Formatting.None
            });


            byte[] transactionBytes = Encoding.UTF8.GetBytes(transactionJson);

            var jobParamsStruct = new
            {
                caip2Id = caip2Id,
                transactions = new[] { transactionBytes }
            };

            CustomLogger.Log($"Generated jobParamsStruct: {JsonConvert.SerializeObject(jobParamsStruct, Formatting.Indented)}");

            byte[] encodedJobParameters = encoder.EncodeParameters(
                new[] {
                new Parameter("string", "caip2Id", 1),
                new Parameter("bytes[]", "transactions", 2)
                },
                new object[] { jobParamsStruct.caip2Id, jobParamsStruct.transactions }
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

        // Step 5: Generate Call Data
        public string GenerateCallData(
            string userSWA,
            string clientSWA,
            string nonce,
            OktoSDK.UserOp.Transaction transaction = null,
            string intentType = "RAW_TRANSACTION",
            Dictionary<string, object> intentData = null)
        {
            try
            {
                byte[] finalEncodedJobParameters = EncodeJobParameters(transaction, TransactionConstants.CurrentChain.caipId);
                byte[] encodedGSNData = EncodeGSNData();
                byte[] encodedPolicyInfo;
                if (TransactionConstants.CurrentChain == null)
                {
                    //if current chain is not set then set false
                    encodedPolicyInfo = EncodePolicyInfo(false, false);
                }
                else
                {
                    encodedPolicyInfo = EncodePolicyInfo(TransactionConstants.CurrentChain.gsnEnabled, TransactionConstants.CurrentChain.sponsorshipEnabled);
                }
                // Calculate initiateJob selector
                string functionSignature = $"{Constants.FUNCTION_NAME}({string.Join(",", TransactionConstants.INTENT_ABI.Select(p => p.Type))})";
                byte[] hashBytes = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(functionSignature));
                string initiateJobSelector = "0x" + NethereumHex.ToHex(hashBytes).Substring(0, 8).ToLowerInvariant();

                // Convert nonce to jobId properly
                string cleanNonce = nonce.StartsWith("0x") ? nonce.Substring(2) : nonce;
                byte[] nonceBytes = HexToByteArray(cleanNonce.PadLeft(64, '0'));
                BigInteger jobId = new BigInteger(nonceBytes.Reverse().ToArray(), isUnsigned: true);

                CustomLogger.Log($"Using jobId: {jobId}");

                // Encode initiateJob parameters
                byte[] initiateJobParamsData = encoder.EncodeParameters(
                    TransactionConstants.INTENT_ABI,
                    new object[] {
                    jobId,  // Use the properly converted jobId
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

                // Final encoding for executeUserOp
                byte[] executeUserOpParams = encoder.EncodeParameters(
                    new[] {
                    new Parameter("bytes4", "functionSelector"),
                    new Parameter("address", "jobManager"),
                    new Parameter("uint256", "value"),
                    new Parameter("bytes", "data")
                    },
                    new object[] {
                    HexToByteArray(Constants.EXECUTE_USEROP_FUNCTION_SELECTOR),
                    EnvironmentHelper.GetJobManagerAddress(),
                    BigInteger.Zero,
                    initiateJobData
                    }
                );

                return "0x" + BitConverter.ToString(executeUserOpParams).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error generating calldata: {ex.Message}");
                CustomLogger.LogError($"Nonce used: {nonce}");
                throw;
            }
        }
        public async Task<UserOpType> CreateUserOp(
            string userSWA,
            string clientSWA,
            string nonce,
            string paymasterData,
            OktoSDK.UserOp.Transaction transaction = null)
        {
            string callData = GenerateCallData(userSWA, clientSWA, nonce, transaction);

            // Ensure nonce is properly formatted as 32 bytes
            string paddedNonce = nonce.StartsWith("0x") ? nonce : "0x" + nonce;
            paddedNonce = PadHex(paddedNonce, 32).ToLowerInvariant();

            // Fetch gas price info
            UserOperationGasPriceResult gasPriceResult = (await TransactionConstants.GetUserOperationGasPriceAsync()).Result;

            CustomLogger.Log("EnvironmentHelper.GetPaymasterAddress() " + EnvironmentHelper.GetPaymasterAddress());

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

        // Helper Methods
        private byte[] AddOffset(byte[] data)
        {
            // Create a byte array with 32 bytes for the offset and then the data
            byte[] result = new byte[32 + data.Length];
            
            // Set the offset at position 0x20 (32 in decimal)
            BigInteger offset = 32;
            byte[] offsetBytes = offset.ToByteArray().Reverse().ToArray();
            
            // Pad with zeros to 32 bytes
            int padLength = 32 - offsetBytes.Length;
            for (int i = 0; i < offsetBytes.Length; i++)
            {
                result[padLength + i] = offsetBytes[i];
            }
            
            // Copy data after the offset
            Array.Copy(data, 0, result, 32, data.Length);
            
            return result;
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

        private string PadHex(string hex, int size)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            // Ensure the hex string is properly padded to the required size
            string paddedHex = hex.PadLeft(size * 2, '0');
            return "0x" + paddedHex;
        }
    }
}