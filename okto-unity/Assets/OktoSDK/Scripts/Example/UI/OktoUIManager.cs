using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using OktoSDK.BFF;
using OktoSDK.Auth;

namespace OktoSDK
{
    public class OktoUIManager : MonoBehaviour
    {
        [System.Serializable]
        public class WalletListWrapper
        {
            public List<Wallet> wallets;
        }

        // Button click handlers
        public async void OnGetAccountButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var wallets = await BffClientRepository.GetWallets();
                string jsonString = JsonConvert.SerializeObject(wallets, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Account", e);
            }
        }

        public async void OnGetChainsButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var networks = await BffClientRepository.GetSupportedNetworks();
                string jsonString = JsonConvert.SerializeObject(networks, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Chains", e);
            }
        }

        public async void OnGetNFTCollectionsButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var collections = await BffClientRepository.GetPortfolioNft();
                string jsonString = JsonConvert.SerializeObject(collections, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get NFT Collections", e);
            }
        }

        public async void OnGetOrdersHistoryButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var orders = await BffClientRepository.GetOrders();
                string jsonString = JsonConvert.SerializeObject(orders, Formatting.Indented);
                OrderResponsePanel.SetOrderResponse(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Orders History", e);
            }
        }

        public async void OnGetPortfolioButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var portfolioData = await BffClientRepository.GetPortfolio();
                string jsonString = JsonConvert.SerializeObject(portfolioData, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Portfolio", e);
            }
        }

        public async void OnGetPortfolioActivityButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var portfolioData = await BffClientRepository.GetPortfolioActivity();
                string jsonString = JsonConvert.SerializeObject(portfolioData, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Portfolio Activity", e);
            }
        }

        public async void OnGetTokensButtonClick()
        {
            if (!EnsureLoggedIn()) return;

            try
            {
                Loader.ShowLoader();

                var tokens = await BffClientRepository.GetSupportedTokens();
                string jsonString = JsonConvert.SerializeObject(tokens, Formatting.Indented);
                DisplayResult(jsonString);
            }
            catch (System.Exception e)
            {
                DisplayError("Get Tokens", e);
            }
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

        private void DisplayResult(string message)
        {
            CustomLogger.Log(message);
            ResponsePanel.SetResponse(message);
        }

        private void DisplayError(string operation, System.Exception e)
        {
            string message = $"{operation} failed: {e.Message}";
            CustomLogger.Log(message);
            ResponsePanel.SetResponse(message);
        }
    }
}