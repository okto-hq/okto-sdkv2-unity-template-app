using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OktoSDK.BFF;
using OktoSDK.Auth;

// This script handles the selection of the blockchain network from a dropdown UI element.
// It updates the current chain and network ID (capID) dynamically based on user selection.
//
// Usage:
// - Attach this script to a GameObject in the scene.
// - Ensure the dropdown component is properly assigned in the Inspector.
// - The selected network will determine the active blockchain configuration.

namespace OktoSDK
{
    public class TokenTransferUiManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown chainList;

        [SerializeField]
        private GameObject transferPanel;

        [SerializeField]
        private Button transferTokenTransaction;

        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private Account account;

        [SerializeField]
        private Chain chain;

        public List<NetworkData> networkList;

        public List<Wallet> walletList;

        private void OnEnable()
        {
            walletList = new List<Wallet>();
            transferTokenTransaction.onClick.AddListener(OpenRawTransaction);
            chainList.onValueChanged.AddListener(SelectChain);
            closeBtn.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            transferTokenTransaction.onClick.RemoveListener(OpenRawTransaction);
            chainList.onValueChanged.RemoveListener(SelectChain);
            closeBtn.onClick.AddListener(Close);
        }

        void Close()
        {
            transferPanel.SetActive(false);
            TokenTransferView.OnClose();
        }

        private bool EnsureLoggedIn()
        {
            var oc = OktoAuthManager.GetOktoClient();

            if (oc == null || !oc.IsLoggedIn())
            {
                string message = "You are not logged In!";
                ResponsePanel.SetResponse(message);
                return false;
            }

            return true;
        }

        private async void OpenRawTransaction()
        {
            if (!EnsureLoggedIn()) return;

            Loader.ShowLoader();

            try
            {
                chainList.options.Clear();
                walletList.Clear();

                walletList =
                (List<Wallet>)await account.GetWallets();

                foreach (var item in walletList)
                {
                    chainList.options.Add(new TMP_Dropdown.OptionData(item.networkName));
                }

                CallNetWorkApi();
            }
            catch (Exception e)
            {
                CustomLogger.Log("execption : " + e.Message);
            }
        }

        private async void CallNetWorkApi()
        {
            networkList = await chain.GetChains();
            SelectChain(0);

        }

        private void SetChain()
        {
            try
            {
                for (int i = 0; i < walletList.Count; i++)
                {
                    if (walletList[i].networkName.ToLower().Equals(chainList.options[chainList.value].text.ToLower()))
                    {
                        CustomLogger.Log("SetCurrentNetwork " + walletList[i].capId);
                        TokenTransferView.SetNetwork(walletList[i].capId);
                        break;
                    }
                }

                for (int i = 0; i < networkList.Count; i++)
                {
                    if (networkList[i].caipId.Equals(TokenTransferView.GetNetwork()))
                    {
                        TransactionConstants.CurrentChain = networkList[i];
                        CustomLogger.Log("SetCurrent_Chain " + JsonConvert.SerializeObject(networkList[i]));
                        break;

                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log("execption : " + e.Message);
            }

            Loader.DisableLoader();
            transferPanel.SetActive(true);

        }

        private void SelectChain(int index)
        {
            Loader.ShowLoader();
            SetChain();
        }
    }
}