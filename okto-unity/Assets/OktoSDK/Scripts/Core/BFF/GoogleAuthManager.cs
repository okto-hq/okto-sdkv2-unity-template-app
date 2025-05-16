using System;
using UnityEngine;

namespace OktoSDK.BFF
{
    [Serializable]
    public class GoogleAuthManager
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        
        /// <summary>
        /// Singleton instance for global access.
        /// </summary>
        public static GoogleAuthManager Instance { get; private set; }

        public GoogleAuthManager()
        {
            Instance = this;
        }
        
        public GoogleAuthManager(string clientId)
        {
            ClientId = clientId;
        }

        /// <summary>
        /// Extracts the id_token from a deeplink URL and sets the IdToken property.
        /// </summary>
        /// <param name="url">The deeplink URL containing the id_token fragment.</param>
        public void HandleDeeplinkUrl(string url)
        {
            if (Instance == null)
            {
                Debug.LogError("GoogleAuthManager.Instance is null. Please ensure an instance is created.");
                return;
            }
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("HandleDeeplinkUrl: URL is null or empty.");
                return;
            }
            try
            {
                // Find the fragment part (after #)
                int hashIndex = url.IndexOf('#');
                if (hashIndex == -1 || hashIndex == url.Length - 1)
                {
                    Debug.LogWarning("HandleDeeplinkUrl: No fragment found in URL.");
                    return;
                }
                string fragment = url.Substring(hashIndex + 1);

                // Split fragment into key-value pairs
                var pairs = fragment.Split('&');
                foreach (var pair in pairs)
                {
                    var kv = pair.Split('=');
                    if (kv.Length == 2 && kv[0] == "id_token")
                    {
                        IdToken = Uri.UnescapeDataString(kv[1]);
                        Debug.Log($"Extracted id_token: {IdToken}");
                        return;
                    }
                }
                Debug.LogWarning("HandleDeeplinkUrl: id_token not found in fragment.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"HandleDeeplinkUrl: Exception occurred: {ex.Message}");
            }
        }
    }
} 