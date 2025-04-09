

namespace OktoSDK
{
    public static class Constants
    {
        public static readonly long HOURS_IN_MS = 60 * 60 * 1000;

        public static readonly string EXECUTE_USEROP_FUNCTION_SELECTOR = "0x8dd7712f";

        public static readonly string FUNCTION_NAME = "initiateJob";

        public static readonly ulong USEROP_VALUE = 0;

        public static readonly string MaxPriorityFeePerGas = "0xBA43B7400";

        public static readonly string MaxFeePerGas = "0xBA43B7400";

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
            BffBaseUrl = "https://3p-bff.oktostage.com",
            PaymasterAddress = "0xdAa292E9B9a6B287c84d636F3b65f4A5Dc787e3f",
            JobManagerAddress = "0xd57F1802d164Ae465363ec3F2d62cbf6fc7dfF23",
            EntryPointContractAdress = "0xec3F5f7a3f0e43e61D8711A90B8c8Fc59B9a88ba",
            ChainId = 124736089
        };
    }
}
