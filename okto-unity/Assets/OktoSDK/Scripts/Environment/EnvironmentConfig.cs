using UnityEngine;

namespace OktoSDK
{
    [CreateAssetMenu(fileName = "EnvironmentConfig", menuName = "OktoSDK/Environment Config")]
    public class EnvironmentConfig : ScriptableObject
    {
        public Config[] clientDetails;
        public bool isAutoLoginEnabled;
        public bool isLogEnabled;
        public ScreenOrientation defaultOrientation = ScreenOrientation.Portrait;
    }

    [System.Serializable]
    public class Config
    {
        public string env;
        public string clientSwa;
        public string clientPrivateKey;
    }
}
