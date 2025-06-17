using System;
using System.Collections.Generic;
using UnityEngine;
using OktoSDK.Features.Wallet;
using OktoSDK.BFF;
using OktoSDK.Auth;

namespace OktoSDK
{
    public class AccountTest : MonoBehaviour
    {
        [SerializeField]
        private string targetNetwork;

        [SerializeField]
        private AccountPrefab accountPrefab;

        [SerializeField]
        private string ownAddress;

        private void OnEnable()
        {
            OktoAuthManager.Instance.OnAuthenticationComplete += OnLoggedIn;
        }

        private void OnDisable()
        {
            OktoAuthManager.Instance.OnAuthenticationComplete -= OnLoggedIn;
        }

        void OnLoggedIn(bool status)
        {
            if (status)
            {
                 CallAccountApi();
            }
        }

        public async void CallAccountApi()
        {
            if (accountPrefab != null)
            {
                List<Wallet> allWallets = await accountPrefab.CallAccountApi();

                // Find the first wallet with the target network name
                Wallet filteredWallet = allWallets.Find(wallet =>
                    wallet.networkName.Equals(targetNetwork, StringComparison.OrdinalIgnoreCase));

                if (filteredWallet != null)
                {
                    ownAddress = filteredWallet.address;
                    CustomLogger.Log($"Found Wallet => Address: {filteredWallet.address}, Network: {filteredWallet.networkName}");
                }
                else
                {
                    CustomLogger.LogError("No wallet found for the specified network.");
                }

            }
            else
            {
                CustomLogger.LogError("AccountPrefab reference not set in AccountTest.");
            }
        }

    }
}
