using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;

//This script deals with visble ui components in Token transfer Screen
//It takes those params and feed them to NftTransfer Responsible for encoding of data.
namespace OktoSDK
{
    public class TokenTransferView : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField tokenAddress;

        [SerializeField]
        private TMP_InputField value;

        [SerializeField]
        private TMP_InputField receiptAddress;

        [SerializeField]
        private Button signAndExecuteBtn;

        public TokenTransferController tokenTransfer;

        public static TokenTransferView _instance;

        private string network;

        public static void SetNetwork(string network)
        {
            _instance.network = network;
        }

        public static string GetNetwork()
        {
            return _instance.network;
        }

        private void OnEnable()
        {
            _instance = this;
        }

        public static void OnClose()
        {
            _instance.signAndExecuteBtn.gameObject.SetActive(false);
            _instance.receiptAddress.text = string.Empty;
            _instance.value.text = string.Empty;
            _instance.tokenAddress.text = string.Empty;
        }

        public async Task<string> ExecuteTokenTransfer(string receiptAddress, BigInteger amount, string network, string tokenAddress)
        {
            var transaction = new TokenTransferIntentParams
            {
                recipientWalletAddress = receiptAddress,
                tokenAddress = tokenAddress,
                amount = amount,
                caip2Id = network
            };

            Debug.Log($"Generated transaction: {JsonConvert.SerializeObject(transaction, Formatting.Indented)}");

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

            if (string.IsNullOrEmpty(receiptAddress.text))
            {
                ResponsePanel.SetResponse("ReceiptAddress is required");
                return;
            }

            if (string.IsNullOrEmpty(value.text))
            {
                ResponsePanel.SetResponse("Amount is required");
                return;
            }

            BigInteger parsedAmount;
            if (BigInteger.TryParse(value.text, out parsedAmount))
            {
                if (parsedAmount <= 0)
                {
                    ResponsePanel.SetResponse("Enter valid amount");
                    return;
                }
            }

            var transaction = new TokenTransferIntentParams
            {
                caip2Id = network,
                recipientWalletAddress = receiptAddress.text,
                tokenAddress = tokenAddress.text,
                amount = parsedAmount
            };

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            Debug.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
            if (userOp != null)
            {
                signAndExecuteBtn.gameObject.SetActive(true);
            }
        }

        public async void OnUserSignAndExecute()
        {
            Loader.ShowLoader();

            userOp = SignUserOp(userOp, network);
            Debug.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);
            ResponsePanel.SetResponse(txHashStr);
        }

        public async void OnUserOPExecute()
        {
            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            Debug.Log($"Transaction executed. Hash: {txHashStr}");

            ResponsePanel.SetResponse(txHashStr);
        }

        public async void OnTransactionButtonClick()
        {
            Loader.ShowLoader();

            if (string.IsNullOrEmpty(receiptAddress.text))
            {
                ResponsePanel.SetResponse("Recipient address is required");
                return;
            }

            if (string.IsNullOrEmpty(value.text))
            {
                ResponsePanel.SetResponse("Amount is required");
                return;
            }

            BigInteger parsedAmount;
            if (BigInteger.TryParse(value.text, out parsedAmount))
            {
                if (parsedAmount <= 0)
                {
                    ResponsePanel.SetResponse("Enter valid amount");
                    return;
                }
            }

            Debug.Log("parsedAmount " + parsedAmount);

            string txHashStr = await ExecuteTokenTransfer(receiptAddress.text, parsedAmount, network, tokenAddress.text);
            Debug.Log($"Transaction executed. Hash: {txHashStr}");

            ResponsePanel.SetResponse(txHashStr);
        }

        async Task<UserOp> CreateUserOp(TokenTransferIntentParams transaction)
        {
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            Debug.Log($"Generated nonce : {nonce}");

            Debug.Log("Step 1: Creating UserOp");
            Debug.Log("Sessipn.PrivateKey" + OktoAuthExample.GetSession().SessionPrivKey);


            // Create UserOp
            var userOp = tokenTransfer.CreateUserOp(
                userSWA: OktoAuthExample.GetSession().UserSWA,
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

        // Example usage method
        public async void TestTokenTransfer()
        {
            string caip2Id = "eip155:137";
            try
            {
                var transferParams = new TokenTransferIntentParams
                {
                    caip2Id = "eip155:137", // Target chain CAIP-2 ID
                    recipientWalletAddress = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                    tokenAddress = "",   // Token address ("" for native token)
                    amount = 1000000000000000000  // Target chain CAIP-2 ID
                };

                string txHashStr = await ExecuteTokenTransfer(transferParams.recipientWalletAddress, transferParams.amount, caip2Id, transferParams.tokenAddress);
             
                Debug.Log($"Transaction executed. Hash: {txHashStr}");
                Debug.Log("Starting TestTokenTransfer");

                ResponsePanel.SetResponse(txHashStr);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

    }
}