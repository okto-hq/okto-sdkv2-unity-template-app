using OktoSDK.Auth;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OktoSDK.OnRamp
{
    public class OnRampUiManager : MonoBehaviour
    {
        [SerializeField]
        private Button closeTokenPanelBtn;

        [SerializeField]
        private Button openTokenPanelBtn;

        [SerializeField]
        private TokenItem tokenItem;

        [SerializeField]
        private Transform tokenListParent;

        [SerializeField]
        private List<TokenItem> tokensList;

        [SerializeField]
        private GameObject tokenPanel;

        private void OnEnable()
        {
            closeTokenPanelBtn.onClick.AddListener(ClearTokens);
            openTokenPanelBtn.onClick.AddListener(CallOnRampApi);
        }

        private void OnDisable()
        {
            closeTokenPanelBtn.onClick.RemoveListener(ClearTokens);
            openTokenPanelBtn.onClick.RemoveListener(CallOnRampApi);
        }

        private bool EnsureLoggedIn()
        {
            var oc = OktoAuthManager.GetOktoClient();

            if (oc == null || !oc.IsLoggedIn())
            {
                string message = "You are not logged In!";
                ResponsePanel.SetResponse(message);
                CustomLogger.Log(message);
                return false;
            }

            return true;
        }

        async void CallOnRampApi()
        {
            if (!EnsureLoggedIn()) return;

            Loader.ShowLoader();
            try
            {
                string authToken = await OktoAuthManager.GetOktoClient().GetAuthorizationToken();
                List<WhitelistedToken> response = await SupportedOnRampToken.FetchStatus(authToken);
                SuccessCallBack(response);
            }
            catch (Exception e)
            {
                ResponsePanel.SetResponse("Error fetching onramp data");
            }
        }

        async void SuccessCallBack(List<WhitelistedToken> whitelistedTokens)
        {
            try
            {
                if (whitelistedTokens == null || whitelistedTokens.Count == 0)
                {
                    CustomLogger.Log("No OnRamp tokens active!");
                    return;
                }

                ClearTokens();
                for (int i = 0; i < whitelistedTokens.Count; i++)
                {
                    TokenItem go = Instantiate<TokenItem>(tokenItem, tokenListParent);

                    await ImageLoader.LoadImageFromURL(whitelistedTokens[i].Image, go.tokenImg);
                    go.tokenName.text = whitelistedTokens[i].ShortName;
                    go.whitelistedToken = whitelistedTokens[i];
                    go.gameObject.SetActive(true);
                    CustomLogger.Log(go.tokenName.text);
                    tokensList.Add(go);
                }
                OpenTokenPanel();

            }
            catch (Exception e)
            {
                ResponsePanel.SetResponse("Fetching onRamp token list failed!");
            }
        }

        void ClearTokens()
        {
            for (int i = 0; i < tokensList.Count; i++)
            {
                Destroy(tokensList[i].gameObject);
            }
            tokensList.Clear();
            tokenPanel.SetActive(false);
        }

        void OpenTokenPanel()
        {
            Loader.DisableLoader();
            tokenPanel.SetActive(true);
        }
    }
}