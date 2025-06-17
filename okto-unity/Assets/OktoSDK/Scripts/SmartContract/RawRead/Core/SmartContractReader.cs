using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using OktoSDK.Auth;
using System;

namespace OktoSDK.Features.SmartContract
{
    public static class SmartContractReader
    {
        /// <summary>
        /// Sends a POST request to read smart contract data using the specified API payload.
        /// </summary>
        /// <param name="apiPayload">The API payload containing network and contract details.</param>
        /// <param name="endpoint">The API endpoint URL.</param>
        /// <param name="authToken">The authorization token.</param>
        /// <returns>A task that returns the response string from the request.</returns>
        public static async Task<string> ReadContractData(object apiPayload, string endpoint, string authToken)
        {
            string jsonBody = JsonConvert.SerializeObject(apiPayload);

            var curlCommand = $"curl -X POST \"{endpoint}\" " +
                              $"-H \"Content-Type: application/json\" " +
                              $"-H \"Authorization: Bearer {authToken}\" " +
                              $"-d '{jsonBody}'";

            Debug.Log(curlCommand);

            using (UnityWebRequest webRequest = new UnityWebRequest(endpoint, "POST"))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");

                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = 60;

                await webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"HTTP Error: {webRequest.error}");
                    throw new Exception($"Error: {webRequest.error}\nResponse: {webRequest.downloadHandler.text}");
                }

                string responseText = webRequest.downloadHandler.text;
                Debug.Log($"Response: {responseText}");
                return responseText;
            }
        }
    }
}