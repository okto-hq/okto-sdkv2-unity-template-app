using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Reflection;

namespace OktoSDK
{
    //This script handles seraching through Intent_ID and Intent_type of Orders
    public class OrderFilter : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField intentId;

        [SerializeField]
        private TMP_Dropdown intentType;

        [SerializeField]
        private TextMeshProUGUI intentlabel;

        [SerializeField]
        private Button searchIntentIdBtn;

        [SerializeField]
        private GetOrder order;

        private List<Order> orderList;

        public List<Order> newOrderList;

        private void OnEnable()
        {
            CallApi();
            searchIntentIdBtn.onClick.AddListener(SearchOrdersByIntentID);
            intentType.onValueChanged.AddListener(SearchOrdersByIntentType);

        }

        private void OnDisable()
        {
            searchIntentIdBtn.onClick.RemoveListener(SearchOrdersByIntentID);
            intentType.onValueChanged.RemoveListener(SearchOrdersByIntentType);

        }

        async void CallApi()
        {
            intentId.text = string.Empty;

            newOrderList = new List<Order>();
            orderList = (List<Order>)await order.GetOrdersHistory(OktoAuthExample.getOktoClient());

            intentType.value = 0;
        }

        private void SearchOrdersByIntentType(int index)
        {
            intentId.text = string.Empty;

            try
            {
                //newOrderList = new List<Order>();
                //orderList = (List<Order>)await order.GetOrdersHistory(OktoAuthExample.getOktoClient());

                newOrderList.Clear();
                for (int i = 0; i < orderList.Count; i++)
                {
                    if (intentType.options[index].text == "ALL_TRANSACTION")
                    {
                        newOrderList.Add(orderList[i]);
                    }
                    else if (orderList[i].intentType.ToUpper().Equals(intentType.options[index].text.ToUpper()))
                    {
                        newOrderList.Add(orderList[i]);
                    }
                }

                string newOrderStr = JsonConvert.SerializeObject(newOrderList, Formatting.Indented);
                ResponsePanel.SetOrderResponse(newOrderStr);
            }
            catch (Exception e)
            {
                Debug.Log("execption : " + e.Message);
            }

        }

        private void SearchOrdersByIntentID()
        {
            try
            {
                newOrderList.Clear();

                for (int i = 0; i < orderList.Count; i++)
                {
                    if (orderList[i].intentId.ToUpper().Equals(intentId.text.ToUpper()))
                    {
                        newOrderList.Add(orderList[i]);
                    }
                }

                string newOrderStr = JsonConvert.SerializeObject(newOrderList, Formatting.Indented);
                ResponsePanel.SetOrderResponse(newOrderStr);
            }
            catch (Exception e)
            {
                Debug.Log("execption : " + e.Message);
            }

        }

    }
}