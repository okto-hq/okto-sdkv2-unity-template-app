using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.Auth;
using OktoSDK.Features.Wallet;
using OktoSDK.Features.Order;
using OktoSDK.Features.Transaction;
using OktoSDK.Features.SmartContract;
using OktoSDK.Features.Network;
using OrderType = OktoSDK.Models.Order.Order;

namespace OktoSDK.Example
{
    /// <summary>
    /// Main feature manager that demonstrates the new organized structure
    /// This class shows how to use components from various feature namespaces
    /// </summary>
    public class OktoFeatureManager : MonoBehaviour
    {
        [Header("Authentication")]
        [SerializeField] private bool autoLogin = false;
        
        [Header("Feature References")]
        [SerializeField] private GameObject walletFeatureContainer;
        [SerializeField] private GameObject orderFeatureContainer;
        [SerializeField] private GameObject transactionFeatureContainer;
        [SerializeField] private GameObject smartContractFeatureContainer;
        [SerializeField] private GameObject networkFeatureContainer;
        
        // Features references - will be located in their respective folders
        private WalletManager _walletManager;
        private PrefabManager _prefabManager;
        private OrderPrefab _orderPrefab;
        private CallDataDecoder _callDataDecoder;
        
        // Event to notify when all features are initialized
        public event Action OnFeaturesInitialized;
        
        // Event to notify when authentication state changes
        public event Action<bool> OnAuthStateChanged;
        
        private bool _isInitialized = false;
        
        void Start()
        {
            // Initialize components when the scene starts
            InitializeFeatures();
            
            // Auto-login if enabled
            if (autoLogin)
            {
                AutoLogin();
            }
        }
        
        /// <summary>
        /// Initialize all feature components
        /// </summary>
        private void InitializeFeatures()
        {
            try
            {
                // Find and initialize wallet features
                if (walletFeatureContainer != null)
                {
                    _walletManager = walletFeatureContainer.GetComponentInChildren<WalletManager>();
                    if (_walletManager == null)
                    {
                        CustomLogger.LogWarning("WalletManager not found in walletFeatureContainer");
                    }
                }
                
                // Find and initialize transaction features
                if (transactionFeatureContainer != null)
                {
                    _prefabManager = transactionFeatureContainer.GetComponentInChildren<PrefabManager>();
                    if (_prefabManager == null)
                    {
                        CustomLogger.LogWarning("PrefabManager not found in transactionFeatureContainer");
                    }
                }
                
                // Find and initialize order features
                if (orderFeatureContainer != null)
                {
                    _orderPrefab = orderFeatureContainer.GetComponentInChildren<OrderPrefab>();
                    if (_orderPrefab == null)
                    {
                        CustomLogger.LogWarning("OrderPrefab not found in orderFeatureContainer");
                    }
                }
                
                // Find and initialize smart contract features
                if (smartContractFeatureContainer != null)
                {
                    _callDataDecoder = smartContractFeatureContainer.GetComponentInChildren<CallDataDecoder>();
                    if (_callDataDecoder == null)
                    {
                        CustomLogger.LogWarning("CallDataDecoder not found in smartContractFeatureContainer");
                    }
                }
                
                // Subscribe to auth state changes
                OktoAuthManager.OnAuthStateChanged += HandleAuthStateChanged;
                
                _isInitialized = true;
                CustomLogger.Log("OktoFeatureManager: All features initialized successfully");
                
                // Notify listeners
                OnFeaturesInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error initializing features: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handle authentication state changes
        /// </summary>
        private void HandleAuthStateChanged(bool isLoggedIn)
        {
            CustomLogger.Log($"Auth state changed: {(isLoggedIn ? "Logged in" : "Logged out")}");
            OnAuthStateChanged?.Invoke(isLoggedIn);
        }
        
        /// <summary>
        /// Automatically login using stored credentials if available
        /// </summary>
        public async void AutoLogin()
        {
            try
            {
                var isLoggedIn = OktoAuthManager.GetOktoClient()?.IsLoggedIn() ?? false;
                
                if (!isLoggedIn)
                {
                    CustomLogger.Log("Attempting auto-login...");
                    await OktoAuthManager.OLogin();
                }
                else
                {
                    CustomLogger.Log("Already logged in");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Auto-login failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Example method that uses multiple features together
        /// </summary>
        public async Task<bool> ExecuteTransactionDemo(string contractAddress, string data = "0x", string value = "0x0")
        {
            if (!_isInitialized)
            {
                CustomLogger.LogError("OktoFeatureManager is not initialized");
                return false;
            }
            
            try
            {
                // Check if user is authenticated
                if (!OktoAuthManager.GetOktoClient()?.IsLoggedIn() ?? false)
                {
                    CustomLogger.LogError("User is not logged in");
                    return false;
                }
                
                // Use wallet manager to get supported wallets
                var supportedWallets = await _walletManager.GetSupportedWallets("1"); // Ethereum chain ID
                if (supportedWallets.Count == 0)
                {
                    CustomLogger.LogError("No supported wallets found");
                    return false;
                }
                
                // Select the first wallet
                var walletSelected = _walletManager.SelectWallet(supportedWallets[0]);
                if (!walletSelected)
                {
                    CustomLogger.LogError("Failed to select wallet");
                    return false;
                }
                
                // Execute the transaction
                var txHash = await _prefabManager.ExecuteTransaction(contractAddress, data, value);
                
                if (string.IsNullOrEmpty(txHash))
                {
                    CustomLogger.LogError("Transaction execution failed");
                    return false;
                }
                
                CustomLogger.Log($"Transaction executed successfully. TX Hash: {txHash}");
                return true;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error executing transaction demo: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Example method showing how to fetch user orders
        /// </summary>
        public async Task<List<OrderType>> GetUserOrdersDemo()
        {
            if (!_isInitialized)
            {
                CustomLogger.LogError("OktoFeatureManager is not initialized");
                return new List<OrderType>();
            }
            
            try
            {
                // Check if user is authenticated
                if (!OktoAuthManager.GetOktoClient()?.IsLoggedIn() ?? false)
                {
                    CustomLogger.LogError("User is not logged in");
                    return new List<OrderType>();
                }
                
                // Use the OrderService to get orders
                var orders = await OrderService.GetOrdersHistory();
                CustomLogger.Log($"Found {orders.Count} orders for the user");
                return orders;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error getting user orders: {ex.Message}");
                return new List<OrderType>();
            }
        }
        
        /// <summary>
        /// Clean up resources when the object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            // Unsubscribe from events
            OktoAuthManager.OnAuthStateChanged -= HandleAuthStateChanged;
        }
    }
} 