using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

        private async void OpenRawTransaction()
        {
            Loader.ShowLoader();

            try
            {
                chainList.options.Clear();
                walletList.Clear();

                walletList =
                (List<Wallet>)await account.GetAccount(OktoAuthExample.getOktoClient());


                foreach (var item in walletList)
                {
                    chainList.options.Add(new TMP_Dropdown.OptionData(item.networkName));
                }

                CallNetWorkApi();
            }
            catch (Exception e)
            {
                Debug.Log("execption : " + e.Message);
            }
        }

        private async void CallNetWorkApi()
        {
            networkList =
              (List<NetworkData>)await chain.GetChains(OktoAuthExample.getOktoClient());
            SelectChain(chainList.value);

        }

        private void SetChain()
        {
            try
            {
                for (int i = 0; i < walletList.Count; i++)
                {
                    if (walletList[i].networkName.ToLower().Equals(chainList.options[chainList.value].text.ToLower()))
                    {
                        Debug.Log("SetCurrentNetwork " + walletList[i].capId);
                        TokenTransferView.SetNetwork(walletList[i].capId);
                        break;
                    }
                }

                for (int i = 0; i < networkList.Count; i++)
                {
                    if (networkList[i].caipId.Equals(TokenTransferView.GetNetwork()))
                    {
                        TokenTransferController.SetCurrentChain(networkList[i]);
                        Debug.Log("SetCurrent_Chain " + JsonConvert.SerializeObject(networkList[i]));
                        break;

                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("execption : " + e.Message);
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