using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OrderType = OktoSDK.Models.Order.Order;

namespace OktoSDK.Features.Order
{
    /// <summary>
    /// Component for retrieving orders from the API.
    /// Wrapper for OrderService that can be attached to a GameObject.
    /// </summary>
    public class GetOrder : MonoBehaviour
    {
        /// <summary>
        /// Get all orders from the API
        /// </summary>
        /// <returns>List of orders</returns>
        public async Task<List<OrderType>> GetOrdersHistory()
        {
            return await OrderService.GetOrdersHistory();
        }
        
        /// <summary>
        /// Get a specific order by intent ID
        /// </summary>
        /// <param name="intentId">The intent ID to search for</param>
        /// <returns>The order if found, null otherwise</returns>
        public async Task<OrderType> GetOrderByIntentId(string intentId)
        {
            return await OrderService.GetOrderByIntentId(intentId);
        }
        
        /// <summary>
        /// Filter orders by intent type
        /// </summary>
        /// <param name="intentType">The intent type to filter by, or null for all orders</param>
        /// <returns>Filtered list of orders</returns>
        public async Task<List<OrderType>> GetOrdersByIntentType(string intentType)
        {
            return await OrderService.GetOrdersByIntentType(intentType);
        }
        
        /// <summary>
        /// Get detailed order information including downstream transaction hash
        /// </summary>
        /// <param name="intentId">The intent ID of the order</param>
        /// <returns>Tuple containing the order and its downstream transaction hash</returns>
        public async Task<(OrderType, string)> GetOrderDetailsByIntentId(string intentId)
        {
            return await OrderService.GetOrderDetailsByIntentId(intentId);
        }
    }
} 