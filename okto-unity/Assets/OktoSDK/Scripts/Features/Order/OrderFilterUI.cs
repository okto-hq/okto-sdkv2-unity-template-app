using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using OrderType = OktoSDK.Models.Order.Order;
using OktoSDK.Auth;

namespace OktoSDK.Features.Order
{
    /// <summary>
    /// UI component for filtering orders by intent ID and intent type.
    /// Uses OrderService for business logic.
    /// </summary>
    public class OrderFilterUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField intentId;

        [SerializeField]
        private TMP_Dropdown intentType;

        [SerializeField]
        private TextMeshProUGUI intentLabel;

        [SerializeField]
        private Button searchIntentIdBtn;

        // Event to notify listeners about filtered orders
        public event Action<List<OrderType>> OnOrdersFiltered;

        private List<OrderType> orderList;
        private List<OrderType> filteredOrderList = new List<OrderType>();

        private void OnEnable()
        {
            LoadOrders();
            searchIntentIdBtn.onClick.AddListener(SearchOrdersByIntentID);
            intentType.onValueChanged.AddListener(SearchOrdersByIntentType);
        }

        private void OnDisable()
        {
            searchIntentIdBtn.onClick.RemoveListener(SearchOrdersByIntentID);
            intentType.onValueChanged.RemoveListener(SearchOrdersByIntentType);
        }

        /// <summary>
        /// Load orders from the API
        /// </summary>
        public async void LoadOrders()
        {
            if (OktoAuthManager.GetOktoClient() == null || !OktoAuthManager.GetOktoClient().IsLoggedIn())
            {
                OrderResponsePanel.SetOrderResponse("You are not logged In!");
                return;
            }

            intentId.text = string.Empty;
            
            try
            {
                orderList = await OrderService.GetOrdersHistory();
                // Reset dropdown to initial value
                intentType.value = 0;
                
                // Display all orders initially
                DisplayOrders(orderList);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Failed to load orders: {ex.Message}");
                OrderResponsePanel.SetOrderResponse($"Failed to load orders: {ex.Message}");
            }
        }

        /// <summary>
        /// Filter orders by intent type
        /// </summary>
        private void SearchOrdersByIntentType(int index)
        {
            // Clear the intent ID field to avoid confusion
            intentId.text = string.Empty;

            try
            {
                string selectedType = intentType.options[index].text;
                filteredOrderList = OrderService.FilterOrdersByIntentType(orderList, selectedType);
                DisplayOrders(filteredOrderList);
            }
            catch (Exception e)
            {
                CustomLogger.LogError($"Error filtering by intent type: {e.Message}");
                OrderResponsePanel.SetOrderResponse($"Error filtering orders: {e.Message}");
            }
        }

        /// <summary>
        /// Filter orders by intent ID
        /// </summary>
        private void SearchOrdersByIntentID()
        {
            try
            {
                string idToSearch = intentId.text.Trim();
                
                if (string.IsNullOrEmpty(idToSearch))
                {
                    OrderResponsePanel.SetOrderResponse("Please enter an Intent ID");
                    return;
                }
                
                filteredOrderList = OrderService.FilterOrdersByIntentId(orderList, idToSearch);
                DisplayOrders(filteredOrderList);
            }
            catch (Exception e)
            {
                CustomLogger.LogError($"Error filtering by intent ID: {e.Message}");
                OrderResponsePanel.SetOrderResponse($"Error filtering orders: {e.Message}");
            }
        }

        /// <summary>
        /// Display the filtered orders
        /// </summary>
        private void DisplayOrders(List<OrderType> orders)
        {
            if (orders == null || orders.Count == 0)
            {
                OrderResponsePanel.SetOrderResponse("No Orders found!");
                return;
            }

            string newOrderStr = JsonConvert.SerializeObject(orders, Formatting.Indented);
            OrderResponsePanel.SetOrderResponse(newOrderStr);

            // Trigger event for any listeners
            OnOrdersFiltered?.Invoke(orders);
        }
    }
} 
