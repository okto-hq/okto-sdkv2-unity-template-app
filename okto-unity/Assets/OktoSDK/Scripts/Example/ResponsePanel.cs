using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This script is responsible to show responses for all api's
namespace OktoSDK
{
    public class ResponsePanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject responsePanel;

        [SerializeField]
        private TextMeshProUGUI responseMsg;

        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private GameObject orderResponsePanel;

        [SerializeField]
        private TextMeshProUGUI orderResponseMsg;

        private static ResponsePanel _responsePanel;

        private void OnEnable()
        {
            _responsePanel = this;
            closeBtn.onClick.AddListener(CloseResponse);
        }

        private void OnDisable()
        {
            closeBtn.onClick.RemoveListener(CloseResponse);
        }

        public static void CloseResponse()
        {
            _responsePanel.responsePanel.SetActive(false);
        }


        public static void SetResponse(string data)
        {
            Loader.DisableLoader();
            _responsePanel.responseMsg.text = data;
            _responsePanel.responsePanel.SetActive(true);
        }

        public static void SetOrderResponse(string data)
        {
            Loader.DisableLoader();
            _responsePanel.orderResponseMsg.text = data;
            _responsePanel.orderResponsePanel.SetActive(true);

        }
    }
}