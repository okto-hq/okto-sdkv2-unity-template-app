using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.Features.SmartContract;
using OktoSDK.Models.Order;
using System.Collections;
using System.Numerics;
using System;

namespace OktoSDK.Example
{
    public class PrefabWorkflowExample : MonoBehaviour
    {
        [Header("Core Components")]
        [SerializeField] private PrefabManager prefabManager;
        
        [Header("Transaction Polling")]
        [SerializeField] private float checkIntervalInSeconds = 10f;
        
        private bool _isInitialized = false;
        private Coroutine _pollingCoroutine;
        private string _lastTransactionHash;
        private string _lastIntentId;
        private float _pollingStartTime;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                if (_isInitialized) return;

                // If prefabManager is not assigned, try to find an existing one that may have persisted from another scene
                if (prefabManager == null)
                {
                    prefabManager = FindObjectOfType<PrefabManager>();
                    if (prefabManager == null)
                    {
                        CustomLogger.Log("Error: PrefabManager reference is missing. Please assign it in the inspector.");
                        return;
                    }
                }

                prefabManager.OnWorkflowCompleted += OnWorkflowCompleted;
                prefabManager.OnWorkflowFailed += OnWorkflowFailed;
                
                // Add handler for transaction manager events
                if (prefabManager.transactionManager != null)
                {
                    prefabManager.transactionManager.OnTransactionCompleted += OnTransactionCompleted;
                    prefabManager.transactionManager.OnTransactionFailed += OnTransactionFailed;
                    prefabManager.transactionManager.OnOrderDetailsRetrieved += OnOrderDetailsRetrieved;
                }

                CustomLogger.Log("Ready");
                _isInitialized = true;
                CustomLogger.Log("PrefabWorkflowExample initialized successfully");
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error initializing: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            if (prefabManager != null)
            {
                prefabManager.OnWorkflowCompleted -= OnWorkflowCompleted;
                prefabManager.OnWorkflowFailed -= OnWorkflowFailed;
                
                if (prefabManager.transactionManager != null)
                {
                    prefabManager.transactionManager.OnTransactionCompleted -= OnTransactionCompleted;
                    prefabManager.transactionManager.OnTransactionFailed -= OnTransactionFailed;
                    prefabManager.transactionManager.OnOrderDetailsRetrieved -= OnOrderDetailsRetrieved;
                }
            }
            
            StopPollingTransactionStatus();
        }

        public async void SelectChain(string chainName)
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                    if (!_isInitialized)
                    {
                        CustomLogger.Log("Error: Component not initialized");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(chainName))
                {
                    CustomLogger.Log("Error: Chain name cannot be empty");
                    return;
                }

                CustomLogger.Log($"Selecting chain: {chainName}...");
                bool success = await prefabManager.StartWorkflow(chainName);

                CustomLogger.Log(success
                    ? $"Chain {chainName} selected successfully"
                    : $"Failed to select chain {chainName}");
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error selecting chain: {ex.Message}");
            }
        }

        public void SelectWallet(string walletAddress)
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                    if (!_isInitialized)
                    {
                        CustomLogger.Log("Error: Component not initialized");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(walletAddress))
                {
                    CustomLogger.Log("Error: Wallet address cannot be empty");
                    return;
                }

                CustomLogger.Log($"Selecting wallet: {walletAddress}...");
                bool success = prefabManager.SelectWallet(walletAddress);

                CustomLogger.Log(success
                    ? $"Wallet {walletAddress} selected successfully"
                    : $"Failed to select wallet {walletAddress}");
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error selecting wallet: {ex.Message}");
            }
        }

        public async void ExecuteTransaction(string contractAddress, string calldata = "0x", string value = "0x0")
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                    if (!_isInitialized)
                    {
                        CustomLogger.Log("Error: Component not initialized");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(contractAddress))
                {
                    CustomLogger.Log("Error: Contract address cannot be empty");
                    return;
                }

                if (string.IsNullOrEmpty(calldata)) calldata = "0x";
                if (string.IsNullOrEmpty(value)) value = "0x0";

                CustomLogger.Log("Executing transaction...");
                string txHash = await prefabManager.ExecuteTransaction(contractAddress, calldata, value);

                if (!string.IsNullOrEmpty(txHash))
                {
                    _lastTransactionHash = txHash;
                    CustomLogger.Log("Transaction executed successfully");
                    CustomLogger.Log($"Transaction hash: {txHash}");
                    
                    // Start polling for transaction status
                    StartPollingTransactionStatus();
                }
                else
                {
                    CustomLogger.Log("Failed to execute transaction");
                }
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error executing transaction: {ex.Message}");
            }
        }

        
        public void StartPollingTransactionStatus()
        {
            StopPollingTransactionStatus(); // Stop any existing polling
            _pollingStartTime = Time.realtimeSinceStartup; // Record start time
            _pollingCoroutine = StartCoroutine(PollTransactionStatus());
        }
        
        public void StopPollingTransactionStatus()
        {
            if (_pollingCoroutine != null)
            {
                try
                {
                    StopCoroutine(_pollingCoroutine);
                }
                catch (Exception ex)
                {
                    // This could happen if we're changing scenes
                    CustomLogger.LogWarning($"Error stopping coroutine: {ex.Message}");
                }
                _pollingCoroutine = null;
            }
        }
        
        private IEnumerator PollTransactionStatus()
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(_lastIntentId))
                {
                    // Calculate elapsed time
                    float elapsedTime = Time.realtimeSinceStartup - _pollingStartTime;
                    
                    // Start the async operation
                    var task = prefabManager.transactionManager.GetOrderDetailsByIntentId(_lastIntentId);
                    
                    // Wait until it completes
                    yield return new WaitUntil(() => task.IsCompleted);
                    
                    // Process the result
                    if (task.IsCompletedSuccessfully && task.Result.Item1 != null)
                    {
                        var order = task.Result.Item1;
                        // If transaction is completed (SUCCESS or FAILURE), stop polling
                        if (order.Status == "SUCCESS" || order.Status == "FAILURE" || 
                            order.Status == "SUCCESSFUL" || order.Status == "FAILED")
                        {
                            CustomLogger.Log($"Transaction status: {order.Status} - completed in {elapsedTime:F2} seconds");
                            
                            // Uncomment to enable auto-decoding
                            // if (order.Status == "SUCCESS" || order.Status == "SUCCESSFUL")
                            // {
                            //     AutoDecodeTransactionData(order);
                            // }
                            
                            break; // Exit the polling loop
                        }
                        else
                        {
                            CustomLogger.Log($"Current transaction status: {order.Status} - polling for {elapsedTime:F2} seconds");
                        }
                    }
                }
                
                // Wait before the next poll
                yield return new WaitForSeconds(checkIntervalInSeconds);
            }
        }
        

        private void OnWorkflowCompleted(CallDataDecoder.DecodedCallData decodedData)
        {
            CustomLogger.Log("Workflow completed successfully");
            CustomLogger.Log(decodedData.ToString());
        }

        private void OnWorkflowFailed(string stage, string error)
        {
            CustomLogger.Log($"Workflow failed at stage '{stage}': {error}");
        }
        
        private void OnTransactionCompleted(string txHash)
        {
            CustomLogger.Log($"Transaction completed with hash: {txHash}");
            _lastTransactionHash = txHash;
            
            // Get the intent ID for this transaction
            if (prefabManager?.transactionManager != null)
            {
                _lastIntentId = prefabManager.transactionManager.LatestIntentId;
                if (!string.IsNullOrEmpty(_lastIntentId))
                {
                    CustomLogger.Log($"Transaction intent ID: {_lastIntentId}");
                    StartPollingTransactionStatus();
                }
            }
        }
        
        private void OnTransactionFailed(string error)
        {
            CustomLogger.Log($"Transaction failed: {error}");
        }
        
        private void OnOrderDetailsRetrieved(Order order, string downstreamHash)
        {
            float elapsedTime = Time.realtimeSinceStartup - _pollingStartTime;
            CustomLogger.Log($"Received order details - Status: {order.Status}, Downstream hash: {downstreamHash}, Time: {elapsedTime:F2} seconds");
            
            // Stop polling if status is successful
            if (order.Status == "SUCCESS" || order.Status == "SUCCESSFUL")
            {
                StopPollingTransactionStatus();
                CustomLogger.Log($"Transaction completed successfully in {elapsedTime:F2} seconds");
            }
            
            // Handle different types of order details similar to TestOrder.cs
            if (order.Details is RawTransactionDetails raw)
            {
                CustomLogger.Log($"[RAW] CAIP2: {raw.Caip2Id}, Transactions Count: {raw.Transactions?.Count}");
                
                for (int i = 0; i < raw.Transactions?.Count; i++)
                {
                    var transactionList = raw.Transactions[i];
                    
                    // Iterate through each RawTransactionItem in the transaction list
                    for (int j = 0; j < transactionList.Count; j++)
                    {
                        var item = transactionList[j]; // This is a RawTransactionItem
                        CustomLogger.Log($"Transaction {i + 1}, Item {j + 1}: Key: {item.Key}, Value: {item.Value}");
                    }
                }
            }
            else if (order.Details is TokenTransferDetails token)
            {
                CustomLogger.Log("[TOKEN]");
                CustomLogger.Log($"Amount: {token.Amount}");
                CustomLogger.Log($"CAIP2 ID: {token.Caip2IdAlt}");
                CustomLogger.Log($"Recipient: {token.RecipientWalletAddress}");
                CustomLogger.Log($"Token Address: {token.TokenAddress}");
            }
            else if (order.Details is NftTransferDetails nft)
            {
                CustomLogger.Log("[NFT]");
                CustomLogger.Log($"Collection Address: {nft.CollectionAddress}");
                CustomLogger.Log($"NFT ID: {nft.NftId}");
                CustomLogger.Log($"Recipient Wallet: {nft.RecipientWalletAddress}");
                CustomLogger.Log($"Amount: {nft.Amount}");
                CustomLogger.Log($"NFT Type: {nft.NftType}");
            }
        }

        public async void RunFullWorkflow(string chainName, string contractAddress, string calldata, string value, string abiJson)
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                    if (!_isInitialized)
                    {
                        CustomLogger.Log("Error: Component not initialized");
                        return;
                    }
                }

                CustomLogger.Log($"Step 1: Selecting chain {chainName}...");
                bool chainSuccess = await prefabManager.StartWorkflow(chainName);
                if (!chainSuccess)
                {
                    CustomLogger.Log($"Failed to select chain {chainName}");
                    return;
                }

                await Task.Delay(1000);

                CustomLogger.Log("Step 2: Executing transaction...");
                string txHash = await prefabManager.ExecuteTransaction(contractAddress, calldata, value);
                if (string.IsNullOrEmpty(txHash))
                {
                    CustomLogger.Log("Failed to execute transaction");
                    return;
                }
                
                _lastTransactionHash = txHash;
                
                // Get intent ID if available
                if (prefabManager?.transactionManager != null)
                {
                    _lastIntentId = prefabManager.transactionManager.LatestIntentId;
                    if (!string.IsNullOrEmpty(_lastIntentId))
                    {
                        CustomLogger.Log($"Transaction intent ID: {_lastIntentId}");
                        
                        // Start polling for transaction status
                        StartPollingTransactionStatus();
                        
                        // Wait a bit for transaction to be processed
                        await Task.Delay(2000);
                        
                        // Try to get order details
                        CustomLogger.Log("Step 3: Getting order details...");
                        var (orderDetails, downstreamHash) = await prefabManager.transactionManager.GetOrderDetailsByIntentId(_lastIntentId);
                        
                        if (orderDetails != null)
                        {
                            CustomLogger.Log($"Order details retrieved - Status: {orderDetails.Status}");
                        }
                    }
                }

                CustomLogger.Log("Step 4: Decoding call data...");
                var callDataDecoder = prefabManager.GetComponent<CallDataDecoder>();
                if (callDataDecoder != null)
                {
                    var decodedData = callDataDecoder.DecodeCallDataDirectWrapper(calldata, abiJson);
                    if (decodedData.IsSuccessful)
                    {
                        CustomLogger.Log("Workflow completed successfully");
                        CustomLogger.Log(decodedData.ToString());
                    }
                    else
                    {
                        CustomLogger.Log($"Failed to decode call data: {decodedData.ErrorMessage}");
                    }
                }
                else
                {
                    CustomLogger.Log("CallDataDecoder component not found");
                }
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error in workflow: {ex.Message}");
            }
        }

        
        // Add helper method to generate deposit function calldata for testing
        public string GenerateDepositCalldata()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [
                        {""name"": ""amount"", ""type"": ""uint256""}
                    ],
                    ""name"": ""deposit"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "deposit";
                object[] parameters = new object[] { "1000000000000000000" }; // 1 ETH
                
                var abiEncoder = prefabManager.GetComponent<OktoSDK.Features.SmartContract.AbiEncoding>();
                if (abiEncoder != null)
                {
                    string encodedData = abiEncoder.EncodeFunctionData(abi, functionName, parameters);
                    CustomLogger.Log($"Generated deposit calldata: {encodedData}");
                    return encodedData;
                }
                else
                {
                    CustomLogger.Log("ABIEncoding component not found");
                    return null;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Log($"Error generating deposit calldata: {ex.Message}");
                return null;
            }
        }
        
        // Add a method that demonstrates the full deposit workflow
        public async void ExecuteAndDecodeDepositFunction(string contractAddress, string value = "0x0")
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                    if (!_isInitialized)
                    {
                        CustomLogger.Log("Error: Component not initialized");
                        return;
                    }
                }
                
                // Generate the deposit calldata
                string calldata = GenerateDepositCalldata();
                if (string.IsNullOrEmpty(calldata))
                {
                    CustomLogger.Log("Failed to generate deposit calldata");
                    return;
                }
                
                // Execute the transaction
                CustomLogger.Log($"Executing deposit transaction to contract: {contractAddress}");
                string txHash = await prefabManager.ExecuteTransaction(contractAddress, calldata, value);
                
                if (!string.IsNullOrEmpty(txHash))
                {
                    _lastTransactionHash = txHash;
                    CustomLogger.Log($"Deposit transaction executed with hash: {txHash}");
                    
                    // Start polling for transaction status
                    StartPollingTransactionStatus();
                }
                else
                {
                    CustomLogger.Log("Failed to execute deposit transaction");
                }
            }
            catch (System.Exception ex)
            {
                CustomLogger.Log($"Error in deposit workflow: {ex.Message}");
            }
        }

        private void OnEnable()
        {
            // Reattach to events if we were re-enabled (e.g., after a scene change)
            if (prefabManager != null && _isInitialized)
            {
                prefabManager.OnWorkflowCompleted += OnWorkflowCompleted;
                prefabManager.OnWorkflowFailed += OnWorkflowFailed;
                
                if (prefabManager.transactionManager != null)
                {
                    prefabManager.transactionManager.OnTransactionCompleted += OnTransactionCompleted;
                    prefabManager.transactionManager.OnTransactionFailed += OnTransactionFailed;
                    prefabManager.transactionManager.OnOrderDetailsRetrieved += OnOrderDetailsRetrieved;
                }
                
                // If we have an ongoing transaction, restart polling
                if (!string.IsNullOrEmpty(_lastIntentId))
                {
                    StartPollingTransactionStatus();
                }
            }
            else if (prefabManager != null && !_isInitialized)
            {
                // Initialize if we have the manager but weren't initialized
                Initialize();
            }
        }
        
        private void OnDisable()
        {
            // Stop polling when disabled
            StopPollingTransactionStatus();
            
            // We don't unsubscribe from events here as we might be disabled temporarily
            // This happens in OnDestroy instead
        }
    }
}
