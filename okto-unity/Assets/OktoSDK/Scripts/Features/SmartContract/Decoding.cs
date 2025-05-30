using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace OktoSDK.Features.SmartContract
{
    public class Decoding : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField abiJsonInput;
        [SerializeField] private TMP_InputField functionNameInput;
        [SerializeField] private TMP_InputField encodedDataInput;
        [SerializeField] private Button decodeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject decodingPanel;
        [SerializeField] private Button decodePanelBtn;

        private void OnEnable()
        {
            decodeButton.onClick.AddListener(OnDecodeButtonClick);
            closeButton.onClick.AddListener(ClosePanel);
            decodePanelBtn.onClick.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            decodeButton.onClick.RemoveListener(OnDecodeButtonClick);
            closeButton.onClick.RemoveListener(ClosePanel);
            decodePanelBtn.onClick.RemoveListener(OpenPanel);
        }

        private void OpenPanel()
        {
            ClearFields();
            decodingPanel.SetActive(true);
        }

        private void ClosePanel()
        {
            ClearFields();
            decodingPanel.SetActive(false);
        }

        private void ClearFields()
        {
            abiJsonInput.text = string.Empty;
            functionNameInput.text = string.Empty;
            encodedDataInput.text = string.Empty;
        }

        public void OnDecodeButtonClick()
        {
            string abi = abiJsonInput.text.Trim();
            string functionName = functionNameInput.text.Trim();
            string encodedData = encodedDataInput.text.Trim();

            if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(functionName) || string.IsNullOrEmpty(encodedData))
            {
                DisplayError("Error: ABI, function name, and encoded data must all be provided.");
                return;
            }

            try
            {
                abi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(abi), Formatting.None);
                functionName = CleanInput(functionName);
                encodedData = CleanInput(encodedData);

                string decodedResult = DecodingCore.DecodeFunctionInputAsString(abi, functionName, encodedData);
                CustomLogger.Log($"Decoded Data:\n{decodedResult}");
                ResponsePanel.SetResponse($"Decoded Data:\n{decodedResult}");
            }
            catch (Exception ex)
            {
                DisplayError($"Decoding error: {ex.Message}");
            }
        }

        private string CleanInput(string input)
        {
            return Regex.Replace(input ?? string.Empty, @"[\n\r\\\"" ]", "").Trim();
        }

        private void DisplayError(string message)
        {
            CustomLogger.LogError(message);
            ResponsePanel.SetResponse(message);
        }
    }
}
