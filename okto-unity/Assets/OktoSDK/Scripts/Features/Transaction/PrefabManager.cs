using OktoSDK.Features.SmartContract;
using OktoSDK.Features.Transaction;
using OktoSDK.Features.Wallet;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OktoSDK.Example
{
    /// <summary>
    /// Main manager class that coordinates the blockchain interaction workflow:
    /// 1. Feed chain name -> 2. Trigger supported chain/wallets -> 3. Call ethereumRaw -> 4. Decode call data
    /// </summary>
    public class PrefabManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private ChainManager chainManager;
        [SerializeField] private WalletManager walletManager;
        public TransactionManager transactionManager;
        
        [Header("Configuration")]
        [SerializeField] private string defaultAbiJson = "[]";
        [SerializeField] private bool autoSelectWallet = true;
        
        // Event that fires when the entire process is complete
        public event Action<DecodingCore.DecodedCallData> OnWorkflowCompleted;
        
        // Event that fires when any step of the workflow fails
        public event Action<string, string> OnWorkflowFailed;
        
        private bool _isInitialized = false;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            InitializeComponents();
        }
        
        /// <summary>
        /// Initializes all components and sets up event handlers
        /// </summary>
        private void InitializeComponents()
        {
            try
            {
                if (_isInitialized) return;
                
                // Check all required components
                if (chainManager == null)
                {
                    CustomLogger.LogError("ChainManager reference is missing");
                    return;
                }
                
                if (walletManager == null)
                {
                    CustomLogger.LogError("WalletManager reference is missing");
                    return;
                }
                
                if (transactionManager == null)
                {
                    CustomLogger.LogError("TransactionManager reference is missing");
                    return;
                }
                
                
                // Set up event handlers
                chainManager.OnChainSelected += OnChainSelected;
                walletManager.OnWalletSelected += OnWalletSelected;
                transactionManager.OnTransactionCompleted += OnTransactionCompleted;
                transactionManager.OnTransactionFailed += OnTransactionFailed;
                
                _isInitialized = true;
                CustomLogger.Log("PrefabManager initialized successfully");
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error initializing PrefabManager: {ex.Message}");
                OnWorkflowFailed?.Invoke("initialization", ex.Message);
            }
        }
        
        private void OnDestroy()
        {
            // Clean up event handlers
            if (chainManager != null) chainManager.OnChainSelected -= OnChainSelected;
            if (walletManager != null) walletManager.OnWalletSelected -= OnWalletSelected;
            if (transactionManager != null)
            {
                transactionManager.OnTransactionCompleted -= OnTransactionCompleted;
                transactionManager.OnTransactionFailed -= OnTransactionFailed;
            }
        }
        
        /// <summary>
        /// Starts the workflow by selecting a chain
        /// </summary>
        /// <param name="chainName">The name of the chain to select</param>
        /// <returns>Task representing the async operation</returns>
        public async Task<bool> StartWorkflow(string chainName)
        {
            try
            {
                if (!_isInitialized)
                {
                    InitializeComponents();
                    if (!_isInitialized)
                    {
                        OnWorkflowFailed?.Invoke("initialization", "Failed to initialize components");
                        return false;
                    }
                }
                
                CustomLogger.Log($"Starting workflow with chain: {chainName}");
                bool success = await chainManager.SelectChain(chainName);
                return success;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error starting workflow: {ex.Message}");
                OnWorkflowFailed?.Invoke("workflow_start", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Called when a chain is selected
        /// </summary>
        private async void OnChainSelected(string chainId)
        {
            try
            {
                CustomLogger.Log($"Chain selected: {chainId}");
                
                // Get supported wallets for the selected chain
                var supportedWallets = await walletManager.GetSupportedWallets(chainId);
                
                if (supportedWallets.Count == 0)
                {
                    string error = $"No supported wallets found for chain {chainId}";
                    CustomLogger.LogError(error);
                    OnWorkflowFailed?.Invoke("wallet_selection", error);
                    return;
                }
                
                // Auto-select wallet if enabled
                if (autoSelectWallet)
                {
                    walletManager.AutoSelectWallet();
                }
                else
                {
                    CustomLogger.Log($"Found {supportedWallets.Count} supported wallets. Please select one manually.");
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in OnChainSelected: {ex.Message}");
                OnWorkflowFailed?.Invoke("wallet_selection", ex.Message);
            }
        }
        
        /// <summary>
        /// Manually selects a wallet by address
        /// </summary>
        /// <param name="walletAddress">The wallet address to select</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SelectWallet(string walletAddress)
        {
            try
            {
                if (!_isInitialized)
                {
                    InitializeComponents();
                    if (!_isInitialized)
                    {
                        OnWorkflowFailed?.Invoke("initialization", "Failed to initialize components");
                        return false;
                    }
                }
                
                return walletManager.SelectWallet(walletAddress);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error selecting wallet: {ex.Message}");
                OnWorkflowFailed?.Invoke("wallet_selection", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Called when a wallet is selected
        /// </summary>
        private async void OnWalletSelected(string walletAddress)
        {
            CustomLogger.Log($"Wallet selected: {walletAddress}");
            CustomLogger.Log("Ready to execute transactions. Call ExecuteTransaction() to proceed.");

            string txn = await ExecuteTransaction("0xFc4aFB0CD4C6227a578Fb230275683d68C8db015","1");
        }
        
        /// <summary>
        /// Executes a transaction using the selected chain and wallet
        /// </summary>
        /// <param name="toAddress">The recipient address</param>
        /// <param name="data">The transaction calldata (hex string)</param>
        /// <param name="value">The transaction value in wei (hex string)</param>
        /// <returns>Task representing the async operation</returns>
        public async Task<string> ExecuteTransaction(string toAddress, string value = "0x0", string data = "0x")
        {
            try
            {
                if (!_isInitialized)
                {
                    InitializeComponents();
                    if (!_isInitialized)
                    {
                        OnWorkflowFailed?.Invoke("initialization", "Failed to initialize components");
                        return null;
                    }
                }
                
                string fromAddress = walletManager.GetSelectedWalletAddress();
                
                if (string.IsNullOrEmpty(fromAddress))
                {
                    string error = "No wallet selected. Select a wallet before executing a transaction.";
                    CustomLogger.LogError(error);
                    OnWorkflowFailed?.Invoke("transaction_execution", error);
                    return null;
                }
                
                return await transactionManager.ExecuteTransaction(fromAddress, toAddress, data, value);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error executing transaction: {ex.Message}");
                OnWorkflowFailed?.Invoke("transaction_execution", ex.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Called when a transaction is completed
        /// </summary>
        private void OnTransactionCompleted(string txHash)
        {
            CustomLogger.Log($"Transaction completed with hash: {txHash}");
            CustomLogger.Log("To decode call data from an order, call DecodeCallDataFromOrder()");
        }
        
        /// <summary>
        /// Called when a transaction fails
        /// </summary>
        private void OnTransactionFailed(string error)
        {
            CustomLogger.LogError($"Transaction failed: {error}");
            OnWorkflowFailed?.Invoke("transaction_execution", error);
        }
        
        
        /// <summary>
        /// Called when call data decoding fails
        /// </summary>
        private void OnDecodingFailed(string error)
        {
            CustomLogger.LogError($"Call data decoding failed: {error}");
            OnWorkflowFailed?.Invoke("call_data_decoding", error);
        }
    }
} 
