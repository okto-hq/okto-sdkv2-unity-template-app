using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OktoSDK
{
    [System.Serializable]
    public class OktoClientConfig
    {
        public string Environment { get; set; }  // "sandbox" or "production"
        public string ClientPrivateKey { get; set; }
        public string ClientSWA { get; set; }
    }

    [System.Serializable]
    public class EnvConfig
    {
        public string GatewayBaseUrl { get; set; }
        public string BffBaseUrl { get; set; }
        public string PaymasterAddress { get; set; }
        public string JobManagerAddress { get; set; }
        public string EntryPointContractAdress { get; set; }
        public int ChainId { get; set; }
    }

    [System.Serializable]
    public class ClientConfig
    {
        public string ClientPubKey { get; set; }
        public string ClientPrivKey { get; set; }
        public string ClientSWA { get; set; }
    }

    [System.Serializable]
    public class SessionConfig
    {
        public string SessionPubKey;
        public string SessionPrivKey;
        public string UserSWA;
    }


    public static class AuthProvider
    {
        public const string OKTO = "okto";
        public const string GOOGLE = "google";
    }

    public static class OktoEnv
    {
        public const string SANDBOX = "sandbox";
        public const string STAGING = "staging";
    }

}