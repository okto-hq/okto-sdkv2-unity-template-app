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
            //GatewayBaseUrl = "https://sandbox-okto-gateway.oktostage.com",
            BffBaseUrl = "https://sandbox-api.okto.tech",
            PaymasterAddress = "0x74324fA6Fa67b833dfdea4C1b3A9898574d076e3",
            JobManagerAddress = "0x0543aD80b41C5f5956d34503668CDb965deCB617",
            EntryPointContractAdress = "0xCa5b1b0d3893b5152014fD5B519FF50f7C40f9da",
            ChainId = 1802466136,
            OnRampBaseUrl = "https://sandbox-pay.okto.tech",
            LoginOAuth = "https://sandbox-onboarding.okto.tech/__/auth/handler",
        };

        public static readonly EnvConfig StagingEnvConfig = new EnvConfig
        {
            //GatewayBaseUrl = "https://okto-gateway.oktostage.com",
            BffBaseUrl = "https://3p-bff.oktostage.com",
            PaymasterAddress = "0xc2D31Cdc6EFd02F85Ab943c4587f8D60E6E15F9c",
            JobManagerAddress = "0x57820589F31a9e4a34A0299Ea4aDe7c536139682",
            EntryPointContractAdress = "0x322eF240AD89d19a50Ca092CF70De9603bf6778E",
            ChainId = 124736089,
            OnRampBaseUrl = "https://pay.oktostage.com",
            LoginOAuth = "https://onboarding.oktostage.com/__/auth/handler",
        };

        public static readonly EnvConfig ProductionEnvConfig = new EnvConfig
        {
            //GatewayBaseUrl = "https://okto-gateway.okto.tech",
            BffBaseUrl = "https://apigw.okto.tech",
            PaymasterAddress = "0xB0E2BD2EFb99F982F8cCB8e6737A572B3B0eCE11",
            JobManagerAddress = "0x7F1E1e98Dde775Fae0d340D3E5D28004Db58A0d3",
            EntryPointContractAdress = "0x0b643Bcd21a72b10075F1938Ebebba6E077A1742",
            ChainId = 8088,
            OnRampBaseUrl = "https://pay.okto.tech",
            LoginOAuth = "https://onboarding.okto.tech/__/auth/handler",
        };

    }
}
