using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;

namespace OktoSDK.OnRamp
{
    public class OnRampWebViewController : MonoBehaviour
    {
        private WebViewObject webViewObject;
        private bool isInjected;
        private HashSet<string> processedMessageIds = new HashSet<string>();
        public static OnRampWebViewController _instance;

        [SerializeField] private HomeController homeController;

        private void OnEnable() => _instance = this;

        public void StartWebView(string url, string authToken, string deviceToken)
        {
            StartCoroutine(InitializeWebViewCoroutine(url, authToken, deviceToken));
        }

        private IEnumerator InitializeWebViewCoroutine(string url, string authToken, string deviceToken)
        {
            // Force orientation to portrait
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;

            // Wait for orientation to apply (at least one frame)
            yield return new WaitForSeconds(0.5f);

            try
            {
                CustomLogger.Log($"[WebView] Initializing WebView with URL: {url}");
                isInjected = false;
                processedMessageIds.Clear();
                webViewObject = gameObject.AddComponent<WebViewObject>();

                webViewObject.Init(
                    cb: (msg) =>
                    {
                        CustomLogger.Log($"[WebView] Callback received: {msg}");
                        OnMessageReceived(msg);
                    },
                    err: (msg) => CustomLogger.LogError($"[WebView] Error: {msg}"),
                    httpErr: (msg) => CustomLogger.LogError($"[WebView] HTTP Error: {msg}"),
                    hooked: (msg) => CustomLogger.Log($"[WebView] Hooked: {msg}"),
                    started: (msg) =>
                    {
#if UNITY_EDITOR || UNITY_ANDROID || (UNITY_EDITOR && UNITY_IOS)
                if (!isInjected)
                        {
                            CustomLogger.Log("[WebView] Injecting Tokens and JS...");
                            InjectJavaScript();
                            isInjected = true;
                        }
#endif
            },
                    ld: (msg) =>
                    {
                        InjectAuthAndDeviceToken(authToken, deviceToken);
                        CustomLogger.Log($"[WebView] Page Loaded: {msg}");

                        int chunkSize = 2000;
                        for (int i = 0; i < msg.Length; i += chunkSize)
                        {
                            string chunk = msg.Substring(i, Mathf.Min(chunkSize, msg.Length - i));
                            CustomLogger.Log($"[WebView] Page Loaded Part: {chunk}");
                        }
                    },
#if UNITY_ANDROID
            androidForceDarkMode: 1
#elif UNITY_IOS
            enableWKWebView: true,
            wkContentMode: 1,
            wkAllowsLinkPreview: true,
            wkAllowsBackForwardNavigationGestures: true,
            separated: true
#elif UNITY_EDITOR
            separated: true
#endif
        );

                webViewObject.ClearCache(true);
                webViewObject.ClearCookies();
                webViewObject.SetInteractionEnabled(true);
                webViewObject.SetAlertDialogEnabled(true);
                webViewObject.SetMargins(0, 0, 0, 0);
                webViewObject.SetURLPattern("*", ".*", ".*");
                webViewObject.LoadURL(url);
                webViewObject.SetVisibility(true);
                webViewObject.GetComponent<Image>().enabled = true;
                Loader.DisableLoader();
            }
            catch (Exception e)
            {
                CustomLogger.LogError($"[WebView] Exception: {e.Message}\n{e.StackTrace}");
                ResponsePanel.SetResponse("Failed opening webview");
            }
        }


        private void InjectAuthAndDeviceToken(string authToken, string deviceToken)
        {
            string injectTokensJS = $@"
                window.localStorage.setItem('authToken', '{authToken}');
                console.log('Tokens Injected:', window.localStorage.getItem('authToken'));
            ";
            webViewObject.EvaluateJS(injectTokensJS);
        }

        private void InjectJavaScript()
        {
            string js = @"
    (function() {
        function sendMessage(msg) {
            try {
                if (window.webkit?.messageHandlers?.unityControl) {
                    window.webkit.messageHandlers.unityControl.postMessage(msg); // iOS
                } else if (window.Unity && typeof Unity.call === 'function') {
                    Unity.call(typeof msg === 'string' ? msg : JSON.stringify(msg)); // Android
                } else {
                    console.warn('No bridge found to send message:', msg);
                }
            } catch (e) {
                console.error('Failed to send message to Unity:', e);
            }
        }

        window.UnityBridge = {
            postMessage: sendMessage
        };

        // Test connection
        sendMessage({
            type: 'test',
            message: 'Bridge initialized'
        });

        // Override window.postMessage safely
        var originalPostMessage = window.postMessage;
        window.postMessage = function(msg) {
            sendMessage(msg);
            if (typeof originalPostMessage === 'function') {
                originalPostMessage.apply(window, arguments);
            }
        };
    })();
    ";

            webViewObject.EvaluateJS(js);
        }

        public void SetCameraAccess(bool access)
        {
            webViewObject.SetCameraAccess(access);
        }

        private void OnMessageReceived(string message)
        {
            // Delegate to service for event handling
            OnRampService.Instance.HandleWebViewMessage(message, this);
        }


        public void SendMessageToWebView(string jsonMessage)
        {
            try
            {
                // jsonMessage should be a valid JSON string (e.g. from JsonConvert.SerializeObject)
                // Escape the JSON string so it can be safely injected into JS code
                string safeJsonString = JsonConvert.ToString(jsonMessage);

                // JavaScript code to parse the JSON string and post the message
                string jsCode = $@"
                (function() {{
                    try {{
                        const message = JSON.parse({safeJsonString});
                        console.log('Received message from Unity:', message);
                        window.postMessage(message, '*');
                    }} catch (e) {{
                        console.error('Failed to parse message from Unity:', e, {safeJsonString});
                    }}
                }})();
            ";

                webViewObject.EvaluateJS(jsCode);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error sending message to WebView: {ex}");
            }
        }


        public void CloseWebView()
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

        private void OnDestroy()
        {
            Destroy(webViewObject);
        }

        public static bool GetOnRamp()
        {
            return OnRampService.GetOnRamp();
        }

        public static void CallOnRamp(string tokenId, WhitelistedToken whitelistedToken, HomeController homeController)
        {
            OnRampService.CallOnRamp(tokenId, whitelistedToken, homeController);
        }
    }
}