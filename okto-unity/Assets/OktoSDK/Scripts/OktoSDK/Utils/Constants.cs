

namespace OktoSDK
{
    public static class Constants
    {
        public static readonly long HOURS_IN_MS = 60 * 60 * 1000;

        public static readonly string EXECUTE_USEROP_FUNCTION_SELECTOR = "0x8dd7712f";

        public static readonly string FUNCTION_NAME = "initiateJob";

        public static readonly ulong USEROP_VALUE = 0;

    
        public static readonly EnvConfig SandboxEnvConfig = new EnvConfig
        {
            GatewayBaseUrl = "https://sandbox-okto-gateway.oktostage.com",
            BffBaseUrl = "https://sandbox-api.okto.tech",
            PaymasterAddress = "0x5408fAa7F005c46B85d82060c532b820F534437c",
            JobManagerAddress = "0x21E822446C32FA22b29392F29597ebdcFd8511f8",
            EntryPointContractAdress = "0xA5E95a08229A816c9f3902E4a5a618C3928ad3bA",
            ChainId = 8801
        };

        public static readonly EnvConfig ProductionEnvConfig = new EnvConfig
        {
            GatewayBaseUrl = "https://okto-gateway.oktostage.com",
            BffBaseUrl = "https://apigw.oktostage.com",
            PaymasterAddress = "0x0871051BfF8C7041c985dEddFA8eF63d23AD3Fa0",
            JobManagerAddress = "0xED3D17cae886e008D325Ad7c34F3bdE030B80c2E",
            EntryPointContractAdress = "0x8D29ECb381CA4874767Ef3744F6df37748B12715",
            ChainId = 24879
        };
    }
} 