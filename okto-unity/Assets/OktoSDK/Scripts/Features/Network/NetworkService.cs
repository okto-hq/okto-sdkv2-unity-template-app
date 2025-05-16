using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OktoSDK.BFF;

namespace OktoSDK.Features.Network
{
    /// <summary>
    /// Service class for network-related operations.
    /// </summary>
    public class NetworkService
    {
        /// <summary>
        /// Get all supported networks
        /// </summary>
        /// <returns>List of supported networks</returns>
        public static async Task<List<NetworkData>> GetSupportedNetworks()
        {
            try
            {
                var bffNetworks = await BffClientRepository.GetSupportedNetworks();
                
                // Convert BFF networks to model networks
                List<NetworkData> modelNetworks = new List<NetworkData>();
                foreach (var network in bffNetworks)
                {
                    modelNetworks.Add(new NetworkData
                    {
                        chainId = network.chainId,
                        networkId = network.networkId,
                        networkName = network.networkName,
                        gsnEnabled = network.gsnEnabled,
                        sponsorshipEnabled = network.sponsorshipEnabled
                    });
                }
                
                return modelNetworks;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve supported networks: {error}");
                throw new Exception("Failed to retrieve networks. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get a network by name
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        /// <returns>The network if found, null otherwise</returns>
        public static async Task<NetworkData> GetNetworkByName(string networkName)
        {
            try
            {
                if (string.IsNullOrEmpty(networkName))
                    return null;
                    
                var networks = await GetSupportedNetworks();
                
                foreach (var network in networks)
                {
                    if (network.networkName.Equals(networkName, StringComparison.OrdinalIgnoreCase))
                    {
                        return network;
                    }
                }
                
                return null;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to find network by name: {error}");
                return null;
            }
        }
        
        /// <summary>
        /// Check if a network supports the given chain ID
        /// </summary>
        /// <param name="networkData">The network to check</param>
        /// <param name="chainId">The chain ID to check</param>
        /// <returns>True if the network supports the chain, false otherwise</returns>
        public static bool SupportsChain(NetworkData networkData, string chainId)
        {
            if (networkData == null || string.IsNullOrEmpty(chainId))
                return false;
                
            // Networks typically have a 1:1 relationship with chains in EVM
            return networkData.chainId.Equals(chainId, StringComparison.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Get all supported tokens
        /// </summary>
        /// <returns>List of supported tokens</returns>
        public static async Task<List<Token>> GetSupportedTokens()
        {
            try
            {
                var bffTokens = await BffClientRepository.GetSupportedTokens();
                
                // Convert BFF tokens to model tokens
                List<Token> modelTokens = new List<Token>();
                foreach (var token in bffTokens)
                {
                    modelTokens.Add(new Token
                    {
                        address = token.address,
                        symbol = token.symbol,
                        name = token.name,
                        networkId = token.networkId,
                        networkName = token.networkName
                    });
                }
                
                return modelTokens;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve supported tokens: {error}");
                throw new Exception("Failed to retrieve tokens. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get tokens for a specific network
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        /// <returns>List of tokens for the network</returns>
        public static async Task<List<Token>> GetTokensForNetwork(string networkName)
        {
            try
            {
                if (string.IsNullOrEmpty(networkName))
                    return new List<Token>();
                    
                var tokens = await GetSupportedTokens();
                var result = new List<Token>();
                
                foreach (var token in tokens)
                {
                    if (token.networkName.Equals(networkName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(token);
                    }
                }
                
                return result;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to get tokens for network: {error}");
                return new List<Token>();
            }
        }
    }
} 
