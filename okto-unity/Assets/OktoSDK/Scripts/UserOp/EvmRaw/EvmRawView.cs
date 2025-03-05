using System;
using System.Numerics;
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

        public async Task<string> ExecuteSimpleTransaction(string network, string sender, string receipent, string value, string data)
        {
            string hexValue = string.IsNullOrEmpty(value) ? "0x00" : ToHex(value);

            var transaction = unityCallDataEncoder.CreateTransaction(
                from: sender,
                to: receipent,
                data: string.IsNullOrEmpty(data) ? "0x00" : data,
                value: string.IsNullOrEmpty(value) ? "0x00" : hexValue
            );

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            Debug.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");


            userOp = SignUserOp(userOp, network);
            userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            Debug.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            Debug.Log($"Transaction executed. Hash: {txHashStr}");

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

            var transaction = unityCallDataEncoder.CreateTransaction(
                from: sender.text,
                to: receipent.text,
                data: string.IsNullOrEmpty(data.text) ? "0x00" : data.text,  // No data for simple transfer
                value: ToHex(value.text)
            );

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            Debug.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
        }

        public void OnUserSign()
        {
            Loader.ShowLoader();

            userOp = SignUserOp(userOp, network);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            Debug.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
        }
        public async void OnUserOPExecute()
        {
            Loader.ShowLoader();

            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            Debug.Log($"Transaction executed. Hash: {txHashStr}");

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

            Debug.Log("Transaction button clicked");
            string txHashStr = await ExecuteSimpleTransaction(network, sender.text, receipent.text, value.text, data.text);
            ResponsePanel.SetResponse(txHashStr);

        }

        public BffClientRepository bffClientResitory;

        async Task<UserOp> CreateUserOp(Transaction transaction)
        {
            // Generate nonce using Guid
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            Debug.Log($"Generated fresh nonce : {nonce}");
            Debug.Log($"oktoAuthExample._oktoClient.ClientSWA: {OktoAuthExample.getOktoClient().ClientSWA}");
            Debug.Log("Step 1: Creating UserOp");
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
            Debug.Log($"NFT transfer transaction executed. Hash: {txHashStr}");
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
                    data = "",
                    value = "1"
                };

                Debug.Log("Starting TestTokenTransfer");
                string txHashStr = await ExecuteSimpleTransaction(
                    "eip155:137",
                    transferParams.from,
                    transferParams.to,
                    transferParams.value,
                    transferParams.data);

                Debug.Log($"Transaction executed. Hash: {txHashStr}");

                ResponsePanel.SetResponse(txHashStr);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

        //Test cases--------------
        // Example usage method
        //Note :- Input your own details to test manually
        public async void TestContractRead()
        {
            var transferParams = new Transaction
            {
                from = "",
                to = "",
                data = "0x18160ddd",
                value = ""
            };

            Debug.Log("Starting TestTokenTransfer");
            string txHashStr = await ExecuteSimpleTransaction(
                "eip155:137",
                transferParams.from,
                transferParams.to,
                transferParams.value,
                transferParams.data);

            Debug.Log($"Transaction executed. Hash: {txHashStr}");

            ResponsePanel.SetResponse(txHashStr);

        }

    }

}