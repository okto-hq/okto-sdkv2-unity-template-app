using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OktoSDK.Auth;
using OktoSDK.BFF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script handles the selection of the blockchain network from a dropdown UI element.
// It updates the current chain and network ID (capID) dynamically based on user selection.
//
// Usage:
// - Attach this script to a GameObject in the scene.
// - Ensure the dropdown component is properly assigned in the Inspector.
// - The selected network will determine the active blockchain configuration.
// - This script works for EVM Raw Panel

namespace OktoSDK
{
    public class EvmUiManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown chainList;

        [SerializeField]
        private GameObject emvPanel;

        [SerializeField]
        private Button evmRawTransaction;

        [SerializeField]
        private TMP_InputField address;

        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private Account account;

        [SerializeField]
        private Chain chain;

        public List<BFF.Wallet> walletList;

        public List<NetworkData> networkList;

        private void OnEnable()
        {
            walletList = new List<Wallet>();
            evmRawTransaction.onClick.AddListener(OpenRawTransaction);
            chainList.onValueChanged.AddListener(SelectChain);
            closeBtn.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            chainList.options.Clear();
            evmRawTransaction.onClick.RemoveListener(OpenRawTransaction);
            chainList.onValueChanged.RemoveListener(SelectChain);
            closeBtn.onClick.AddListener(Close);

        }

        void Close()
        {
            emvPanel.SetActive(false);
            EvmRawView.OnClose();
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

                walletList = await account.GetWallets();

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
                        CustomLogger.Log("SetCurrent_Network " + walletList[i].capId);
                        address.text = walletList[i].address;
                        EvmRawView.SetNetwork(walletList[i].capId);
                        break;
                    }
                }

                for (int i = 0; i < networkList.Count; i++)
                {
                    if (networkList[i].caipId.Equals(EvmRawView.GetNetwork()))
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
            emvPanel.SetActive(true);
        }

        private void SelectChain(int index)
        {
            Loader.ShowLoader();
            SetChain();
        }

    }
}