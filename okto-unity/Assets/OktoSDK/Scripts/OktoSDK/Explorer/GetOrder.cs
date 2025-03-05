using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which called GetOrder() Api
namespace OktoSDK
{
    public class GetOrder : MonoBehaviour
    {

        public async Task<object> GetOrdersHistory(OktoClient oc, OrderFilterRequest filters = null)
        {

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetOrderResponse("You are not logged In!");
                //ResponsePanel.DisplayResponse();
                return "You are not logged In!";
            }

            try
            {
                return await BffClientRepository.GetBffClientRepository().GetOrders(filters);
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to retrieve orders: {error}");
                throw new Exception("Failed to retrieve orders. Please try again later.");
            }
        }
    }
}