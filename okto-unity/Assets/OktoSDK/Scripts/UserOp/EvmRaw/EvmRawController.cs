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

        private readonly FunctionCallEncoder encoder;

        public EVMRawController()
        {
            encoder = new FunctionCallEncoder();
        }

        // Step 1: Create Transaction
        public Transaction CreateTransaction(string from, string to, string value, string data = "0x")
        {
            return new Transaction
            {
                from = from,
                to = to,
                data = data,
                value = value,
            };
        }

        public static EVMRawController _instance;

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


        // Step 2: Encode Job Parameters
        public byte[] EncodeJobParameters(Transaction transaction, string caip2Id)
        {
            // Ensure consistent JSON formatting
            var txParams = new
            {
                from = transaction.from,
                to = transaction.to,
                data = transaction.data,
                value = transaction.value
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
            Transaction transaction = null,
            string intentType = "RAW_TRANSACTION",
            Dictionary<string, object> intentData = null)
        {
            try
            {
                byte[] finalEncodedJobParameters = EncodeJobParameters(transaction,currentChain.caipId);
                byte[] encodedGSNData = EncodeGSNData();
                byte[] encodedPolicyInfo;
                if (currentChain == null)
                {
                    //if current chain is not set then set false
                    encodedPolicyInfo = EncodePolicyInfo(false, false);
                }
                else
                {
                    encodedPolicyInfo = EncodePolicyInfo(currentChain.gsnEnabled, currentChain.sponsorshipEnabled);
                }
                // Calculate initiateJob selector
                string functionSignature = $"{Constants.FUNCTION_NAME}({string.Join(",", INTENT_ABI.Select(p => p.Type))})";
                byte[] hashBytes = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(functionSignature));
                string initiateJobSelector = "0x" + NethereumHex.ToHex(hashBytes).Substring(0, 8).ToLowerInvariant();

                // Convert nonce to jobId properly
                string cleanNonce = nonce.StartsWith("0x") ? nonce.Substring(2) : nonce;
                byte[] nonceBytes = HexToByteArray(cleanNonce.PadLeft(64, '0'));
                BigInteger jobId = new BigInteger(nonceBytes.Reverse().ToArray(), isUnsigned: true);

                CustomLogger.Log($"Using jobId: {jobId}");

                // Encode initiateJob parameters
                byte[] initiateJobParamsData = encoder.EncodeParameters(
                    INTENT_ABI,
                    new object[] {
                    jobId,  // Use the properly converted jobId
                    clientSWA,
                    userSWA,
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
                    OktoAuthExample.getOktoClient().Env.JobManagerAddress,
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

        // Step 6: Create UserOp
        public UserOp CreateUserOp(
            string userSWA,
            string clientSWA,
            string nonce,
            string paymasterData,
            Transaction transaction = null)
        {
            string callData = GenerateCallData(userSWA, clientSWA, nonce, transaction);

            // Ensure nonce is properly formatted as 32 bytes
            string paddedNonce = nonce.StartsWith("0x") ? nonce : "0x" + nonce;
            paddedNonce = PadHex(paddedNonce, 32).ToLowerInvariant();

            return new UserOp
            {
                sender = userSWA,
                nonce = paddedNonce,
                paymaster = OktoAuthExample.getOktoClient().Env.PaymasterAddress,
                callData = callData,
                paymasterData = paymasterData
                // signature is not set here, will be null by default
            };
        }

        // Helper Methods
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
            return "0x" + hex.PadLeft(size * 2, '0');
        }

    }
}