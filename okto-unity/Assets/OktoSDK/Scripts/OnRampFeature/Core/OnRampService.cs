using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using OktoSDK.Auth;
using OktoSDK.BFF;
using static OktoSDK.BFF.UserPortfolioData;
using static OktoSDK.LoginOAuthDataModels;
using OktoSDK.Utils;

namespace OktoSDK.OnRamp
{
    public class OnRampService : MonoBehaviour
    {
        public static OnRampService Instance { get; private set; }
        private OktoRemoteConfig localConfig;
        public string AuthToken { get; private set; }
        public string DeviceToken { get; private set; }
        public OnRampToken OnRampToken { get; private set; }
        public AddFundsData AddFunds { get; private set; }
        public string PayToken { get; private set; }
        public string Url { get; private set; }

        private HomeController homeController; // Store reference for async calls

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void OpenInSafariView(string url);
#endif

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            DontDestroyOnLoad(gameObject);
            InitializeLocalConfig();
        }

        private void InitializeLocalConfig()
        {
            try
            {
                localConfig = OktoRemoteConfig.Instance;
                CustomLogger.Log("Local configuration initialized successfully");
            }
            catch (Exception e)
            {
                CustomLogger.LogError($"Local Config error: {e}");
            }
        }

        public async void StartOnRamp(string tokenId, WhitelistedToken whitelistedToken, HomeController homeController)
        {
            this.homeController = homeController;
            AuthToken = await OktoAuthManager.GetOktoClient().GetAuthorizationToken();
            try
            {
                CustomLogger.Log($"Initiating add funds for token ID: {tokenId}");
                CombinedToken combinedToken = await CreateCombinedTokenFromApis(tokenId, whitelistedToken);
                if (combinedToken == null) throw new Exception("Failed Combine token");
                PayToken = await homeController.GetTransactionToken(AuthToken, DeviceToken);
                OnRampToken = new OnRampToken(whitelistedToken, combinedToken.token);

                var userDetails = OktoAuthManager.GetUserDetails();


                combinedToken.email = UserDetailsFetcher.GetStoredEmail();

                AddFunds = new AddFundsData
                {
                    walletAddress = combinedToken.walletAddress,
                    walletBalance = combinedToken.balance,
                    tokenId = combinedToken.id,
                    networkId = combinedToken.networkId,
                    tokenName = combinedToken.shortName,
                    chainId = combinedToken.networkName,
                    userId = combinedToken.userId,
                    email = combinedToken.email,
                    countryCode = "IN",
                    theme = "light",
                    app_version = "500000",
                    screen_source = "qab+Portfolio+Screen",
                    payToken = PayToken,
                };

                Url = BuildConfig.Instance.CreatePaymentUriAdd(AddFunds);
                OnRampWebViewController._instance.StartWebView(Url, AuthToken, DeviceToken);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error initiating add funds: {ex.Message}");
                ResponsePanel.SetResponse("Could not load add funds page. Please try again later.");
            }
        }

        private async Task<CombinedToken> CreateCombinedTokenFromApis(string tokenId, WhitelistedToken whitelistedToken)
        {
            try
            {
                List<Wallet> wallets = await OnRampApi.GetWalletAsync(AuthToken, DeviceToken);
                UserPortfolioData portfolio = await OnRampApi.GetPortfolioAsync(AuthToken);
                UserFromToken userfromToken = await OnRampApi.GetUserFromTokenAsync(AuthToken);
                CustomLogger.Log("useToken " + userfromToken.UserId);
                GroupToken portfolioToken = portfolio.groupTokens.FirstOrDefault(t => t.id == tokenId);
                CustomLogger.Log(JsonConvert.SerializeObject(portfolioToken));

                Wallet wallet = wallets.FirstOrDefault(w => w.cap2Id == whitelistedToken.NetworkId);
                CustomLogger.Log(JsonConvert.SerializeObject(whitelistedToken));

                CustomLogger.Log(JsonConvert.SerializeObject(wallet));
                return new CombinedToken(whitelistedToken, portfolioToken, wallet, userfromToken);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error initiating add funds: {ex.Message}");
                ResponsePanel.SetResponse("Could not load add funds page. Please try again later.");
                return null;
            }
        }

        // Handles messages from the WebView
        public void HandleWebViewMessage(string message, OnRampWebViewController webViewController)
        {
            if (string.IsNullOrEmpty(message))
            {
                CustomLogger.Log("[Unity] Empty or null message received");
                return;
            }
            CustomLogger.Log($"[Unity] Received message: Step1 - Raw: {message}");
            if (message.StartsWith("\"") && message.Contains("\\\""))
            {
                try
                {
                    message = JsonConvert.DeserializeObject<string>(message);
                    CustomLogger.Log($"[Unity] Unwrapped double-serialized message: {message}");
                }
                catch (Exception ex)
                {
                    CustomLogger.LogError($"[Unity] Failed to unwrap double-serialized JSON: {ex.Message}");
                    return;
                }
            }
            WebEventModel data = null;
            try
            {
                data = JsonConvert.DeserializeObject<WebEventModel>(message);
            }
            catch (Exception jsonEx)
            {
                //Debug.LogError($"[Unity] JSON Deserialize Error: {jsonEx.Message}\nRaw Message: {message}");
                return;
            }
            if (data == null)
            {
                CustomLogger.LogWarning("[Unity] Deserialized data is null");
                return;
            }
            if (data.source == "okto_web")
            {
                CustomLogger.Log("[Unity] Skipping unity-originated event");
                return;
            }
            if (data.type == "onMetaHandler")
            {
                HandleMetaEvent(message);
            }
            else
            {
                HandleWebEvent(data, webViewController);
            }
            CustomLogger.Log($"[Unity] Received message: Step2 - Type: {data.type}, ID: {data.id}");
        }

        private void HandleWebEvent(WebEventModel model, OnRampWebViewController webViewController)
        {
            CustomLogger.Log("==== JsonConvert.SerializeObject(model) ==== " + JsonConvert.SerializeObject(model));
            CustomLogger.Log($"[Unity] Handling event type: {model.type}");
            switch (model.type)
            {
                case WebEvent.Analytics:
                    CustomLogger.Log($"[Unity] Analytics event: {JsonConvert.SerializeObject(model)}");
                    break;
                case WebEvent.Close:
                    webViewController.CloseWebView();
                    break;
                case WebEvent.Url:
                    HandleUrl(model);
                    break;
                case WebEvent.RequestPermission:
                    HandleRequestPermission(model, webViewController);
                    break;
                case WebEvent.RequestPermissionAck:
                    HandleRequestPermissionAck(model, webViewController);
                    break;
                case WebEvent.Data:
                    HandleData(model, webViewController);
                    break;
                default:
                    CustomLogger.Log($"[Unity] Unhandled event type: {model.type}");
                    break;
            }
        }

        //private WebViewObject kycWebView;

        private void HandleUrl(WebEventModel model)
        {
            CustomLogger.Log($"[Unity] URL event: {JsonConvert.SerializeObject(model.@params)}");

            string url = model.@params["url"]?.ToString();
            if (!string.IsNullOrEmpty(url))
            {


#if UNITY_IOS && !UNITY_EDITOR
    OpenInSafariView(url);
#else
                Application.OpenURL(url);
#endif
            }
        }

        private void HandleRequestPermission(WebEventModel model, OnRampWebViewController webViewController)
        {
#if !UNITY_EDITOR
    RequestCameraPermissionLoop(model,webViewController);
#endif
        }

        private void RequestCameraPermissionLoop(WebEventModel model, OnRampWebViewController webViewController)
        {
            CameraPermissionManager.HandleCameraPermission(this, (granted) =>
            {
                if (granted)
                {
                    if (model.@params?.ContainsKey("data") == true)
                    {
                        var permissionData = new Dictionary<string, object>
                            {
                                { "requestPermissions", model.@params["data"] }
                            };

                        webViewController.SendMessageToWebView(JsonConvert.SerializeObject(permissionData));
                        CustomLogger.Log($"[Unity] Permission request: {JsonConvert.SerializeObject(permissionData)}");
                    }

                    CustomLogger.Log("[Unity] Permission granted, you can now access the camera.");
                    webViewController.SetCameraAccess(true);
                    // Proceed with camera usage
                    var ackModel = model.CopyWith(newType: WebEvent.RequestPermissions);
                    webViewController.SendMessageToWebView(JsonConvert.SerializeObject(ackModel.AckJson()));
                }
                else
                {
                    CustomLogger.Log("[Unity] Permission denied, prompting user...");

                    // Show custom popup or UI prompt to retry
                    ShowPermissionRetryDialog(() =>
                    {
                        // Retry when user clicks "Try Again"
                        RequestCameraPermissionLoop(model, webViewController);
                    });
                }
            });
        }

        private void ShowPermissionRetryDialog(Action onRetry)
        {
            // Replace this with your UI dialog logic (native or Unity UI)
            CustomLogger.Log("[Unity] Showing custom permission retry popup...");

            StartCoroutine(AutoRetry(onRetry));
        }

        private IEnumerator AutoRetry(Action onRetry)
        {
            yield return new WaitForSeconds(2f);
            onRetry?.Invoke();
        }


        private void HandleRequestPermissionAck(WebEventModel model, OnRampWebViewController webViewController)
        {
            var ackModel = model.CopyWith(newType: WebEvent.RequestPermission);
            webViewController.SendMessageToWebView(JsonConvert.SerializeObject(ackModel.AckJson()));
        }

        private async void HandleData(WebEventModel model, OnRampWebViewController webViewController)
        {
            var response = await FetchAndAckData(model);
            if (response != null)
            {
                webViewController.SendMessageToWebView(JsonConvert.SerializeObject(response.AckJson()));
            }
        }

        private async Task<WebEventModel> FetchAndAckData(WebEventModel model)
        {
            try
            {
                var requestData = model.@params ?? new Dictionary<string, object>();
                string key = requestData.GetValueOrDefault(WebKeys.Key)?.ToString() ?? "";
                string source = requestData.GetValueOrDefault(WebKeys.Source)?.ToString() ?? "";
                CustomLogger.Log($"[Unity] FetchAndAckData - Key: {key}, Source: {source}");
                object responseData = null;
                Dictionary<string, object> responseDict = new Dictionary<string, object>();

                if (source == WebKeys.RemoteConfig)
                {
                    if (localConfig != null)
                    {
                        var configValue = localConfig.GetValue(key);
                        responseDict[key] = configValue.StringValue;
                    }
                }
                else
                {
                    switch (key)
                    {
                        case WebKeys.TransactionId:
                            if (homeController != null)
                            {
                                AuthToken = await OktoAuthManager.GetOktoClient().GetAuthorizationToken();
                                responseData = await homeController.GetTransactionToken(AuthToken, DeviceToken);
                                responseDict[key] = responseData;
                            }
                            break;

                        case WebKeys.TokenData:
                            if (source == OnRampToken?.WhitelistedToken?.Id)
                            {
                                var tokenData = OnRampToken.AckJson();

                                // Serialize tokenData to JSON string before adding to dictionary
                                string tokenDataJsonString = JsonConvert.SerializeObject(tokenData);

                                responseDict["tokenData"] = tokenDataJsonString;
                            }
                            break;

                        case WebKeys.OrderSuccessBottomSheet:
                            OnRampWebViewController._instance.CloseWebView();
                            ResponsePanel.SetResponse("Your transaction is Sucessfull.Your transaction might take a few minutes to complete!");
                            break;

                        case WebKeys.OrderFailureBottomSheet:
                            OnRampWebViewController._instance.CloseWebView();
                            ResponsePanel.SetResponse("Your transaction failed due to some reason. Please try again");
                            break;
                    }
                }

                CustomLogger.Log($"[Unity] FetchAndAckData Response: {JsonConvert.SerializeObject(responseDict)}");

                return new WebEventModel
                {
                    type = WebEvent.Data,
                    response = responseDict,
                    source = "okto_web",
                    id = model.id
                };
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"[Unity] Error in FetchAndAckData: {ex.Message}");
                return null;
            }
        }

        public void HandleMetaEvent(string json)
        {
            var metaEvent = JsonConvert.DeserializeObject<MetaEventModel>(json);
            if (metaEvent != null && metaEvent.detail.paymentStatus == "success")
            {
                ResponsePanel.SetResponse(JsonConvert.SerializeObject(metaEvent.detail, Formatting.Indented));
            }
            else if (metaEvent != null && metaEvent.detail.paymentStatus == "failed")
            {
                ResponsePanel.SetResponse(JsonConvert.SerializeObject(metaEvent.detail, Formatting.Indented));
            }
        }

        public static bool GetOnRamp()
        {
            if (Instance == null || Instance.localConfig == null)
                return false;
            return Instance.localConfig.GetValue(WebKeys.OnRampEnabled).BooleanValue;
        }

        public static void CallOnRamp(string tokenId, WhitelistedToken whitelistedToken, HomeController homeController)
        {
            if (Instance == null)
            {
                CustomLogger.LogError("OnRampService.Instance is null. Make sure the service is initialized.");
                return;
            }
            Instance.StartOnRamp(tokenId, whitelistedToken, homeController);
        }
    }
}