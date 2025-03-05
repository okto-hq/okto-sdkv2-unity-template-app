namespace OktoSDK
{
    public class OktoClientConfig
    {
        public string Environment { get; set; }  // "sandbox" or "production"
        public string ClientPrivateKey { get; set; }
        public string ClientSWA { get; set; }
    }

    public class EnvConfig
    {
        public string GatewayBaseUrl { get; set; }
        public string BffBaseUrl { get; set; }
        public string PaymasterAddress { get; set; }
        public string JobManagerAddress { get; set; }
        public string EntryPointContractAdress { get; set; }
        public int ChainId { get; set; }
    }

    public class ClientConfig
    {
        public string ClientPubKey { get; set; }
        public string ClientPrivKey { get; set; }
        public string ClientSWA { get; set; }
    }

    public class SessionConfig
    {
        public string SessionPubKey { get; set; }
        public string SessionPrivKey { get; set; }
        public string UserSWA { get; set; }
    }
} 