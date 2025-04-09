using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

//script to manage webview events
namespace OktoSDK
{
    public class OnboardingWebview : MonoBehaviour
    {
        [SerializeField]
        private WhatsAppAuthentication whatsAppAuthentication;

        private WebViewObject webViewObject;


        private string GetBuildUrl()
        {
            switch (OktoAuthExample.getOktoClientConfig().Environment.ToUpper())
            {
                case "SANDBOX":
                    return "https://sandbox-onboarding.okto.tech";
                case "STAGING":
                    return "https://onboarding.oktostage.com";
                default:
                    return "";
            }
        }

        public void OpenOnBoardingScreen()
        {
            Screen.orientation = ScreenOrientation.Portrait;

            try
            {
                webViewObject = gameObject.AddComponent<WebViewObject>();

                webViewObject.Init(
                    cb: (msg) => OnMessageReceived(msg),
                    ld: (msg) =>
                    {
                        CustomLogger.Log($"Page Loaded: {msg}");
                        InjectJavaScript();
                    },

#if !UNITY_EDITOR && UNITY_ANDROID

                    androidForceDarkMode : 1
#endif

#if !UNITY_EDITOR && UNITY_IOS

                enableWKWebView: true,
                wkContentMode: 1,  // 0: recommended, 1: mobile, 2: desktop
                wkAllowsLinkPreview: true,
                wkAllowsBackForwardNavigationGestures: true
#endif


#if UNITY_EDITOR
                    separated: true
#endif
                ) ;


                string url = GetBuildUrl();
                webViewObject.LoadURL(url);
                webViewObject.SetMargins(0, 0, 0, 0);
                webViewObject.SetVisibility(false);
                OpenOnboarding();
            }
            catch (Exception e)
            {
                ResponsePanel.SetResponse(e.Message);
            }
        }


        public void OnMessageReceived(string message)
        {
            try
            {
                CustomLogger.Log($"Message Received: {message}");

                // Deserialize the actual request model
                WebViewRequestDataModel requestModel = JsonConvert.DeserializeObject<WebViewRequestDataModel>(message);

                if (requestModel != null)
                {
                    CustomLogger.Log($"Request ID: {requestModel.Id}");
                    CustomLogger.Log($"Method: {requestModel.Method}");

                    // Handle different request types based on `method`
                    switch (requestModel.Method)
                    {
                        case "okto_sdk_login":
                            HandleOktoLogin(requestModel);
                            break;
                    }
                }
                else
                {
                    CustomLogger.LogError("Failed to deserialize WebViewRequestModel.");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error processing message: {ex.Message}");
            }
        }


        private void HandleOktoLogin(WebViewRequestDataModel webViewRequestModel)
        {
            if (webViewRequestModel.Data == null)
            {
                CustomLogger.LogError("Login request data is null.");
                return;
            }


            // Switch case based on `type`
            switch (webViewRequestModel.Data.Type)
            {
                case "request_otp":
                    HandleSendOtp(webViewRequestModel);
                    break;

                case "resend_otp":
                    HandleResendOtp(webViewRequestModel);
                    break;

                case "verify_otp":
                    HandleOtpVerification(webViewRequestModel);
                    break;

                case "close_webview":
                    CloseWebView();
                    break;

                default:
                    CustomLogger.LogWarning($"Unhandled login type: {webViewRequestModel.Data.Type}");
                    break;
            }
        }

        private async void HandleSendOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"HandleSendOtp OTP: {webViewRequestModel.Data.Otp} for {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    WhatsAppApiResponse response = await whatsAppAuthentication.SendPhoneOtpAsync(webViewRequestModel.Data.WhatsappNumber);
                    webViewRequestModel.Data.Token = response.Token;
                    whatsAppAuthentication.latestToken = response.Token;
                }
                catch (Exception ex)
                {
                    CustomLogger.Log("HandleSendOtp : " + ex.Message);
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed,try Sending Otp Again!";
            }


            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private async void HandleOtpVerification(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Verifying OTP: {webViewRequestModel.Data.Otp} for {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    AuthResponse response = await whatsAppAuthentication.VerifyPhoneOtpAsync(webViewRequestModel.Data.WhatsappNumber, webViewRequestModel.Data.Otp, webViewRequestModel.Data.Token);
                    if (!String.IsNullOrEmpty(response.AuthToken))
                    {
                        OktoAuthExample.Authenticate(response.AuthToken, AuthProvider.OKTO);
                        OktoAuthExample.isWhatsAppLogin = true;
                        OktoAuthExample.SetWhatsAppDeatils(webViewRequestModel.Data.WhatsappNumber);
                        SendMessageToWebView(JsonConvert.SerializeObject(webViewRequestModel));
                        CloseWebView();
                        return;
                    }
                    else
                    {
                        webViewRequestModel.error = "Request Failed,try Sending Otp Again!";
                    }
                }
                catch (Exception ex)
                {
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed,try Sending Otp Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);

        }

        private async void HandleResendOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Resending OTP to {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    WhatsAppApiResponse response = await whatsAppAuthentication.ResendPhoneOtpAsync(webViewRequestModel.Data.WhatsappNumber, whatsAppAuthentication.latestToken);
                    webViewRequestModel.Data.Token = response.Token;
                    whatsAppAuthentication.latestToken = response.Token;
                }
                catch (Exception ex)
                {
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Try Resend OTP Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);

        }

        private void OpenOnboarding()
        {
            webViewObject.GetComponent<Image>().enabled = true;
            webViewObject.SetVisibility(true);
        }

        private void CloseWebView()
        {
            if (webViewObject != null)
            {
                webViewObject.GetComponent<Image>().enabled = false;
                webViewObject.SetVisibility(false);
                Destroy(webViewObject);
                webViewObject = null;
            }

            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        private void InjectJavaScript()
        {
            string js = @"
                (function() {
                    window.unityBridge = {
                        postMessage: function(msg) {
                            try {
                                msg = (typeof msg === 'string') ? msg : JSON.stringify(msg);
                                if (window.Unity && typeof Unity.call === 'function') {
                                    Unity.call(msg);  // Android specific
                                } else if (window.webkit?.messageHandlers?.unityControl) {
                                    window.webkit.messageHandlers.unityControl.postMessage(msg);  // iOS specific
                                } else {
                                    console.error('Unity bridge not found');
                                }
                            } catch (e) {
                                console.error('Failed to send message to Unity:', e);
                            }
                        }
                    };

                    globalThis.requestChannel = globalThis.requestChannel || {
                        postMessage: function(msg) {
                            console.log('requestChannel Event from WebView:', msg);
                            try {
                                let parsedMsg = (typeof msg === 'string') ? JSON.parse(msg) : msg;
                                window.unityBridge.postMessage(parsedMsg);
                            } catch (e) {
                                console.error('Error formatting requestChannel message:', e);
                            }
                        }
                    };
                    console.log('Unity WebView bridge initialized for Android and iOS');
                })();
                ";

            webViewObject.EvaluateJS(js);
        }

        public void SendMessageToWebView(string message)
        {
            string js = $"globalThis.responseChannel?.('{message}');";
            webViewObject.EvaluateJS(js);
            CustomLogger.Log($"Message sent to WebView: {message}");
        }

    }


    [Serializable]
    public class WebViewRequestDataModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("data")]
        public LoginRequestData Data { get; set; }

        [JsonProperty("error")]
        public string error;
    }

    [Serializable]
    public class LoginRequestData
    {

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("whatsapp_number")]
        public string WhatsappNumber { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }


}