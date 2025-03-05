using Newtonsoft.Json;


namespace OktoSDK
{

    public class AuthData
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }
        
        [JsonProperty("provider")]
        public string Provider { get; set; }
    }

    public class AuthSessionData
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        
        [JsonProperty("clientSWA")]
        public string ClientSWA { get; set; }
        
        [JsonProperty("sessionPk")]
        public string SessionPk { get; set; }
        
        [JsonProperty("maxPriorityFeePerGas")]
        public string MaxPriorityFeePerGas { get; set; }
        
        [JsonProperty("maxFeePerGas")]
        public string MaxFeePerGas { get; set; }
        
        [JsonProperty("paymaster")]
        public string Paymaster { get; set; }
        
        [JsonProperty("paymasterData")]
        public string PaymasterData { get; set; }
    }

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

    public class AuthenticateResult
    {
        public string UserSWA { get; set; }
        public string Nonce { get; set; }
        public string ClientSWA { get; set; }
        public long SessionExpiry { get; set; }
    }
} 