
using OktoSDK.Auth;

namespace OktoSDK
{
    /// <summary>
    /// Helper class to provide consistent environment configuration access across the codebase
    /// </summary>
    public static class EnvironmentHelper
    {
        /// <summary>
        /// Gets the current environment configuration from OktoClient
        /// </summary>
        public static EnvConfig GetCurrentEnvConfig()
        {
            // Get from the OktoClient instance
            return OktoAuthManager.GetOktoClient()?.Env;
        }

        /// <summary>
        /// Gets the BFF base URL for API requests
        /// </summary>
        public static string GetBffBaseUrl()
        {
            return GetCurrentEnvConfig()?.BffBaseUrl;
        }

        /// <summary>
        /// Gets the Gateway base URL for API requests
        /// </summary>
        public static string GetGatewayBaseUrl()
        {
            return GetCurrentEnvConfig()?.GatewayBaseUrl;
        }

        /// <summary>
        /// Gets the Paymaster address
        /// </summary>
        public static string GetPaymasterAddress()
        {
            CustomLogger.Log("GetPaymasterAddress() " + GetCurrentEnvConfig()?.PaymasterAddress);
            return GetCurrentEnvConfig()?.PaymasterAddress;
        }

        /// <summary>
        /// Gets the Job Manager address
        /// </summary>
        public static string GetJobManagerAddress()
        {
            return GetCurrentEnvConfig()?.JobManagerAddress;
        }

        /// <summary>
        /// Gets the Entry Point contract address
        /// </summary>
        public static string GetEntryPointAddress()
        {
            return GetCurrentEnvConfig()?.EntryPointContractAdress;
        }

        /// <summary>
        /// Gets the Chain ID
        /// </summary>
        public static int GetChainId()
        {
            return GetCurrentEnvConfig()?.ChainId ?? 0;
        }
    }
} 