using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Nethereum.Signer;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using OktoSDK.Auth;

namespace OktoSDK
{
    public class WhatsAppAuthentication : MonoBehaviour
    {
        public string latestToken;

        public async Task<OtpApiResponse> SendPhoneOtpAsync(string phoneNumber, string countryShortName = "IN")
        {
            CustomLogger.Log("====SendPhoneOtpAsync===");

            string apiUrl = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new SendOtpData(phoneNumber, countryShortName, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new SendOtpRequest(phoneNumber, countryShortName, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp, signature);

            CustomLogger.Log(JsonConvert.SerializeObject(req));

            var response = await MakeApiCall<OtpApiResponse>(apiUrl, req);
            return response;
        }

        public async Task<OtpApiResponse> ResendPhoneOtpAsync(string phoneNumber, string token , string countryShortName = "IN")
        {
            string apiUrl = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new ResendOtpData(phoneNumber, countryShortName, token, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new ResendOtpRequest(phoneNumber, countryShortName, token, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp, signature);

            OtpApiResponse response = await MakeApiCall<OtpApiResponse>(apiUrl, req);
            return response;
        }

        public async Task<AuthResponse> VerifyPhoneOtpAsync(string phoneNumber, string otp, string token, string countryShortName = "IN")
        {
            string apiUrl = OktoAuthManager.GetOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp/verify";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new VerifyOtpData(phoneNumber, countryShortName, otp, token, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new VerifyOtpRequest(phoneNumber, countryShortName, otp, token, OktoAuthManager.GetOktoClientConfig().ClientSWA, timestamp, signature);

            AuthResponse response = await MakeApiCall<AuthResponse>(apiUrl, req);
            return response;
        }

        private async Task<T> MakeApiCall<T>(string apiUrl, object requestBody) where T : class
        {
            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

            CustomLogger.Log($"API Request to {apiUrl}: {jsonData}");

            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                string responseText = request.downloadHandler.text;
                CustomLogger.Log($"Raw API Response: {responseText}");

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        try
                        {
                            // Handle WhatsAppBffApiResponse specifically
                            if (typeof(T) == typeof(OtpApiResponse))
                            {
                                var whatsAppResponse = JsonConvert.DeserializeObject<OTPBffApiResponse>(responseText);
                                if (whatsAppResponse != null)
                                {
                                    if (whatsAppResponse.Status == "error" && whatsAppResponse.Error != null)
                                    {
                                        throw new Exception($"WhatsApp Error {whatsAppResponse.Error.Code}: {whatsAppResponse.Error.Details ?? whatsAppResponse.Error.Message}");
                                    }

                                    if (whatsAppResponse.Status == "success" && whatsAppResponse.Data != null)
                                    {
                                        return whatsAppResponse.Data as T;
                                    }
                                }
                            }
                            // Handle AuthResponseBff separately
                            else if (typeof(T) == typeof(AuthResponse))
                            {
                                var authResponseBff = JsonConvert.DeserializeObject<AuthResponseBff>(responseText);
                                if (authResponseBff != null)
                                {
                                    if (authResponseBff.Status == "error")
                                    {
                                        throw new Exception($"Auth Error: {ExtractErrorMessage(responseText)}");
                                    }

                                    if (authResponseBff.Status == "success" && authResponseBff.Data != null)
                                    {
                                        return authResponseBff.Data as T;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CustomLogger.Log($"Error parsing response: {ex.Message}");
                            throw new Exception(ExtractErrorMessage(responseText));
                        }
                    }
                }

                throw new Exception(ExtractErrorMessage(responseText));
            }
        }


        public string ExtractErrorMessage(string responseText)
        {
            try
            {
                // Deserialize the response to a dictionary
                var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

                // Check if the error object exists
                if (errorResponse.ContainsKey("error") && errorResponse["error"] is Newtonsoft.Json.Linq.JObject errorObj)
                {
                    // Extract the details field from the error object
                    string errorDetails = errorObj["message"]?.ToString();
                    if (!string.IsNullOrEmpty(errorDetails))
                    {
                        return errorDetails;
                    }
                }

                return "Failed";
            }
            catch (Exception)
            {
                return "Failed";
            }
        }


        public static string SignMessage(string message)
        {
            string privateKey = OktoAuthManager.GetOktoClientConfig().ClientPrivateKey;
            var ethKey = new EthECKey(privateKey.StartsWith("0x") ? privateKey.Substring(2) : privateKey);
            var signer = new EthereumMessageSigner();
            string signature = signer.EncodeUTF8AndSign(message, ethKey);
            return signature.StartsWith("0x") ? signature : "0x" + signature;
        }
    }
}

