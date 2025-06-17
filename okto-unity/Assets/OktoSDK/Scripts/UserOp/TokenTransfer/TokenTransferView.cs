using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;
using OktoSDK.Features.Transaction;
using UserOpType = OktoSDK.UserOp.UserOp;
using ExecuteResult = OktoSDK.BFF.ExecuteResult;
using OktoSDK.Auth;

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

        [SerializeField] private TMP_InputField feePayer;

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
            _instance.feePayer.text = string.Empty;
        }

        public async Task<string> ExecuteTokenTransfer(string receiptAddress, BigInteger amount, string network, string tokenAddress)
        {
            var transaction = new OktoSDK.UserOp.TokenTransferIntentParams
            {
                recipientWalletAddress = receiptAddress,
                tokenAddress = tokenAddress,
                amount = amount,
                caip2Id = network
            };

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);

            userOp = SignUserOp(userOp, network);
            userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);

            BFF.JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);


            //clear all inputfield
            OnClose();

            return txHashStr;
        }

        UserOpType userOp;

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

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
                CustomLogger.Log(TransactionConstants.FeePayerAddress);
            }

            var transaction = new OktoSDK.UserOp.TokenTransferIntentParams
            {
                caip2Id = network,
                recipientWalletAddress = receiptAddress.text,
                tokenAddress = tokenAddress.text,
                amount = parsedAmount
            };

            userOp = await CreateUserOp(transaction);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            if (userOp != null)
            {
                signAndExecuteBtn.gameObject.SetActive(true);
            }
        }

        public async void OnUserSignAndExecute()
        {
            Loader.ShowLoader();

            userOp = SignUserOp(userOp, network);
            BFF.JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);
            ResponsePanel.SetResponse(txHashStr);
        }

        public async void OnUserOPExecute()
        {
            BFF.JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

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

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
            }

            string txHashStr = await ExecuteTokenTransfer(receiptAddress.text, parsedAmount, network, tokenAddress.text);
            ResponsePanel.SetResponse(txHashStr);
        }

        async Task<UserOpType> CreateUserOp(OktoSDK.UserOp.TokenTransferIntentParams transaction)
        {
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");


            // Create UserOp
            var userOp = await tokenTransfer.CreateUserOp(
                userSWA: OktoAuthManager.GetSession().UserSWA,
                clientSWA: OktoAuthManager.GetOktoClient().ClientSWA,
                nonce: "0x" + new string('0', 24) + guidString,
                paymasterData: await PaymasterDataGenerator.Generate(
                    clientSWA: OktoAuthManager.GetOktoClient().ClientSWA,
                    clientPrivateKey: OktoAuthManager.GetOktoClientConfig().ClientPrivateKey,
                    nonce: nonce.ToString(),
                    validUntil: DateTime.UtcNow.AddHours(6)
                ),
                transaction: transaction
            );

            return userOp;
        }

        public UserOpType SignUserOp(UserOpType userOp, string chainId)
        {
            var userOpForSigning = userOp.Clone();

            // Sign UserOp
            var signedUserOp = UserOpSign.SignUserOp(
                userOp: userOpForSigning,
                privateKey: OktoAuthManager.GetSession().SessionPrivKey,
                entryPointAddress: OktoAuthManager.GetOktoClient().Env.EntryPointContractAdress,
                chainId: OktoAuthManager.GetOktoClient().Env.ChainId
            );

            userOp.signature = signedUserOp.signature;

            return signedUserOp;
        }

        async Task<BFF.JsonRpcResponse<ExecuteResult>> ExecuteUserOp(UserOpType signedUserOp)
        {
            // Execute UserOp
            BFF.JsonRpcResponse<ExecuteResult> txHash = await UserOpExecute.ExecuteUserOp(signedUserOp, signedUserOp.signature);
            //clear all inputfield
            OnClose();
            return txHash;
        }

        // Example usage method
        public async void TestTokenTransfer()
        {
            string caip2Id = "eip155:137";
            try
            {
                var transferParams = new OktoSDK.UserOp.TokenTransferIntentParams
                {
                    caip2Id = "eip155:137", // Target chain CAIP-2 ID
                    recipientWalletAddress = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                    tokenAddress = "",   // Token address ("" for native token)
                    amount = 1  // Target chain CAIP-2 ID
                };

                string txHashStr = await ExecuteTokenTransfer(transferParams.recipientWalletAddress, transferParams.amount, caip2Id, transferParams.tokenAddress);

                ResponsePanel.SetResponse(txHashStr);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

    }
}