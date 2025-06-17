using OktoSDK.Auth;
using System.Collections.Generic;
using UnityEngine;

namespace OktoSDK.OnRamp
{
    public class BuildConfig : MonoBehaviour
    {
        private static BuildConfig _instance;
        public static BuildConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BuildConfig");
                    _instance = go.AddComponent<BuildConfig>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [System.Serializable]
        public class Config
        {
            public string apiBaseUrl;
            public string paymentBaseUrl;
            public string environment;
        }

        public Config config;


        public string CreatePaymentUriAdd(AddFundsData data)
        {
            var uriBuilder = new System.UriBuilder(OktoAuthManager.GetOktoClient().Env.OnRampBaseUrl);
            uriBuilder.Path = "deposit/add-funds";

            var queryParams = new List<string>();
            foreach (var param in data.ToQueryParameters())
            {
                string value = param.Value;
                //Special handling for screen_source
                if (param.Key == "screen_source")
                {
                    value = "qab+Portfolio+Screen".Replace("+", "%2B");
                }
                else
                {
                    value = System.Web.HttpUtility.UrlEncode(param.Value);
                }
                queryParams.Add($"{param.Key}={value}");
            }

            uriBuilder.Query = string.Join("&", queryParams);
            return uriBuilder.Uri.ToString();
        }
    }
}