using System;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OktoSDK.Features.Transaction;
using UserOpType = OktoSDK.UserOp.UserOp;
using NFTTransferIntentParamsType = OktoSDK.UserOp.NFTTransferIntentParams;
using ExecuteResult = OktoSDK.BFF.ExecuteResult;
using OktoSDK.Auth;

//This script deals with visble ui components in NFT transfer Screen
//It takes those params and feed them to NftTransfer Responsible for encoding of data.
namespace OktoSDK
{
    public class NftTransferView : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField collectionAddress;

        [SerializeField]
        private TMP_InputField nftId;

        [SerializeField]
        private TMP_InputField receiptAddress;

        [SerializeField]
        private TMP_InputField amount;

        [SerializeField]
        private TMP_InputField nftType;

        [SerializeField] 
        private TMP_InputField feePayer;

        [SerializeField]
        private Button signAndExecuteBtn;

        public static NftTransferView _instance;

        public NftTransferController nftTransfer;

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
            _instance.collectionAddress.text = string.Empty;
            _instance.nftId.text = string.Empty;
            _instance.amount.text = string.Empty;
            _instance.nftType.text = string.Empty;
            _instance.receiptAddress.text = string.Empty;
            _instance.feePayer.text = string.Empty;
        }


        public async Task<string> ExecuteNFTTransaction(string recipientWalletAddress,
            string collectionAddress,
            string nftId, BigInteger amount, string type, string network)
        {
            var data = new NFTTransferIntentParamsType
            {
                recipientWalletAddress = recipientWalletAddress,
                collectionAddress = collectionAddress,
                nftId = nftId,
                amount = amount.ToString(),
                nftType = type,
                caip2Id = network
            };

            userOp = await CreateUserOp(data);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            CustomLogger.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

            userOp = SignUserOp(userOp);
            CustomLogger.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

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

            if (string.IsNullOrEmpty(collectionAddress.text))
            {
                ResponsePanel.SetResponse("collectionAddress is required");
                return;
            }

            if (string.IsNullOrEmpty(nftId.text))
            {
                ResponsePanel.SetResponse("nftId is required");
                return;
            }

            if (string.IsNullOrEmpty(receiptAddress.text))
            {
                ResponsePanel.SetResponse("receiptAddress is required");
                return;
            }

            if (string.IsNullOrEmpty(amount.text))
            {
                ResponsePanel.SetResponse("amount is required");
                return;
            }

            if (string.IsNullOrEmpty(nftType.text))
            {
                ResponsePanel.SetResponse("nftType is required");
                return;
            }

            int parsedAmount;
            if (int.TryParse(amount.text, out parsedAmount))
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

            CustomLogger.Log("parsedAmount " + parsedAmount);
            var transferParams = new NFTTransferIntentParamsType
            {
                caip2Id = network,
                recipientWalletAddress = receiptAddress.text,
                collectionAddress = collectionAddress.text,
                nftId = nftId.text,
                amount = parsedAmount.ToString(),
                nftType = nftType.text
            };

            userOp = await CreateUserOp(transferParams);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            ResponsePanel.SetResponse(userOpStr);
            CustomLogger.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
            if (userOp != null)
            {
                signAndExecuteBtn.gameObject.SetActive(true);
            }

        }

        public async void OnUserSignAndExecute()
        {
            Loader.ShowLoader();

            userOp = SignUserOp(userOp);
            CustomLogger.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
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

            if (string.IsNullOrEmpty(collectionAddress.text))
            {
                ResponsePanel.SetResponse("collectionAddress is required");
                return;
            }

            if (string.IsNullOrEmpty(nftId.text))
            {
                ResponsePanel.SetResponse("nftId is required");
                return;
            }

            if (string.IsNullOrEmpty(receiptAddress.text))
            {
                ResponsePanel.SetResponse("receiptAddress is required");
                return;
            }

            if (string.IsNullOrEmpty(amount.text))
            {
                ResponsePanel.SetResponse("amount is required");
                return;
            }

            if (string.IsNullOrEmpty(nftType.text))
            {
                ResponsePanel.SetResponse("nftType is required");
                return;
            }

            BigInteger amountParsed;
            if (BigInteger.TryParse(amount.text, out amountParsed))
            {
                if (amountParsed <= 0)
                {
                    CustomLogger.LogError("Value is required");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
            }

            string txHashStr = await ExecuteNFTTransaction(receiptAddress.text, collectionAddress.text, nftId.text, amountParsed, nftType.text, network);
            ResponsePanel.SetResponse(txHashStr);
        }


        async Task<UserOpType> CreateUserOp(NFTTransferIntentParamsType transaction)
        {
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            CustomLogger.Log($"Generated nonce : {nonce}");

            CustomLogger.Log("Step 1: Creating UserOp");
            // Create UserOp
            var userOp = await nftTransfer.CreateUserOp(
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

        public UserOpType SignUserOp(UserOpType userOp)
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


        // Test NFT Transfer by Default Values
        public async void TestNFTTransfer()
        {
            Loader.ShowLoader();
            var transferParams = new NFTTransferIntentParamsType
            {
                caip2Id = network,
                recipientWalletAddress = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                collectionAddress = "0x9501f6020b0cf374918ff3ea0f2817f8fbdd0762",
                nftId = "7",
                nftType = "ERC1155",
                amount = "1"
            };

            string txHashStr = await ExecuteNFTTransaction(
                transferParams.recipientWalletAddress,
                transferParams.collectionAddress,
                transferParams.nftId,
                System.Numerics.BigInteger.Parse(transferParams.amount),
                transferParams.nftType,
                network);

            CustomLogger.Log("Starting TestTokenTransfer");

            CustomLogger.Log($"Transaction executed. Hash: {txHashStr}");
            ResponsePanel.SetResponse(txHashStr);
        }
    }
}