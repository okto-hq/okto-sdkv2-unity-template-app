using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OktoSDK.Features.Transaction
{
    /// <summary>
    /// Manages chain selection and triggers events when a chain is selected
    /// </summary>
    public class ChainManager : MonoBehaviour
    {
        [SerializeField] private NetworkPrefab networkPrefab;

        // Event that fires when a chain is selected with the chain information
        public event Action<string> OnChainSelected;
        
        private string _selectedChainId;
        
        /// <summary>
        /// Selects a chain by name and triggers the OnChainSelected event
        /// </summary>
        /// <param name="chainName">The name of the chain to select</param>
        /// <returns>Task representing the async operation</returns>
        public async Task<bool> SelectChain(string chainName)
        {
            try
            {
                if (networkPrefab == null)
                {
                    CustomLogger.LogError("NetworkPrefab reference is missing. Please assign it in the inspector.");
                    return false;
                }

                string chainId = await networkPrefab.GetSelectedNetwork(chainName);
                
                if (string.IsNullOrEmpty(chainId))
                {
                    CustomLogger.LogError($"Chain '{chainName}' not found or not supported.");
                    return false;
                }

                _selectedChainId = chainId;
                CustomLogger.Log($"Chain selected: {chainName} with ID: {chainId}");
                
                // Trigger the event with the selected chain ID
                OnChainSelected?.Invoke(_selectedChainId);
                
                return true;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error selecting chain {chainName}: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets the currently selected chain ID
        /// </summary>
        /// <returns>The selected chain ID, or null if no chain is selected</returns>
        public string GetSelectedChainId()
        {
            return _selectedChainId;
        }
    }
} 
