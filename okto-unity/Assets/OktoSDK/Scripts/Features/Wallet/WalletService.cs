using OktoSDK.BFF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletType = OktoSDK.BFF.Wallet;

namespace OktoSDK.Features.Wallet.Features.Wallet
{
    /// <summary>
    /// Service class for wallet-related operations.
    /// </summary>
    public class WalletService
    {
        /// <summary>
        /// Get all wallets for the current user
        /// </summary>
        /// <returns>List of user wallets</returns>
        public static async Task<List<WalletType>> GetWallets()
        {
            try
            {
                return await BffClientRepository.GetWallets();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve wallets: {error}");
                throw new Exception("Failed to retrieve wallets. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get a wallet by address
        /// </summary>
        /// <param name="address">The wallet address</param>
        /// <returns>The wallet if found, null otherwise</returns>
        public static async Task<WalletType> GetWalletByAddress(string address)
        {
            try
            {
                if (string.IsNullOrEmpty(address))
                    return null;
                    
                var wallets = await GetWallets();
                
                foreach (var wallet in wallets)
                {
                    if (wallet.address.Equals(address, StringComparison.OrdinalIgnoreCase))
                    {
                        return wallet;
                    }
                }
                
                return null;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to get wallet by address: {error}");
                return null;
            }
        }
        
        /// <summary>
        /// Get wallets for a specific network
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        /// <returns>List of wallets for the network</returns>
        public static async Task<List<WalletType>> GetWalletsForNetwork(string networkName)
        {
            try
            {
                if (string.IsNullOrEmpty(networkName))
                    return new List<WalletType>();
                    
                var wallets = await GetWallets();
                var result = new List<WalletType>();
                
                foreach (var wallet in wallets)
                {
                    if (wallet.networkName.Equals(networkName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(wallet);
                    }
                }
                
                return result;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to get wallets for network: {error}");
                return new List<WalletType>();
            }
        }
        
        /// <summary>
        /// Check if a wallet supports the given chain ID
        /// </summary>
        /// <param name="wallet">The wallet to check</param>
        /// <param name="chainId">The chain ID to check for</param>
        /// <returns>True if the wallet supports the chain, false otherwise</returns>
        public static bool SupportsChain(WalletType wallet, string chainId)
        {
            if (wallet == null || string.IsNullOrEmpty(chainId))
                return false;
                
            // For now, we assume all wallets on a network support all chains that network supports
            // This can be refined later based on actual requirements
            var network = wallet.networkName;
            var cap2Id = wallet.cap2Id;
            
            return !string.IsNullOrEmpty(network) && !string.IsNullOrEmpty(cap2Id);
        }
        
        /// <summary>
        /// Get portfolio data for the current user
        /// </summary>
        /// <returns>Portfolio data</returns>
        public static async Task<UserPortfolioData> GetPortfolioData()
        {
            try
            {
                return await BffClientRepository.GetPortfolio();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve portfolio data: {error}");
                throw new Exception("Failed to retrieve portfolio data. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get portfolio activity for the current user
        /// </summary>
        /// <returns>List of portfolio activities</returns>
        public static async Task<List<UserPortfolioActivity>> GetPortfolioActivity()
        {
            try
            {
                return await BffClientRepository.GetPortfolioActivity();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve portfolio activity: {error}");
                throw new Exception("Failed to retrieve portfolio activity. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get NFT balances for the current user
        /// </summary>
        /// <returns>List of NFT balances</returns>
        public static async Task<List<UserNFTBalance>> GetNFTBalances()
        {
            try
            {
                return await BffClientRepository.GetPortfolioNft();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve NFT balances: {error}");
                throw new Exception("Failed to retrieve NFT balances. Please try again later.");
            }
        }
    }
} 
