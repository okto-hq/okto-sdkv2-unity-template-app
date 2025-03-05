using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

[System.Serializable]
public class ContractReadData
{
    public string contractAddress;
    public string abi;
    public string functionName;
    public string[] args;
}

[System.Serializable]
public class ReadContractRequest
{
    public string network_name;
    public ContractReadData data;
}


/*
This script provides functionality to read smart contract data  
by making API requests with user-provided contract details.  
It sends a POST request with contract information and handles responses.  
*/

namespace OktoSDK
{
    public class ReadSmartContract : MonoBehaviour
    {
        public TMP_InputField networkInputField;
        public TMP_InputField abiInputField;
        public TMP_InputField contractAddressField;
        public TMP_InputField functionNameField;
        public TMP_InputField functionArgsField;

        public async void OnReadContract()
        {
            if (!OktoAuthExample.getOktoClient().IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not Logged in!");
                return;
            }

            string network = networkInputField.text;
            string abi = abiInputField.text;
            string contractAddress = contractAddressField.text;
            string functionName = functionNameField.text;
            string functionArgs = functionArgsField.text; // Comma-separated arguments

            if (string.IsNullOrWhiteSpace(network) || string.IsNullOrWhiteSpace(abi) || string.IsNullOrWhiteSpace(contractAddress) || string.IsNullOrWhiteSpace(functionName))
            {
                ResponsePanel.SetResponse("Please enter valid Network,ABI, Contract Address, and Function Name!");
                return;
            }

            var apiPayload = new
            {
                contractAddress = contractAddress,
                abi = abi,
                functionName = functionName,
                functionArgs = functionArgs
            };


            var finalPayload = new
            {
                network_name = network,
                data = apiPayload
            };


            try
            {
                var response = await ReadContractData(finalPayload);
                ResponsePanel.SetResponse(response);
            }
            catch (Exception ex)
            {
                ResponsePanel.SetResponse($"Error: {ex.Message}");
            }
        }

        public static async Task<string> ReadContractData(object apiPayload)
        {
            try
            {
                string url = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/v1/readContractData";

                // Use the provided authorization token directly
                string authToken = await OktoAuthExample.getOktoClient().GetAuthorizationToken();

                string jsonBody = JsonConvert.SerializeObject(apiPayload, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

                using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
                {
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");

                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.timeout = 60;

                    Debug.Log($"Sending request to {url} with payload: {jsonBody}");

                    await webRequest.SendWebRequest();
                    string responseText = webRequest.downloadHandler.text;
                    Debug.Log($"Response: {responseText}");

                    return responseText;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading contract data: {ex.Message}");
                throw;
            }
        }

    }
}