using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace OktoSDK.Features.SmartContract
{
    public class AbiEncodingUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField abiJsonInput;
        [SerializeField] private TMP_InputField functionNameInput;
        [SerializeField] private TMP_InputField parametersInput;
        [SerializeField] private Button encodeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject encodingPanel;
        [SerializeField] private Button encodePanelBtn;

        private void OnEnable()
        {
            if (encodeButton != null) encodeButton.onClick.AddListener(OnEncodeButtonClick);
            if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
            if (encodePanelBtn != null) encodePanelBtn.onClick.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            if (encodeButton != null) encodeButton.onClick.RemoveListener(OnEncodeButtonClick);
            if (closeButton != null) closeButton.onClick.RemoveListener(ClosePanel);
            if (encodePanelBtn != null) encodePanelBtn.onClick.RemoveListener(OpenPanel);
        }

        private void OpenPanel()
        {
            ClearFields();
            encodingPanel.SetActive(true);
        }

        private void ClosePanel()
        {
            ClearFields();
            encodingPanel.SetActive(false);
        }

        private void ClearFields()
        {
            abiJsonInput.text = string.Empty;
            functionNameInput.text = string.Empty;
            parametersInput.text = string.Empty;
        }

        public void OnEncodeButtonClick()
        {
            try
            {
                string abi = abiJsonInput.text.Trim();
                string functionName = functionNameInput.text.Trim();
                string parametersText = parametersInput.text.Trim();

                if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(functionName))
                {
                    CustomLogger.LogError("Error: ABI and function name must be provided.");
                    return;
                }

                abi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(abi), Formatting.None);
                functionName = CleanInput(functionName);
                parametersText = CleanInput(parametersText);

                object[] parameters = AbiEncoding.ParseInputParameters(parametersText);
                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                CustomLogger.Log($"Encoded Data: {encodedData}");
                ResponsePanel.SetResponse($"Encoded Data: {encodedData}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Encoding error: {ex.Message}";
                CustomLogger.LogError(errorMessage);
                ResponsePanel.SetResponse(errorMessage);
            }
        }

        private string CleanInput(string input)
        {
            return Regex.Replace(input, @"[\n\r\\\"" ]", "").Trim();
        }
    }
}
