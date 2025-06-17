using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UserOpType = OktoSDK.UserOp.UserOp;

namespace OktoSDK
{
    public class EvmRawView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField sender;
        [SerializeField] private TMP_InputField receipent;
        [SerializeField] private TMP_InputField value;
        [SerializeField] private TMP_InputField data;
        [SerializeField] private TMP_InputField feePayer;
        [SerializeField] private EvmRawPrefab evmRawPrefab;

        public static EvmRawView _instance;
        private string network;

        private UserOpType userOp;  // to store the created UserOp

        private void OnEnable()
        {
            _instance = this;
        }

        public static void SetNetwork(string network)
        {
            _instance.network = network;
        }

        public static string GetNetwork()
        {
            return _instance.network;
        }

        public static void OnClose()
        {
            _instance.receipent.text = string.Empty;
            _instance.value.text = string.Empty;
            _instance.data.text = string.Empty;
            _instance.feePayer.text = string.Empty;
        }

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

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
            }

            string txHashStr = await evmRawPrefab.CallEvmRaw(
                sender.text,
                receipent.text,
                value.text,
                data.text
            );

            ResponsePanel.SetResponse(txHashStr);
        }

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

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
            }

            string hexValue = string.Empty;

            try
            {
                hexValue = EvmRawManager.ToHex(value.text);
            }
            catch
            {
                ResponsePanel.SetResponse("Invalid Amount!");
                return;
            }

            if (!string.IsNullOrEmpty(feePayer.text))
            {
                TransactionConstants.FeePayerAddress = feePayer.text;
            }

            var tx = evmRawPrefab.EvmRawManager.evmRawController.CreateTransaction(
                from: sender.text,
                to: receipent.text,
                data: string.IsNullOrEmpty(data.text) ? "0x" : data.text,
                value: hexValue
            );

            userOp = await evmRawPrefab.EvmRawManager.CreateUserOp(tx);

            ResponsePanel.SetResponse(JsonConvert.SerializeObject(userOp, Formatting.Indented));
        }

        public void OnUserSign()
        {
            if (userOp == null)
            {
                ResponsePanel.SetResponse("Please create a UserOp first.");
                return;
            }

            Loader.ShowLoader();

            userOp = evmRawPrefab.EvmRawManager.SignUserOp(userOp);

            ResponsePanel.SetResponse(JsonConvert.SerializeObject(userOp, Formatting.Indented));
        }

        public async void OnUserOPExecute()
        {
            if (userOp == null)
            {
                ResponsePanel.SetResponse("Please create and sign a UserOp first.");
                return;
            }

            Loader.ShowLoader();

            var result = await evmRawPrefab.EvmRawManager.ExecuteUserOp(userOp, userOp.signature);

            ResponsePanel.SetResponse("Execution Result:\n" + JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
