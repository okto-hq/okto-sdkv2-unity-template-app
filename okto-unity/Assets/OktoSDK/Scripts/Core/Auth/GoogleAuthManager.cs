using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace OktoSDK.Auth
{
    public class GoogleAuthManager : MonoBehaviour
    {
        private const string CLIENT_ID = "625834323626-c0rnc34fogig1n059prr5q73p6a6uulf.apps.googleusercontent.com";

        private string REDIRECT_URI;
        private const string SCOPE = "openid email profile";

        public static GoogleAuthManager Instance { get; private set; }
        private string currentNonce;

        public static event Action<string, string> OnFetchedToken;

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void OpenInSafariView(string url);
#endif


#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CloseSafariView();
#endif


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Application.deepLinkActivated += HandleDeepLink;

                // Handle already received deep link (cold start)
                if (!string.IsNullOrEmpty(Application.absoluteURL))
                {
                    HandleAuthCallback(Application.absoluteURL);
                }
            }
            else
            {
                Destroy(gameObject);
            }

        }

        private string GenerateNonce()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] nonceBytes = new byte[32];
                rng.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes)
                             .TrimEnd('=') // Remove padding
                             .Replace('+', '-') // URL-safe
                             .Replace('/', '_');
            }
        }

        public void LoginUsingGoogleAuth()
        {
            REDIRECT_URI = OktoAuthManager.GetOktoClient().Env.LoginOAuth;
            CustomLogger.Log("REDIRECT_URI "+ REDIRECT_URI);

            Loader.ShowLoader();
            try
            {
                currentNonce = GenerateNonce();
                string baseUrl = "https://accounts.google.com/o/oauth2/v2/auth";

                // Determine client_url and platform manually
                string clientUrl = "oktosdk://auth"; // This should match your deep link setup on mobile
                string platform =
#if UNITY_WEBGL
            "web";
#elif UNITY_ANDROID
                    "android";
#elif UNITY_IOS
            "ios";
#else
            "unknown";
#endif

                var stateObject = new AuthState(clientUrl, platform);
                string stateJson = JsonUtility.ToJson(stateObject);
                string state = Uri.EscapeDataString(stateJson);

                string url = $"{baseUrl}?" +
                             $"scope={Uri.EscapeDataString(SCOPE)}&" +
                             $"redirect_uri={Uri.EscapeDataString(REDIRECT_URI)}&" +
                             $"response_type=id_token&" +
                             $"client_id={CLIENT_ID}&" +
                             $"nonce={currentNonce}&" +
                             $"state={state}&" +
                             $"prompt=select_account";

                CustomLogger.Log("Google Auth URL: " + url);
#if UNITY_IOS && !UNITY_EDITOR
    OpenInSafariView(url);
#else
                Application.OpenURL(url);
#endif
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in LoginUsingGoogleAuth: {ex.Message}");
            }
        }


        public void HandleDeepLink(string url)
        {
            CustomLogger.Log($"Deep link activated: {url}");
            #if UNITY_IOS && !UNITY_EDITOR
                CloseSafariView();
            #endif
            HandleAuthCallback(url);
        }

        public void HandleAuthCallback(string url)
        {
            try
            {
                CustomLogger.Log($"Handling auth callback URL: {url}");

                Uri uri = new Uri(url);
                string fragment = uri.Fragment;
                string query = uri.Query;

                string tokenSource = !string.IsNullOrEmpty(fragment) ? fragment.Substring(1) :
                                     !string.IsNullOrEmpty(query) ? query.Substring(1) : null;

                if (string.IsNullOrEmpty(tokenSource))
                {
                    CustomLogger.LogWarning("No token found in URL fragment or query.");
                    return;
                }

                var parameters = ParseQueryString(tokenSource);

                if (parameters.ContainsKey("id_token"))
                {
                    string idToken = parameters["id_token"];
                    ProcessValidToken(idToken);
                }
                else
                {
                    CustomLogger.LogWarning("id_token not found in the callback URL.");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error processing auth callback: {ex.Message}");
            }
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(query))
                return parameters;

            var pairs = query.Split('&');

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = Uri.UnescapeDataString(keyValue[0]);
                    string value = Uri.UnescapeDataString(keyValue[1]);
                    parameters[key] = value;
                }
            }

            return parameters;
        }

        private void ProcessValidToken(string idToken)
        {
            try
            {
                CustomLogger.Log($"Received ID Token: {idToken}");

                var payload = DecodeJwtPayload(idToken);

                if (payload != null)
                {
                    CustomLogger.Log($"Authenticated user email: {payload.email}");
                    OnFetchedToken?.Invoke(payload.email, idToken);
                }
                else
                {
                    CustomLogger.LogWarning("Failed to decode JWT payload.");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error processing token: {ex.Message}");
            }
        }

        private JwtPayload DecodeJwtPayload(string idToken)
        {
            try
            {
                var parts = idToken.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];
                var padding = new string('=', (4 - payload.Length % 4) % 4);
                var base64 = payload.Replace('-', '+').Replace('_', '/') + padding;
                var jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

                return JsonUtility.FromJson<JwtPayload>(jsonPayload);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error decoding JWT: {ex.Message}");
                return null;
            }
        }

        [Serializable]
        private class JwtPayload
        {
            public string sub;
            public string email;
            public bool email_verified;
            public string name;
            public string picture;
            public string nonce;
        }

        [Serializable]
        public class AuthState
        {
            public string client_url;
            public string platform;

            public AuthState(string clientUrl, string platform)
            {
                this.client_url = clientUrl;
                this.platform = platform;
            }
        }
    }
}

