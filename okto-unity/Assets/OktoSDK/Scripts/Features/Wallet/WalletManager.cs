using OktoSDK.Features.Wallet.Features.Wallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OktoSDK.Features.Wallet
{
    /// <summary>
    /// Manages wallet selection and connection based on the selected chain
    /// </summary>
    public class WalletManager : MonoBehaviour
    {
        // Event that fires when a wallet is selected with the wallet address
        public event Action<string> OnWalletSelected;
        
        private string _selectedWalletAddress;
        
        // Keep track of the supported wallets for the current chain
        private List<string> _supportedWallets = new List<string>();
        
        /// <summary>
        /// Called when a chain is selected to determine the supported wallets
        /// </summary>
        /// <param name="chainId">The selected chain ID</param>
        /// <returns>List of supported wallet addresses</returns>
        public async Task<List<string>> GetSupportedWallets(string chainId)
        {
            try
            {
                // Get accounts and filter by selected chain
                var accounts = await WalletService.GetWallets();
                _supportedWallets.Clear();
                
                if (accounts != null && accounts.Count > 0)
                {
                    foreach (var account in accounts)
                    {
                        // For now, assume all wallets support all chains
                        // This can be refined later based on actual requirements
                        if (account.networkName != null && 
                            !string.IsNullOrEmpty(account.address))
                        {
                            _supportedWallets.Add(account.address);
                        }
                    }
                }
                
                CustomLogger.Log($"Found {_supportedWallets.Count} supported wallets for chain {chainId}");
                return _supportedWallets;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error fetching supported wallets: {ex.Message}");
                return new List<string>();
            }
        }
        
        /// <summary>
        /// Selects a wallet by address and triggers the OnWalletSelected event
        /// </summary>
        /// <param name="walletAddress">The wallet address to select</param>
        /// <returns>True if wallet selection succeeded, false otherwise</returns>
        public bool SelectWallet(string walletAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(walletAddress))
                {
                    CustomLogger.LogError("Wallet address cannot be empty");
                    return false;
                }
                
                if (_supportedWallets.Count == 0 || !_supportedWallets.Contains(walletAddress))
                {
                    CustomLogger.LogError($"Wallet {walletAddress} is not supported for the selected chain");
                    return false;
                }
                
                _selectedWalletAddress = walletAddress;
                CustomLogger.Log($"Wallet selected: {walletAddress}");
                
                // Trigger the event with the selected wallet address
                OnWalletSelected?.Invoke(_selectedWalletAddress);
                
                return true;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error selecting wallet {walletAddress}: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets the currently selected wallet address
        /// </summary>
        /// <returns>The selected wallet address, or null if no wallet is selected</returns>
        public string GetSelectedWalletAddress()
        {
            return _selectedWalletAddress;
        }
        
        /// <summary>
        /// Automatically selects the first available wallet for the selected chain
        /// </summary>
        /// <returns>True if auto-selection succeeded, false otherwise</returns>
        public bool AutoSelectWallet()
        {
            if (_supportedWallets.Count > 0)
            {
                return SelectWallet(_supportedWallets[0]);
            }
            
            CustomLogger.LogError("No supported wallets available for auto-selection");
            return false;
        }
    }
} 
