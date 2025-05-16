using Nethereum.ABI.Model;

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

        public static readonly string FEE_PAYER_ADDRESS = "0x0000000000000000000000000000000000000000";

        public static readonly EnvConfig SandboxEnvConfig = new EnvConfig
        {
            GatewayBaseUrl = "https://sandbox-okto-gateway.oktostage.com",
            BffBaseUrl = "https://sandbox-api.okto.tech",
            PaymasterAddress = "0x74324fA6Fa67b833dfdea4C1b3A9898574d076e3",
            JobManagerAddress = "0x0543aD80b41C5f5956d34503668CDb965deCB617",
            EntryPointContractAdress = "0xCa5b1b0d3893b5152014fD5B519FF50f7C40f9da",
            ChainId = 1802466136,
            OnRampBaseUrl = "https://sandbox-pay.okto.tech",
            LoginOAuth = "https://sandbox-onboarding.okto.tech/__/auth/handler",
        };

        public static readonly EnvConfig ProductionEnvConfig = new EnvConfig
        {
            GatewayBaseUrl = "https://okto-gateway.oktostage.com",
            BffBaseUrl = "https://3p-bff.oktostage.com",
            PaymasterAddress = "0xdAa292E9B9a6B287c84d636F3b65f4A5Dc787e3f",
            JobManagerAddress = "0xb5e77f7Ff1ab31Fc1bE99F484DB62f01a6b93D4d",
            EntryPointContractAdress = "0xec3F5f7a3f0e43e61D8711A90B8c8Fc59B9a88ba",
            ChainId = 124736089,
            OnRampBaseUrl = "https://pay.oktostage.com",
            LoginOAuth = "https://onboarding.oktostage.com/__/auth/handler",
        };
    }
}
