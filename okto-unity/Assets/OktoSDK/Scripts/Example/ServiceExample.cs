using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using OktoSDK.Features.Wallet;
using OktoSDK.Features.Order;
using OktoSDK.Features.Network;
using OktoSDK.Features.Wallet.Features.Wallet;

namespace OktoSDK
{
    /// <summary>
    /// Example demonstrating the use of service classes
    /// </summary>
    public class ServiceExample : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button getOrdersButton;
        [SerializeField] private Button getWalletsButton;
        [SerializeField] private Button getNetworksButton;
        [SerializeField] private TextMeshProUGUI resultText;
        
        [Header("Optional References")]
        [SerializeField] private WalletManager walletManager;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (getOrdersButton != null)
                getOrdersButton.onClick.AddListener(GetOrdersExample);
                
            if (getWalletsButton != null)
                getWalletsButton.onClick.AddListener(GetWalletsExample);
                
            if (getNetworksButton != null)
                getNetworksButton.onClick.AddListener(GetNetworksExample);
        }

        private void OnDestroy()
        {
            if (getOrdersButton != null)
                getOrdersButton.onClick.RemoveListener(GetOrdersExample);
                
            if (getWalletsButton != null)
                getWalletsButton.onClick.RemoveListener(GetWalletsExample);
                
            if (getNetworksButton != null)
                getNetworksButton.onClick.RemoveListener(GetNetworksExample);
        }

        /// <summary>
        /// Example showing how to use OrderService
        /// </summary>
        public async void GetOrdersExample()
        {
            try
            {
                CustomLogger.Log("Fetching orders using OrderService...");
                
                // Get all orders
                var orders = await OrderService.GetOrdersHistory();
                
                // Filter by intent type (example)
                var filteredOrders = OrderService.FilterOrdersByIntentType(orders, "RAW_TRANSACTION");
                
                DisplayResults($"Found {orders.Count} total orders, {filteredOrders.Count} Raw Transaction orders");
            }
            catch (Exception ex)
            {
                DisplayError($"Error fetching orders: {ex.Message}");
            }
        }

        /// <summary>
        /// Example showing how to use WalletService
        /// </summary>
        public async void GetWalletsExample()
        {
            try
            {
                CustomLogger.Log("Fetching wallets using WalletService...");
                
                // Get all wallets
                var wallets = await WalletService.GetWallets();
                
                // Get wallets for a specific network (example)
                var ethereumWallets = await WalletService.GetWalletsForNetwork("ETHEREUM");
                
                DisplayResults($"Found {wallets.Count} total wallets, {ethereumWallets.Count} Ethereum wallets");
                
                // Example of using WalletManager with WalletService
                if (walletManager != null && wallets.Count > 0)
                {
                    var supportedWallets = await walletManager.GetSupportedWallets("1"); // Ethereum chain ID
                    DisplayResults($"WalletManager found {supportedWallets.Count} supported wallets for chain ID 1");
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Error fetching wallets: {ex.Message}");
            }
        }

        /// <summary>
        /// Example showing how to use NetworkService
        /// </summary>
        public async void GetNetworksExample()
        {
            try
            {
                CustomLogger.Log("Fetching networks using NetworkService...");
                
                // Get all networks
                var networks = await NetworkService.GetSupportedNetworks();
                
                // Get tokens for a specific network (example)
                var ethereumTokens = await NetworkService.GetTokensForNetwork("ETHEREUM");
                
                DisplayResults($"Found {networks.Count} networks, {ethereumTokens.Count} Ethereum tokens");
            }
            catch (Exception ex)
            {
                DisplayError($"Error fetching networks: {ex.Message}");
            }
        }

        private void DisplayResults(string message)
        {
            CustomLogger.Log(message);
            
            if (resultText != null)
                resultText.text = message;
        }

        private void DisplayError(string errorMessage)
        {
            CustomLogger.LogError(errorMessage);
            
            if (resultText != null)
                resultText.text = $"ERROR: {errorMessage}";
        }
    }
} 