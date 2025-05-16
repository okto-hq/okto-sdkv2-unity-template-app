using System.Threading.Tasks;
using UnityEngine;
using OrderType = OktoSDK.Models.Order.Order;

namespace OktoSDK.Features.Order
{
    public class OrderPrefab : MonoBehaviour
    {
        // Fetches the order matching the intentId
        public async Task<OrderType> CallOrderApi(string intentId)
        {
            return await OrderService.GetOrderByIntentId(intentId);
        }

        public async Task<(OrderType, string)> GetOrderDetailsByIntentId(string intentId)
        {
            return await OrderService.GetOrderDetailsByIntentId(intentId);
        }
    }
}