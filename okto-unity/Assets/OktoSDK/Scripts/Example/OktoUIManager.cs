using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;


    //This script has expose methods for
    // -getAccount()
    // -getOrdersHistory()
    // -getPortfolioActivity()
    // -getTokens()
    // -getChains()
    // -getPortfolio()
    // -getNFTPortfolio()

    namespace OktoSDK
    {
        public class OktoUIManager : MonoBehaviour
        {
            [Header("Components")]
            [SerializeField] private Account account;
            [SerializeField] private Chain chain;
            [SerializeField] private NFT nft;
            [SerializeField] private GetOrder order;
            [SerializeField] private Portfolio portfolio;
            [SerializeField] private GetToken token;

            [Header("Response Text")]
            [SerializeField] private GameObject displayObj;
            [SerializeField] private TextMeshProUGUI resultText;

            [System.Serializable]
            public class WalletListWrapper
            {
                public List<Wallet> wallets;
            }

            // Button click handlers
            public async void OnGetAccountButtonClick()
            {
                try
                {
                    Loader.ShowLoader();

                    var wallets = await account.GetAccount(OktoAuthExample.getOktoClient());
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
                try
                {
                    Loader.ShowLoader();

                    var networks = await chain.GetChains(OktoAuthExample.getOktoClient());
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
                try
                {
                    Loader.ShowLoader();

                    var collections = await nft.GetNftCollections(OktoAuthExample.getOktoClient());
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
                try
                {
                    Loader.ShowLoader();

                    var orders = await order.GetOrdersHistory(OktoAuthExample.getOktoClient());
                    string jsonString = JsonConvert.SerializeObject(orders, Formatting.Indented);
                    ResponsePanel.SetOrderResponse(jsonString);
                }
                catch (System.Exception e)
                {
                    DisplayError("Get Orders History", e);
                }
            }

            public async void OnGetPortfolioButtonClick()
            {
                try
                {
                    Loader.ShowLoader();

                    var portfolioData = await portfolio.GetPortfolio(OktoAuthExample.getOktoClient());
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
                try
                {
                    Loader.ShowLoader();

                    var portfolioData = await portfolio.GetPortfolioActivity(OktoAuthExample.getOktoClient());
                    string jsonString = JsonConvert.SerializeObject(portfolioData, Formatting.Indented);
                    DisplayResult(jsonString);
                }
                catch (System.Exception e)
                {
                    DisplayError("Get Portfolio", e);
                }
            }

            public async void OnGetTokensButtonClick()
            {
                try
                {
                    Loader.ShowLoader();

                    var tokens = await token.GetTokens(OktoAuthExample.getOktoClient());
                    string jsonString = JsonConvert.SerializeObject(tokens, Formatting.Indented);
                    DisplayResult(jsonString);
                }
                catch (System.Exception e)
                {
                    DisplayError("Get Tokens", e);
                }
            }

            private void DisplayResult(string message)
            {
                Debug.Log(message);
                if (resultText != null)
                {
                    ResponsePanel.SetResponse(message);
                }
            }

            private void DisplayError(string operation, System.Exception e)
            {
                string message = $"{operation} failed: {e.Message}";
                Debug.Log(message);
                if (resultText != null)
                {
                    ResponsePanel.SetResponse(message);
                }
            }
        }

    }
