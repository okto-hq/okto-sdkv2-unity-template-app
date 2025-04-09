using System;
using UnityEngine;

//single place to manage all environment variables
namespace OktoSDK
{
    public class Environment : MonoBehaviour
    {
        [SerializeField]
        private string apiKeyV1;

        [SerializeField]
        private string tokenId;

        [SerializeField]
        private bool isManually;

        [SerializeField]
        private bool shouldFetchClientDetails;

        [SerializeField]
        private bool isAutoLoginEnabled;

        [SerializeField]
        private Config[] clientDetails;

        [SerializeField]
        private string googleWebClientId;

        [SerializeField]
        private bool isLogEnabled;

        private static Environment _instance;

        private void OnEnable()
        {
            _instance = this;
        }

        public static string GetTokenId()
        {
            return _instance.tokenId;
        }

        public static string GetApiKeyV1()
        {
            return _instance.apiKeyV1;
        }

        public static bool GetManuallyLogin()
        {
            return _instance.isManually;
        }

        public static bool GetTestMode()
        {
            return _instance.shouldFetchClientDetails;
        }

        public static bool IsAutoLoginEnabled()
        {
            return _instance.isAutoLoginEnabled;
        }

        public static string GetClientSWA(string env)
        {
            if (env.Equals(OktoEnv.SANDBOX))
            {
                return _instance.clientDetails[0].clientSwa;
            }
            else
            {
                return _instance.clientDetails[1].clientSwa;
            }
        }

        public static string GetClientPrivateKey(string env)
        {
            if (env.Equals(OktoEnv.SANDBOX))
            {
                return _instance.clientDetails[0].clientPrivateKey;
            }
            else
            {
                return _instance.clientDetails[1].clientPrivateKey;
            }
        }

        public static string GetGoogleWebClient()
        {
            return _instance.googleWebClientId;
        }

        public static bool IsLogEnabled()
        {
            return _instance.isLogEnabled;
        }
    }

    [Serializable]
    public class Config
    {
        public string env;
        public string clientSwa;
        public string clientPrivateKey;
    }
}