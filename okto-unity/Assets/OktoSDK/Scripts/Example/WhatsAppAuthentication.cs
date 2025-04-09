using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Nethereum.Signer;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace OktoSDK
{
    public class WhatsAppAuthentication : MonoBehaviour
    {
        public string latestToken;

        public async Task<WhatsAppApiResponse> SendPhoneOtpAsync(string phoneNumber, string countryShortName = "IN")
        {
            CustomLogger.Log("====SendPhoneOtpAsync===");

            string apiUrl = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new SendOtpData(phoneNumber, countryShortName, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new SendOtpRequest(phoneNumber, countryShortName, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp, signature);

            CustomLogger.Log(JsonConvert.SerializeObject(req));

            var response = await MakeApiCall<WhatsAppApiResponse>(apiUrl, req);
            return response;
        }

        public async Task<WhatsAppApiResponse> ResendPhoneOtpAsync(string phoneNumber, string token , string countryShortName = "IN")
        {
            string apiUrl = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new ResendOtpData(phoneNumber, countryShortName, token, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new ResendOtpRequest(phoneNumber, countryShortName, token, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp, signature);

            WhatsAppApiResponse response = await MakeApiCall<WhatsAppApiResponse>(apiUrl, req);
            return response;
        }

        public async Task<AuthResponse> VerifyPhoneOtpAsync(string phoneNumber, string otp, string token, string countryShortName = "IN")
        {
            string apiUrl = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/oc/v1/authenticate/whatsapp/verify";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var requestBody = new VerifyOtpData(phoneNumber, countryShortName, otp, token, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp);
            string jsonData = JsonConvert.SerializeObject(requestBody);
            string signature = SignMessage(jsonData);

            var req = new VerifyOtpRequest(phoneNumber, countryShortName, otp, token, OktoAuthExample.getOktoClientConfig().ClientSWA, timestamp, signature);

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
                            if (typeof(T) == typeof(WhatsAppApiResponse))
                            {
                                var whatsAppResponse = JsonConvert.DeserializeObject<WhatsAppBffApiResponse>(responseText);
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
                    string errorDetails = errorObj["details"]?.ToString();
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
            string privateKey = OktoAuthExample.getOktoClientConfig().ClientPrivateKey;
            var ethKey = new EthECKey(privateKey.StartsWith("0x") ? privateKey.Substring(2) : privateKey);
            var signer = new EthereumMessageSigner();
            string signature = signer.EncodeUTF8AndSign(message, ethKey);
            return signature.StartsWith("0x") ? signature : "0x" + signature;
        }
    }
}

[Serializable]
public class WhatsAppBffApiResponse
{
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("data")] public WhatsAppApiResponse Data { get; set; }
    [JsonProperty("error")] public Error Error { get; set; }
}

[Serializable]
public class WhatsAppApiResponse
{
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("token")] public string Token { get; set; }
    [JsonProperty("trace_id")] public string TraceId { get; set; }
}

[Serializable]
public class Error
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("errorCode")] public string ErrorCode { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("trace_id")] public string TraceId { get; set; }
    [JsonProperty("details")] public string Details { get; set; }
}


[Serializable]
public class SendOtpRequest
{
    [JsonProperty("data")]
    public SendOtpData Data { get; set; }

    [JsonProperty("client_signature")]
    public string ClientSignature { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = "ethsign";

    public SendOtpRequest(string phone, string country, string swa, long time, string signature)
    {
        Data = new SendOtpData(phone, country, swa, time);
        ClientSignature = signature;
    }
}

[Serializable]
public class SendOtpData
{
    [JsonProperty("whatsapp_number")]
    public string WhatsappNumber { get; set; }

    [JsonProperty("country_short_name")]
    public string CountryShortName { get; set; }

    [JsonProperty("client_swa")]
    public string ClientSwa { get; set; }

    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    public SendOtpData(string phone, string country, string swa, long time)
    {
        WhatsappNumber = phone;
        CountryShortName = country;
        ClientSwa = swa;
        Timestamp = time;
    }
}

[Serializable]
public class VerifyOtpRequest
{
    [JsonProperty("data")]
    public VerifyOtpData Data { get; set; }

    [JsonProperty("client_signature")]
    public string ClientSignature { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = "ethsign";

    public VerifyOtpRequest(string phone, string country, string otp, string token, string swa, long time, string signature)
    {
        Data = new VerifyOtpData(phone, country, otp, token, swa, time);
        ClientSignature = signature;
    }
}

[Serializable]
public class VerifyOtpData
{
    [JsonProperty("whatsapp_number")]
    public string WhatsappNumber { get; set; }

    [JsonProperty("country_short_name")]
    public string CountryShortName { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("otp")]
    public string Otp { get; set; }

    [JsonProperty("client_swa")]
    public string ClientSwa { get; set; }

    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    public VerifyOtpData(string phone, string country, string otp, string token, string swa, long time)
    {
        WhatsappNumber = phone;
        CountryShortName = country;
        Token = token;
        Otp = otp;
        ClientSwa = swa;
        Timestamp = time;
    }
}

[Serializable]
public class ResendOtpRequest
{
    [JsonProperty("data")]
    public ResendOtpData Data { get; set; }

    [JsonProperty("client_signature")]
    public string ClientSignature { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = "ethsign";

    public ResendOtpRequest(string phone, string country, string token, string swa, long time, string signature)
    {
        Data = new ResendOtpData(phone, country, token, swa, time);
        ClientSignature = signature;
    }
}

[Serializable]
public class ResendOtpData
{
    [JsonProperty("whatsapp_number")]
    public string WhatsappNumber { get; set; }

    [JsonProperty("country_short_name")]
    public string CountryShortName { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("client_swa")]
    public string ClientSwa { get; set; }

    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    public ResendOtpData(string phone, string country, string token, string swa, long time)
    {
        WhatsappNumber = phone;
        CountryShortName = country;
        Token = token;
        ClientSwa = swa;
        Timestamp = time;
    }
}

[Serializable]
public class AuthResponseBff
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("data")]
    public AuthResponse Data { get; set; }
}

[Serializable]
public class AuthResponse
{
    [JsonProperty("auth_token")]
    public string AuthToken { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("refresh_auth_token")]
    public string RefreshAuthToken { get; set; }

    [JsonProperty("device_token")]
    public string DeviceToken { get; set; }

    [JsonProperty("trace_id")]
    public string TraceId { get; set; }
}