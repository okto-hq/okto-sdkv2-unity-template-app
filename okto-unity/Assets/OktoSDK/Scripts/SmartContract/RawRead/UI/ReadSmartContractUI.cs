using UnityEngine;
using TMPro;
using OktoSDK.Auth;
using UnityEngine.UI;
using System;

namespace OktoSDK.Features.SmartContract
{
    public class ReadSmartContractUI : MonoBehaviour
    {
        public TMP_InputField networkField;
        public TMP_InputField abiInputField;
        public TMP_InputField contractAddressField;
        public TMP_InputField functionArgsField;
        [SerializeField] private GameObject readContractPanel;
        [SerializeField] private Button readContractBtn;
        [SerializeField] private Button callReadContractBtn;
        [SerializeField] private Button closeButton;
        
        private void OnEnable()
        {
            callReadContractBtn.onClick.AddListener(OnReadContract);
            closeButton.onClick.AddListener(ClosePanel);
            readContractBtn.onClick.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            callReadContractBtn.onClick.RemoveListener(OnReadContract);
            closeButton.onClick.RemoveListener(ClosePanel);
            readContractBtn.onClick.RemoveListener(OpenPanel);
        }

        void OpenPanel()
        {
            if (!EnsureLoggedIn()) return;

            ClearFields();
            readContractPanel.SetActive(true);
        }

        void ClosePanel()
        {
            ClearFields();
            readContractPanel.SetActive(false);
        }

        void ClearFields()
        {
            networkField.text = string.Empty;
            abiInputField.text = string.Empty;
            contractAddressField.text = string.Empty;
            functionArgsField.text = string.Empty;
        }

        private bool EnsureLoggedIn()
        {
            var oc = OktoAuthManager.GetOktoClient();

            if (oc == null || !oc.IsLoggedIn())
            {
                string message = "You are not logged In!";
                ResponsePanel.SetResponse(message);
                return false;
            }

            return true;
        }

        public async void OnReadContract()
        {
            try
            {
                string network = networkField.text;
                string abiInput = abiInputField.text;
                string contractAddress = contractAddressField.text;
                string functionArgs = functionArgsField.text;

                if (string.IsNullOrEmpty(network))
                {
                    ResponsePanel.SetResponse("Network is required.");
                    return;
                }

                if (string.IsNullOrEmpty(abiInput))
                {
                    ResponsePanel.SetResponse("ABI input is required.");
                    return;
                }

                if (string.IsNullOrEmpty(contractAddress))
                {
                    ResponsePanel.SetResponse("Contract address is required.");
                    return;
                }

                string endpoint = EnvironmentHelper.GetBffBaseUrl() + "/api/oc/v1/readContractData";
                string authToken = OktoAuthManager.GetOktoClient().GetAuthorizationToken();

                var response = await ReadSmartContract.ReadContractAsync(network, abiInput, contractAddress, endpoint, authToken, functionArgs);
                ResponsePanel.SetResponse(response);
            }
            catch(Exception ex)
            {
                ResponsePanel.SetResponse(ex.Message);
            }
        }
    }
} 