using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OktoSDK;
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
// - This script works for NFT Transfer Panel

namespace OktoSDK
{
    public class NftUiManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown chainList;

        [SerializeField]
        private GameObject nftPanel;

        [SerializeField]
        private Button nftTransaction;

        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private Chain chain;

        [SerializeField]
        private Account account;

        public List<Wallet> walletList;

        public List<NetworkData> networkList;


        private void OnEnable()
        {
            walletList = new List<Wallet>();
            nftTransaction.onClick.AddListener(OpenRawTransaction);
            chainList.onValueChanged.AddListener(SelectChain);
            closeBtn.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            chainList.options.Clear();
            nftTransaction.onClick.RemoveListener(OpenRawTransaction);
            chainList.onValueChanged.RemoveListener(SelectChain);
            closeBtn.onClick.AddListener(Close);
        }

        void Close()
        {
            nftPanel.SetActive(false);
            NftTransferView.OnClose();
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
                        NftTransferView.SetNetwork(walletList[i].capId);
                        break;
                    }
                }

                for (int i = 0; i < networkList.Count; i++)
                {
                    if (networkList[i].caipId.Equals(NftTransferView.GetNetwork()))
                    {
                        NftTransferController.SetCurrentChain(networkList[i]);
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
            nftPanel.SetActive(true);

        }

        private void SelectChain(int index)
        {
            Loader.ShowLoader();

            SetChain();
        }
    }
}