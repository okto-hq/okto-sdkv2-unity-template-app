using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//script for whatsap authenticate 
//code referred from v1,need to optimize furher more
namespace OktoSDK
{
    public class WhatsAppAuthentication : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField phoneInputField;

        [SerializeField]
        private TMP_InputField countryCodeInputField;

        [SerializeField]
        private TMP_InputField otpInputFieldPhone;

        [SerializeField]
        private Button sendBtnOtp;

        [SerializeField]
        private Button sendBtnOtpTest;

        [SerializeField]
        private Button verifyOtp;

        [SerializeField]
        private string oktoApiKey;

        private string phoneToken;
        private static HttpClient httpClient;

        private void OnEnable()
        {
            sendBtnOtp.onClick.AddListener(SendPhoneOtp);
            verifyOtp.onClick.AddListener(VerifyPhoneOtp);
        }

        private void OnDisable()
        {
            sendBtnOtp.onClick.RemoveListener(SendPhoneOtp);
            verifyOtp.onClick.RemoveListener(VerifyPhoneOtp);
        }

        private async void SendPhoneOtp()
        {
            string phoneNumber = phoneInputField.text;
            string countryCode = countryCodeInputField.text;

            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(countryCode))
            {
                ResponsePanel.SetResponse("Phone number and country code cannot be empty.");
                return;
            }

            var (success, token, error) = await SendPhoneOtpAsync(phoneNumber, countryCode);
            if (success)
            {
                phoneToken = token;
                ResponsePanel.SetResponse("Phone OTP sent successfully!");
                otpInputFieldPhone.gameObject.SetActive(true);
            }
            else
            {
                ResponsePanel.SetResponse($"Error: {error?.Message ?? "Failed to send phone OTP"}");
            }
        }

        private async void VerifyPhoneOtp()
        {
            string phoneNumber = phoneInputField.text;
            string countryCode = countryCodeInputField.text;
            string otp = otpInputFieldPhone.text;

            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(phoneToken))
            {
                ResponsePanel.SetResponse("All fields must be filled in for verification.");
                return;
            }

            var (success, authToken, error) = await VerifyPhoneOtpAsync(phoneNumber, countryCode, otp, phoneToken);
            if (success)
            {
                ResponsePanel.SetResponse("Phone OTP verified successfully!");
                OktoAuthExample.LoginWithGoogle(authToken, "google");
            }
            else
            {
                ResponsePanel.SetResponse($"Error: {error?.Message ?? "Failed to verify phone OTP"}");
            }
        }

        public async Task<(bool success, string token, Exception error)> SendPhoneOtpAsync(string phoneNumber, string countryShortName)
        {
            var apiUrl = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/v1/authenticate/whatsapp";
            var requestBody = new { phone_number = phoneNumber, country_short_name = countryShortName };

            try
            {
                var response = await MakeApiCallAsync(apiUrl, requestBody);
                if (response.success && response.data.TryGetValue("token", out var token))
                {
                    return (true, token.ToString(), null);
                }
                return (false, null, new Exception("Failed to send phone OTP"));
            }
            catch (Exception ex)
            {
                return (false, null, ex);
            }
        }

        public async Task<(bool success, string authToken, Exception error)> VerifyPhoneOtpAsync(string phoneNumber, string countryShortName, string otp, string token)
        {
            var apiUrl = OktoAuthExample.getOktoClient().Env.BffBaseUrl + "/api/v1/authenticate/whatsapp/verify";
            var requestBody = new { phone_number = phoneNumber, country_short_name = countryShortName, otp = otp, token = token };

            try
            {
                var response = await MakeApiCallAsync(apiUrl, requestBody);
                if (response.success && response.data.TryGetValue("auth_token", out var authToken))
                {
                    return (true, authToken.ToString(), null);
                }
                return (false, null, new Exception("Failed to verify phone OTP"));
            }
            catch (Exception ex)
            {
                return (false, null, ex);
            }
        }

        private async Task<(bool success, Dictionary<string, object> data, Exception error)> MakeApiCallAsync(string apiUrl, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Set headers
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("x-api-key", oktoApiKey);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
                Debug.Log(apiUrl);
                Debug.Log(requestBody);

                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                Debug.Log(response);

                // Check response status
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Deserialize JSON response into a nested object
                        var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                        // Check the "status" field
                        if (responseData != null &&
                            responseData.TryGetValue("status", out var status) &&
                            status.ToString() == "success")
                        {
                            // Check the "data" field
                            if (responseData.TryGetValue("data", out var data) &&
                                data is Newtonsoft.Json.Linq.JObject dataJObject)
                            {
                                // Convert "data" JObject to Dictionary<string, object>
                                var dataDict = dataJObject.ToObject<Dictionary<string, object>>();
                                return (true, dataDict, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, null, new Exception("Error parsing API response: " + ex.Message));
                    }
                }

                return (false, null, new Exception("Error in API response: " + responseContent));
            }
            catch (Exception ex)
            {
                return (false, null, ex);
            }
        }
    }
}