using Newtonsoft.Json;
using OktoSDK.Auth;
using System;
using UnityEngine;
using UnityEngine.UI;
using static OktoSDK.LoginOAuthDataModels;

namespace OktoSDK
{
    public class OnboardingWebview : MonoBehaviour
    {
        [SerializeField]
        private WhatsAppAuthentication whatsAppAuthentication;
        [SerializeField]
        private EmailAuthentication emailAuthentication;
        [SerializeField]
        private GoogleAuthManager googleAuthManager;

        private WebViewObject webViewObject;

        private void OnEnable()
        {
            GoogleAuthManager.OnFetchedToken += OAuthLogin;
        }

        private string GetBuildUrl()
        {
            switch (OktoAuthManager.GetOktoClientConfig().Environment.ToUpper())
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
                	androidForceDarkMode: 1
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
                );

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
                string safeMessage = message.Replace("+", "%2B");
                string decodedMessage = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(safeMessage);

                WebViewRequestDataModel requestModel = JsonConvert.DeserializeObject<WebViewRequestDataModel>(decodedMessage);

                if (requestModel != null)
                {
                    CustomLogger.Log($"Request ID: {requestModel.Id}");
                    CustomLogger.Log($"Method: {requestModel.Method}");

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

            switch (webViewRequestModel.Data.Type)
            {
                case "close_webview":
                    CloseWebView();
                    break;

                default:
                    CustomLogger.LogWarning($"Unhandled WhatsApp login type: {webViewRequestModel.Data.Type}");
                    break;
            }

            switch (webViewRequestModel.Data.Provider?.ToLower())
            {
                case "whatsapp":
                    HandleWhatsAppLogin(webViewRequestModel);
                    break;

                case "email":
                    HandleEmailLogin(webViewRequestModel);
                    break;

                case "google":
                    HandleGoogleLogin(webViewRequestModel);
                    break;

                case "close_webview":
                    CloseWebView();
                    break;

                default:
                    Loader.DisableLoader();
                    CustomLogger.LogWarning($"Unhandled provider: {webViewRequestModel.Data.Provider}");
                    break;
            }
        }

        private void HandleWhatsAppLogin(WebViewRequestDataModel webViewRequestModel)
        {
            switch (webViewRequestModel.Data.Type)
            {
                case "request_otp":
                    HandleSendWhatsAppOtp(webViewRequestModel);
                    break;

                case "resend_otp":
                    HandleResendWhatsAppOtp(webViewRequestModel);
                    break;

                case "verify_otp":
                    HandleWhatsAppOtpVerification(webViewRequestModel);
                    break;

                case "close_webview":
                    CloseWebView();
                    break;

                case "paste_otp":
                    Paste(webViewRequestModel);
                    break;

                default:
                    Loader.DisableLoader();
                    CustomLogger.LogWarning($"Unhandled WhatsApp login type: {webViewRequestModel.Data.Type}");
                    break;
            }
        }

        private void HandleEmailLogin(WebViewRequestDataModel webViewRequestModel)
        {
            switch (webViewRequestModel.Data.Type)
            {
                case "request_otp":
                    HandleSendEmailOtp(webViewRequestModel);
                    break;

                case "resend_otp":
                    HandleResendEmailOtp(webViewRequestModel);
                    break;

                case "verify_otp":
                    HandleEmailOtpVerification(webViewRequestModel);
                    break;

                case "close_webview":
                    CloseWebView();
                    break;

                case "paste_otp":
                    Paste(webViewRequestModel);
                    break;

                default:
                    Loader.DisableLoader();
                    CustomLogger.LogWarning($"Unhandled email login type: {webViewRequestModel.Data.Type}");
                    break;
            }
        }

        private void HandleGoogleLogin(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log("HandleGoogleLogin");

            googleAuthManager.LoginUsingGoogleAuth();
        }

        async void OAuthLogin(string email, string token)
        {
            OktoAuthManager.SetUserDetails(new EmailDetail { email = email });
            await OktoAuthManager.Authenticate(token, AuthProvider.GOOGLE);

            CloseWebView();
        }

        private async void HandleSendWhatsAppOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"HandleSendWhatsAppOtp for {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    OtpApiResponse response = await whatsAppAuthentication.SendPhoneOtpAsync(webViewRequestModel.Data.WhatsappNumber);
                    webViewRequestModel.Data.Token = response.Token;
                    whatsAppAuthentication.latestToken = response.Token;
                }
                catch (Exception ex)
                {
                    CustomLogger.Log("HandleSendWhatsAppOtp: " + ex.Message);
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed, try Sending OTP Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private async void HandleWhatsAppOtpVerification(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Verifying WhatsApp OTP: {webViewRequestModel.Data.Otp} for {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    AuthResponse response = await whatsAppAuthentication.VerifyPhoneOtpAsync(
                        webViewRequestModel.Data.WhatsappNumber,
                        webViewRequestModel.Data.Otp,
                        webViewRequestModel.Data.Token
                    );

                    if (!String.IsNullOrEmpty(response.AuthToken))
                    {
                        OktoAuthManager.SetUserDetails(new WhatsAppDetail { PhoneNumber = webViewRequestModel.Data.WhatsappNumber });
                        await OktoAuthManager.Authenticate(response.AuthToken, AuthProvider.OKTO);
                        SendMessageToWebView(JsonConvert.SerializeObject(webViewRequestModel));
                        CloseWebView();
                        return;
                    }
                    else
                    {
                        webViewRequestModel.error = "Request Failed, try Sending OTP Again!";
                    }
                }
                catch (Exception ex)
                {
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed, try Sending OTP Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private async void HandleResendWhatsAppOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Resending WhatsApp OTP to {webViewRequestModel.Data.WhatsappNumber}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    OtpApiResponse response = await whatsAppAuthentication.ResendPhoneOtpAsync(
                        webViewRequestModel.Data.WhatsappNumber,
                        whatsAppAuthentication.latestToken
                    );
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

        private async void HandleSendEmailOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"HandleSendEmailOtp: {webViewRequestModel.Data.Email}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    OtpApiResponse response = await emailAuthentication.SendEmailOtpAsync(webViewRequestModel.Data.Email);
                    webViewRequestModel.Data.Token = response.Token;
                    emailAuthentication.latestToken = response.Token;
                }
                catch (Exception ex)
                {
                    CustomLogger.Log("HandleSendEmailOtp: " + ex.Message);
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed, try Sending Email OTP Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private async void HandleEmailOtpVerification(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Verifying Email OTP: {webViewRequestModel.Data.Otp} for {webViewRequestModel.Data.Email}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    AuthResponse response = await emailAuthentication.VerifyEmailOtpAsync(
                        webViewRequestModel.Data.Email,
                        webViewRequestModel.Data.Otp,
                        webViewRequestModel.Data.Token
                    );

                    if (!String.IsNullOrEmpty(response.AuthToken))
                    {
                        OktoAuthManager.SetUserDetails(new EmailDetail { email = webViewRequestModel.Data.Email });
                        await OktoAuthManager.Authenticate(response.AuthToken, AuthProvider.OKTO);
                        SendMessageToWebView(JsonConvert.SerializeObject(webViewRequestModel));
                        CloseWebView();
                        return;
                    }
                    else
                    {
                        webViewRequestModel.error = "Request Failed, try Sending OTP Again!";
                    }
                }
                catch (Exception ex)
                {
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Request Failed, try Sending OTP Again!";
            }

            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private async void HandleResendEmailOtp(WebViewRequestDataModel webViewRequestModel)
        {
            CustomLogger.Log($"Resending Email OTP to {webViewRequestModel.Data.Email}");

            if (webViewRequestModel.Data != null)
            {
                try
                {
                    OtpApiResponse response = await emailAuthentication.ResendEmailOtpAsync(
                        webViewRequestModel.Data.Email,
                        emailAuthentication.latestToken
                    );
                    webViewRequestModel.Data.Token = response.Token;
                    emailAuthentication.latestToken = response.Token;
                }
                catch (Exception ex)
                {
                    webViewRequestModel.error = ex.Message;
                }
            }
            else
            {
                webViewRequestModel.error = "Try Resend Email OTP Again!";
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

            Screen.orientation = Environment.GetDefaulOrientation();

        }

        private void OnApplicationQuit()
        {
            CloseWebView();
        }

        private void Paste(WebViewRequestDataModel webViewRequestModel)
        {
            // Get the clipboard text using the system copy buffer
            string clipboardText = GUIUtility.systemCopyBuffer;

            // Handle the pasted data
            string pastedData = clipboardText;

            // Log the pasted data
            CustomLogger.Log("Pasted Data: " + pastedData);
            webViewRequestModel.Data.Message = pastedData;
            string jsonData = JsonConvert.SerializeObject(webViewRequestModel);
            SendMessageToWebView(jsonData);
        }

        private void InjectJavaScript()
        {
            string js = @"
            	(function() {
                	window.unityBridge = {
                    	postMessage: function(msg) {
                        	try {
                            	msg = (typeof msg === 'string') ? msg : JSON.stringify(msg);
                            	msg = encodeURIComponent(msg); // Always encode message
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

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
