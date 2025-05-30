using System;
using UnityEngine;

//single place to manage all environment variables
namespace OktoSDK
{
    public class Environment : MonoBehaviour
    {
        [SerializeField]
        private string tokenId;

        [SerializeField]
        private bool isManually;

        [SerializeField]
        private bool isAutoLoginEnabled;

        [SerializeField]
        private Config[] clientDetails;

        [SerializeField]
        private string googleWebClientId;

        [SerializeField]
        private bool isLogEnabled;

        [SerializeField]
        private ScreenOrientation defaulOrientation = ScreenOrientation.Portrait;

        private static Environment _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static string GetTokenId()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return string.Empty;
            }
            return _instance.tokenId;
        }

        public static ScreenOrientation GetDefaulOrientation()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return ScreenOrientation.Portrait;
            }
            return _instance.defaulOrientation;
        }

        public static bool GetManuallyLogin()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return false;
            }
            return _instance.isManually;
        }

        public static bool IsAutoLoginEnabled()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return false;
            }
            return _instance.isAutoLoginEnabled;
        }

        public static string GetClientSWA(string env)
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return string.Empty;
            }
            if (env.Equals(OktoEnv.SANDBOX))
            {
                return _instance.clientDetails[0].clientSwa;
            }
            else if (env.Equals(OktoEnv.STAGING))
            {
                return _instance.clientDetails[1].clientSwa;
            }
            else
            {
                return _instance.clientDetails[2].clientSwa;
            }
        }

        public static string GetClientPrivateKey(string env)
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return string.Empty;
            }
            if (env.Equals(OktoEnv.SANDBOX))
            {
                return _instance.clientDetails[0].clientPrivateKey;
            }
            else if (env.Equals(OktoEnv.STAGING))
            {
                return _instance.clientDetails[1].clientPrivateKey;
            }
            else
            {
                return _instance.clientDetails[2].clientPrivateKey;
            }
        }

        public static string GetGoogleWebClient()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return string.Empty;
            }
            return _instance.googleWebClientId;
        }

        public static bool IsLogEnabled()
        {
            if (_instance == null)
            {
                CustomLogger.LogError("Environment is not initialized.");
                return false;
            }
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