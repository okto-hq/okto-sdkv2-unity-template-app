using OktoSDK.BFF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderType = OktoSDK.Models.Order.Order;

namespace OktoSDK.Features.Order
{
    /// <summary>
    /// Service class for order-related operations.
    /// Provides a clean API for fetching and filtering orders.
    /// </summary>
    public class OrderService
    {
        /// <summary>
        /// Get all orders from the API
        /// </summary>
        /// <returns>List of orders</returns>
        public static async Task<List<OrderType>> GetOrdersHistory()
        {
            try
            {
                return await BffClientRepository.GetOrders(null);
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve orders: {error}");
                throw new Exception("Failed to retrieve orders. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get a specific order by intent ID
        /// </summary>
        /// <param name="intentId">The intent ID to search for</param>
        /// <returns>The order if found, null otherwise</returns>
        public static async Task<OrderType> GetOrderByIntentId(string intentId)
        {
            try
            {
                if (string.IsNullOrEmpty(intentId))
                    return null;
                    
                List<OrderType> orders = await BffClientRepository.GetOrders(new OrderFilterRequest { intentId = intentId });
                
                if (orders != null && orders.Count > 0)
                    return orders[0];
                    
                return null;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve order by intent ID: {error}");
                return null;
            }
        }
        
        /// <summary>
        /// Filter orders by intent type
        /// </summary>
        /// <param name="intentType">The intent type to filter by, or null for all orders</param>
        /// <returns>Filtered list of orders</returns>
        public static async Task<List<OrderType>> GetOrdersByIntentType(string intentType)
        {
            try
            {
                if (string.IsNullOrEmpty(intentType) || intentType.ToUpper() == "ALL_TRANSACTION")
                    return await GetOrdersHistory();
                    
                return await BffClientRepository.GetOrders(new OrderFilterRequest { intentType = intentType });
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve orders by intent type: {error}");
                throw new Exception("Failed to retrieve orders. Please try again later.");
            }
        }
        
        /// <summary>
        /// Get detailed order information including downstream transaction hash
        /// </summary>
        /// <param name="intentId">The intent ID of the order</param>
        /// <returns>Tuple containing the order and its downstream transaction hash</returns>
        public static async Task<(OrderType, string)> GetOrderDetailsByIntentId(string intentId)
        {
            OrderType order = await GetOrderByIntentId(intentId);
            if (order?.Details == null) return (null, null);

            return (order, order.DownstreamTransactionHash != null && order.DownstreamTransactionHash.Count > 0 && !string.IsNullOrEmpty(order.DownstreamTransactionHash[0])
                ? order.DownstreamTransactionHash[0]
                : null);
        }

        /// <summary>
        /// Filter a list of orders by intent type
        /// </summary>
        /// <param name="orders">The list of orders to filter</param>
        /// <param name="intentType">The intent type to filter by</param>
        /// <returns>Filtered list of orders</returns>
        public static List<OrderType> FilterOrdersByIntentType(List<OrderType> orders, string intentType)
        {
            if (orders == null) return new List<OrderType>();
            
            if (string.IsNullOrEmpty(intentType) || intentType.ToUpper() == "ALL_TRANSACTION")
                return orders;
                
            List<OrderType> result = new List<OrderType>();
            foreach (var order in orders)
            {
                if (order.IntentType.ToUpper().Equals(intentType.ToUpper()))
                {
                    result.Add(order);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Filter a list of orders by intent ID
        /// </summary>
        /// <param name="orders">The list of orders to filter</param>
        /// <param name="intentId">The intent ID to filter by</param>
        /// <returns>Filtered list of orders</returns>
        public static List<OrderType> FilterOrdersByIntentId(List<OrderType> orders, string intentId)
        {
            if (orders == null || string.IsNullOrEmpty(intentId)) 
                return new List<OrderType>();
                
            List<OrderType> result = new List<OrderType>();
            foreach (var order in orders)
            {
                if (order.IntentId.ToUpper().Equals(intentId.ToUpper()))
                {
                    result.Add(order);
                }
            }
            
            return result;
        }
    }
} 
