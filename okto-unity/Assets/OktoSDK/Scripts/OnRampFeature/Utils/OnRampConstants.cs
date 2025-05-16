namespace OktoSDK.OnRamp
{
    public static class OnRampConstants
    {
        public static readonly EnvConfig SandboxEnvConfig = new EnvConfig
        {
            GatewayBaseUrl = "https://sandbox-okto-gateway.oktostage.com",
            BffBaseUrl = "https://sandbox-api.okto.tech",
            PaymasterAddress = "0x74324fA6Fa67b833dfdea4C1b3A9898574d076e3",
            JobManagerAddress = "0x0543aD80b41C5f5956d34503668CDb965deCB617",
            EntryPointContractAdress = "0xCa5b1b0d3893b5152014fD5B519FF50f7C40f9da",
            ChainId = 1802466136
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
