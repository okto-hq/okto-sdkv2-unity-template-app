using System;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;
using Newtonsoft.Json;
using OrderType = OktoSDK.Models.Order.Order;
using System.Collections.Generic;
using OktoSDK.Features.Order;

namespace OktoSDK.Features.Transaction
{
    /// <summary>
    /// Manages blockchain transactions through the EVM Raw interface
    /// </summary>
    public class TransactionManager : MonoBehaviour
    {
        [SerializeField] private EvmRawPrefab evmRawPrefab;
        [SerializeField] private OrderPrefab orderPrefab;
        
        // Dictionary to store transaction hashes and their corresponding intent IDs
        private Dictionary<string, string> transactionIntentIds = new Dictionary<string, string>();
        
        // Property to access the latest intent ID
        public string LatestIntentId { get; private set; }
        
        // Event that fires when a transaction is completed with the transaction hash
        public event Action<string> OnTransactionCompleted;
        
        // Event that fires when a transaction fails with the error message
        public event Action<string> OnTransactionFailed;
        
        // Event that fires when order details are retrieved
        public event Action<OrderType, string> OnOrderDetailsRetrieved;
        
        /// <summary>
        /// Executes an EVM Raw transaction with the specified parameters
        /// </summary>
        /// <param name="fromAddress">The sender address</param>
        /// <param name="toAddress">The recipient address</param>
        /// <param name="data">The transaction calldata (hex string)</param>
        /// <param name="value">The transaction value in wei (hex string)</param>
        /// <returns>Task representing the async operation with transaction hash</returns>
        public async Task<string> ExecuteTransaction(string fromAddress, string toAddress, string data, string value = "0x0")
        {
            try
            {
                if (evmRawPrefab == null)
                {
                    string error = "EvmRawPrefab reference is missing. Please assign it in the inspector.";
                    CustomLogger.LogError(error);
                    OnTransactionFailed?.Invoke(error);
                    return null;
                }
                
                if (string.IsNullOrEmpty(fromAddress) || string.IsNullOrEmpty(toAddress))
                {
                    string error = "From address and to address must be specified.";
                    CustomLogger.LogError(error);
                    OnTransactionFailed?.Invoke(error);
                    return null;
                }
                
                // Use the evmRawPrefab to execute the transaction
                string txHashResult;
                if (string.IsNullOrEmpty(data) || data == "0x")
                {
                    // Simple transfer transaction
                    txHashResult = await evmRawPrefab.CallEvmRaw(fromAddress, toAddress, value, "0x");
                }
                else
                {
                    // Smart contract interaction
                    txHashResult = await evmRawPrefab.CallEvmRaw(fromAddress, toAddress, value, data);
                }
                
                // Process the response to extract the Intent ID and transaction hash
                if (!string.IsNullOrEmpty(txHashResult))
                {
                    try
                    {
                        // Parse the JSON response
                        var response = JsonConvert.DeserializeObject<BFF.JsonRpcResponse<ExecuteResult>>(txHashResult);
                        
                        if (response?.result?.jobId != null)
                        {
                            string txHash = response.result.jobId;
                            
                            // Store the intent ID if available
                            if (txHash != null) 
                            {
                                LatestIntentId = txHash;
                                transactionIntentIds[txHash] = txHash;
                                CustomLogger.Log($"Stored Intent ID: {txHash}");
                            }
                            
                            OnTransactionCompleted?.Invoke(txHash);
                            return txHash;
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LogError($"Error parsing transaction response: {ex.Message}");
                    }
                }
                
                CustomLogger.Log($"Transaction result: {txHashResult}");
                OnTransactionCompleted?.Invoke(txHashResult);
                return txHashResult;
            }
            catch (Exception ex)
            {
                string error = $"Error executing transaction: {ex.Message}";
                CustomLogger.LogError(error);
                OnTransactionFailed?.Invoke(error);
                return null;
            }
        }
        
        /// <summary>
        /// Get the Intent ID for a given transaction hash
        /// </summary>
        /// <param name="txHash">Transaction hash</param>
        /// <returns>The Intent ID or null if not found</returns>
        public string GetIntentIdForTransaction(string txHash)
        {
            if (transactionIntentIds.TryGetValue(txHash, out string intentId))
            {
                return intentId;
            }
            return null;
        }
        
        /// <summary>
        /// Gets order details by intent ID
        /// </summary>
        /// <param name="intentId">Intent ID to query</param>
        /// <returns>Task representing the async operation</returns>
        public async Task<(OrderType, string)> GetOrderDetailsByIntentId(string intentId = null)
        {
            // Use the latest intent ID if none is provided
            if (string.IsNullOrEmpty(intentId))
            {
                intentId = LatestIntentId;
            }
            
            if (string.IsNullOrEmpty(intentId))
            {
                CustomLogger.LogError("No intent ID available. Execute a transaction first.");
                return (null, null);
            }
            
            if (orderPrefab == null)
            {
                CustomLogger.LogError("OrderPrefab reference is missing. Please assign it in the inspector.");
                return (null, null);
            }
            
            try
            {
                var (orderType, downstreamHash) = await orderPrefab.GetOrderDetailsByIntentId(intentId);
                OnOrderDetailsRetrieved?.Invoke(orderType, downstreamHash);
                return (orderType, downstreamHash);

            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error getting order details: {ex.Message}");
                return (null, null);
            }
        }
    }
} 
