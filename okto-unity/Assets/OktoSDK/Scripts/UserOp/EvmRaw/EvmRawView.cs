using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

//This script deals with visble ui components in EVM raw transaction Screen
//It takes those params and feed them to UnityCallDecoder Responsible for encoding of data.

namespace OktoSDK
{
    public class EvmRawView : MonoBehaviour
    {

        [SerializeField]
        private TMP_InputField sender;

        [SerializeField]
        private TMP_InputField receipent;

        [SerializeField]
        private TMP_InputField value;

        [SerializeField]
        private TMP_InputField data;

        public EVMRawController unityCallDataEncoder;

        public static EvmRawView _instance;

        private string network;

        public static void SetNetwork(string network)
        {
            _instance.network = network;
        }

        private void OnEnable()
        {
            _instance = this;
        }

        public static void OnClose()
        {
            _instance.receipent.text = string.Empty;
            _instance.value.text = string.Empty;
            _instance.data.text = string.Empty;
        }

        public static string GetNetwork()
        {
            return _instance.network;
        }

        public async Task<string> ExecuteEvmRawTransaction(string network, string sender, string receipent, string value, string data)
        {
            string hexValue = string.Empty;

            try
            {
                 hexValue = string.IsNullOrEmpty(value) ? "0x" : ToHex(value);
            }
            catch (Exception ex)
            {
                return "Invalid Amount!";
            }

            var transaction = unityCallDataEncoder.CreateTransaction(
                from: sender,
                to: receipent,
                data: string.IsNullOrEmpty(data) ? "0x" : data,
                value: hexValue
            );

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            CustomLogger.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");


            userOp = SignUserOp(userOp, network);
            userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            CustomLogger.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            CustomLogger.Log($"Transaction executed. Hash: {txHashStr}");

            //clear all inputfield
            OnClose();
            return txHashStr;
        }

        UserOp userOp;

        public async void OnUserCreate()
        {
            Loader.ShowLoader();

            if (string.IsNullOrEmpty(sender.text))
            {
                ResponsePanel.SetResponse("Sender address is required");
                return;
            }

            if (string.IsNullOrEmpty(receipent.text))
            {
                ResponsePanel.SetResponse("Recipient address is required");
                return;
            }

            if (string.IsNullOrEmpty(value.text))
            {
                ResponsePanel.SetResponse("Value is required");
                return;
            }

            string hexValue = string.Empty;

            try
            {
                hexValue = string.IsNullOrEmpty(value.text) ? "0x" : ToHex(value.text);
            }
            catch (Exception ex)
            {
                ResponsePanel.SetResponse("Invalid Amount!");
                return;
            }

            var transaction = unityCallDataEncoder.CreateTransaction(
                from: sender.text,
                to: receipent.text,
                data: string.IsNullOrEmpty(data.text) ? "0x" : data.text,  // No data for simple transfer
                value: hexValue
            );

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            CustomLogger.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
        }

        public void OnUserSign()
        {
            Loader.ShowLoader();

            userOp = SignUserOp(userOp, network);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            CustomLogger.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
        }
        public async void OnUserOPExecute()
        {
            Loader.ShowLoader();

            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            CustomLogger.Log($"Transaction executed. Hash: {txHashStr}");

            ResponsePanel.SetResponse(txHashStr);
        }

        // Call this from a UI button or other event
        public async void OnTransactionButtonClick()
        {
            Loader.ShowLoader();

            if (string.IsNullOrEmpty(sender.text))
            {
                ResponsePanel.SetResponse("Sender address is required");
                return;
            }

            if (string.IsNullOrEmpty(receipent.text))
            {
                ResponsePanel.SetResponse("Recipient address is required");
                return;
            }

            if (string.IsNullOrEmpty(value.text))
            {
                ResponsePanel.SetResponse("Value is required");
                return;
            }


            CustomLogger.Log("Transaction button clicked");
            string txHashStr = await ExecuteEvmRawTransaction(network, sender.text, receipent.text, value.text, data.text);
            ResponsePanel.SetResponse(txHashStr);

        }

        public BffClientRepository bffClientResitory;

        async Task<UserOp> CreateUserOp(Transaction transaction)
        {
            // Generate nonce using Guid
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            CustomLogger.Log($"Generated fresh nonce : {nonce}");
            CustomLogger.Log($"oktoAuthExample._oktoClient.ClientSWA: {OktoAuthExample.getOktoClient().ClientSWA}");
            CustomLogger.Log("Step 1: Creating UserOp");
            // Create UserOp
            var userOp = unityCallDataEncoder.CreateUserOp(
                userSWA: OktoAuthExample.getOktoClient()._sessionConfig.UserSWA,
                clientSWA: OktoAuthExample.getOktoClient().ClientSWA,
                nonce: "0x" + new string('0', 24) + guidString,
                paymasterData: await PaymasterDataGenerator.Generate(
                    clientSWA: OktoAuthExample.getOktoClient().ClientSWA,
                    clientPrivateKey: OktoAuthExample.getOktoClientConfig().ClientPrivateKey,
                    nonce: nonce.ToString(),
                    validUntil: DateTime.UtcNow.AddHours(6)
                ),
                transaction: transaction
            );

            string txHashStr = JsonConvert.SerializeObject(transaction, Formatting.Indented);
            CustomLogger.Log($"NFT transfer transaction executed. Hash: {txHashStr}");
            return userOp;
        }

        public UserOp SignUserOp(UserOp userOp, string chainId)
        {
            var userOpForSigning = userOp.Clone();

            // Sign UserOp
            var signedUserOp = UserOpSign.SignUserOp(
                userOp: userOpForSigning,
                privateKey: OktoAuthExample.GetSession().SessionPrivKey,
                entryPointAddress: OktoAuthExample.getOktoClient().Env.EntryPointContractAdress,
                chainId: OktoAuthExample.getOktoClient().Env.ChainId
            );

            userOp.signature = signedUserOp.signature;
            return signedUserOp;
        }


        async Task<JsonRpcResponse<ExecuteResult>> ExecuteUserOp(UserOp signedUserOp)
        {
            // Execute UserOp
            JsonRpcResponse<ExecuteResult> txHash = await UserOpExecute.ExecuteUserOp(signedUserOp, signedUserOp.signature);
            //clear all inputfield
            OnClose();
            return txHash;
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


        //Test cases--------------
        // Example usage method
        //Note :- Input your own details to test manually
        public async void TestTokenTransfer()
        {
            try
            {
                var transferParams = new Transaction
                {
                    from = "0xc3AC3F050CCa482CF6F53070541A7B61A71C4abd",
                    to = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                    data = "0x",
                    value = ToHex(1)
                };

                string txHashStr = await ExecuteEvmRawTransaction(
                    "eip155:137",
                    transferParams.from,
                    transferParams.to,
                    transferParams.value,
                    transferParams.data);

                CustomLogger.Log($"Transaction executed. Hash: {txHashStr}");
                CustomLogger.Log("Starting TestTokenTransfer");

                ResponsePanel.SetResponse(txHashStr);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

    }

}