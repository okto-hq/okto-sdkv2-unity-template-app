using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OktoSDK.OnRamp
{
    public class TokenItem : MonoBehaviour
    {
        public Image tokenImg;
        public TextMeshProUGUI tokenName;

        public Button onRampBtn;
        public WhitelistedToken whitelistedToken;

        private HomeController homeController;

        private void OnEnable()
        {
            if (homeController == null)
            {
                homeController = FindObjectOfType<HomeController>();
                if (homeController == null)
                {
                    CustomLogger.LogError("HomeController not found in the scene.");
                }
            }
            onRampBtn.onClick.AddListener(CallOnRampApi);
        }

        private void OnDisable()
        {
            onRampBtn.onClick.RemoveListener(CallOnRampApi);
        }

        void CallOnRampApi()
        {
            Loader.ShowLoader();
            try
            {
                if (homeController == null)
                    homeController = FindObjectOfType<HomeController>();

                OnRampWebViewController.CallOnRamp(whitelistedToken.Id, whitelistedToken, homeController);
            }
            catch (Exception e)
            {

                ResponsePanel.SetResponse("Error fetching onramp data");
            }
        }
    }
}