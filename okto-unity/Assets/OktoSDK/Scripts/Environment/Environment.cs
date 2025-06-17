using UnityEngine;

namespace OktoSDK
{
    public class Environment : MonoBehaviour
    {
        [Header("Local Fields")]
        [SerializeField]
        private string tokenId;

        [Header("Shared Config")]
        [SerializeField]
        private EnvironmentConfig config;  // ScriptableObject holding other config

        private static Environment _instance;

        private void Awake()
        {
            _instance = this;
        }

        private static Environment Instance()
        {
            return _instance;
        }

        public static string GetTokenId() => Instance()?.tokenId ?? string.Empty;

        public static bool IsAutoLoginEnabled() => Instance()?.config?.isAutoLoginEnabled ?? false;

        public static bool IsLogEnabled() => Instance()?.config?.isLogEnabled ?? false;

        public static ScreenOrientation GetDefaulOrientation() => Instance()?.config?.defaultOrientation ?? ScreenOrientation.Portrait;

        public static string GetClientSWA(string env)
        {
            var conf = GetConfigByEnv(env);
            return conf?.clientSwa ?? string.Empty;
        }

        public static string GetClientPrivateKey(string env)
        {
            var conf = GetConfigByEnv(env);
            return conf?.clientPrivateKey ?? string.Empty;
        }

        private static Config GetConfigByEnv(string env)
        {
            var instance = Instance();
            if (instance?.config?.clientDetails == null) return null;

            foreach (var conf in instance.config.clientDetails)
            {
                if (conf.env.Equals(env))
                    return conf;
            }

            return null;
        }
    }
}
