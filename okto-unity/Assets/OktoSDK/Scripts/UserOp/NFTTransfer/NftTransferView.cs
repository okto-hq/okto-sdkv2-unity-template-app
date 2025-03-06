using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

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
        }


        public async Task<string> ExecuteNFTTransaction(string recipientWalletAddress,
            string collectionAddress,
            string nftId, int amount, string type, string network)
        {
            var data = new NFTTransferIntentParams
            {
                recipientWalletAddress = recipientWalletAddress,
                collectionAddress = collectionAddress,
                nftId = nftId,
                amount = amount,
                nftType = type,
                caip2Id = network
            };

            userOp = await CreateUserOp(data);
            string userOpStr = JsonConvert.SerializeObject(userOp, Formatting.Indented);
            Debug.Log($"UserOp created: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

            userOp = SignUserOp(userOp);
            Debug.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");

            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);

            return txHashStr;
        }

        UserOp userOp;

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

            Debug.Log("parsedAmount " + parsedAmount);
            var transferParams = new NFTTransferIntentParams
            {
                caip2Id = network,
                recipientWalletAddress = receiptAddress.text,
                collectionAddress = collectionAddress.text,
                nftId = nftId.text,
                amount = parsedAmount,
                nftType = nftType.text
            };

            userOp = await CreateUserOp(transferParams);
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

            userOp = SignUserOp(userOp);
            Debug.Log($"UserOp Signed: {JsonConvert.SerializeObject(userOp, Formatting.Indented)}");
            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);
            ResponsePanel.SetResponse(txHashStr);
        }

        public async void OnUserOPExecute()
        {
            JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(userOp);
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

            int parsedAmount;
            if (int.TryParse(amount.text, out parsedAmount))
            {
                if (parsedAmount <= 0)
                {
                    ResponsePanel.SetResponse("Enter valid amount");
                    return;
                }
            }

            int amountParsed;
            if (int.TryParse(amount.text, out amountParsed))
            {
                if (amountParsed <= 0)
                {
                    Debug.LogError("Value is required");
                    return;
                }
            }

            string txHashStr = await ExecuteNFTTransaction(receiptAddress.text, collectionAddress.text, nftId.text, amountParsed, nftType.text, network);
            ResponsePanel.SetResponse(txHashStr);
        }


        async Task<UserOp> CreateUserOp(NFTTransferIntentParams transaction)
        {
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            Debug.Log($"Generated nonce : {nonce}");

            Debug.Log("Step 1: Creating UserOp");
            // Create UserOp
            var userOp = nftTransfer.CreateUserOp(
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

        public UserOp SignUserOp(UserOp userOp)
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


        // Test NFT Transfer by Default Values
        public async void TestNFTTransfer()
        {
            Loader.ShowLoader();
            var transferParams = new NFTTransferIntentParams
            {
                caip2Id = network,
                recipientWalletAddress = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                collectionAddress = "0x9501f6020b0cf374918ff3ea0f2817f8fbdd0762",
                nftId = "7",
                nftType = "ERC1155",
                amount = 1
            };

            string txHashStr = await ExecuteNFTTransaction(
                transferParams.recipientWalletAddress,
                transferParams.collectionAddress,
                transferParams.nftId,
                transferParams.amount,
                transferParams.nftType,
                network);

            Debug.Log("Starting TestTokenTransfer");

            Debug.Log($"Transaction executed. Hash: {txHashStr}");
            ResponsePanel.SetResponse(txHashStr);
        }
    }
}