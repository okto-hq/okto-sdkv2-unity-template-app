using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OktoSDK
{
    // Separate class for handling order responses only
    public class OrderResponsePanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject orderResponsePanel;

        [SerializeField]
        private TextMeshProUGUI orderResponseMsg;

        [SerializeField]
        private Button orderCloseBtn;

        private static OrderResponsePanel _orderResponsePanel;

        private void OnEnable()
        {
            _orderResponsePanel = this;
            orderCloseBtn.onClick.AddListener(CloseOrderResponse);
        }

        private void OnDisable()
        {
            orderCloseBtn.onClick.RemoveListener(CloseOrderResponse);
        }

        public static void CloseOrderResponse()
        {
            _orderResponsePanel.orderResponsePanel.SetActive(false);
        }

        public static void SetOrderResponse(string data)
        {
            CustomLogger.Log("SetOrderResponse");

            if (_orderResponsePanel == null)
                return;

            Loader.DisableLoader();
            _orderResponsePanel.orderResponseMsg.text = data;
            _orderResponsePanel.orderResponsePanel.SetActive(true);
        }
    }
} 