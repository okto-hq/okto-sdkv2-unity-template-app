using Newtonsoft.Json;
using OktoSDK.BFF;
using System;

namespace OktoSDK
{
    [Serializable]
    public class OktoClientConfig
    {
        public string Environment { get; set; }
        public string ClientPrivateKey { get; set; }
        public string ClientSWA { get; set; }
    }

    [Serializable]
    public class ClientConfig
    {
        public string ClientPrivKey { get; set; }
        public string ClientPubKey { get; set; }
        public string ClientSWA { get; set; }
    }

    [Serializable]
    public class SessionConfig
    {
        public string SessionPrivKey { get; set; }
        public string SessionPubKey { get; set; }
        public string UserSWA { get; set; }
    }

    [Serializable]
    public class EnvConfig
    {
        public string GatewayBaseUrl { get; set; }
        public string BffBaseUrl { get; set; }
        public string PaymasterAddress { get; set; }
        public string JobManagerAddress { get; set; }
        public string EntryPointContractAdress { get; set; }
        public int ChainId { get; set; }
        public string OnRampBaseUrl { get; set; }
        public string LoginOAuth { get; set; }

    }

    [Serializable]
    public class AuthData
    {
        public string IdToken { get; set; }
        public string Provider { get; set; }
    }

    [Serializable]
    public class AuthenticateResult
    {
        public string UserSWA { get; set; }
        public string Nonce { get; set; }
        public string ClientSWA { get; set; }
        public long SessionExpiry { get; set; }
    }

    [Serializable]
    public class AuthenticatePayloadParam
    {
        [JsonProperty("authData")]
        public AuthData AuthData { get; set; }

        [JsonProperty("sessionData")]
        public AuthSessionData SessionData { get; set; }

        [JsonProperty("sessionPkClientSignature")]
        public string SessionPkClientSignature { get; set; }

        [JsonProperty("sessionDataUserSignature")]
        public string SessionDataUserSignature { get; set; }
    }

    public static class OktoEnv
    {
        public const string SANDBOX = "sandbox";
        public const string PRODUCTION = "production";
    }
}