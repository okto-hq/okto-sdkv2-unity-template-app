using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which calls GetPortfolio() Api
//This is an independent script which calls GetPortfolioActivity() Api
//This is an independent script which calls GetPortfolioNFT() Api

namespace OktoSDK
{
    public class Portfolio : MonoBehaviour
    {

        public async Task<object> GetPortfolio(OktoClient oc)
        {

            if (oc == null)
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }


            try
            {
                return await BffClientRepository.GetBffClientRepository().GetPortfolio();
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to retrieve portfolio: {error}");
                throw;
            }
        }

        public async Task<object> GetPortfolioActivity(OktoClient oc)
        {
            Loader.ShowLoader();

            if (oc == null)
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            try
            {
                return await BffClientRepository.GetBffClientRepository().GetPortfolioActivity();
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to retrieve portfolio: {error}");
                throw;
            }
        }

        public async Task<object> GetPortfolioNFT(OktoClient oc)
        {
            Loader.ShowLoader();


            if (oc == null)
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            try
            {
                return await BffClientRepository.GetBffClientRepository().GetPortfolioNft();
            }
            catch (Exception error)
            {
                Debug.LogError($"Failed to retrieve orders: {error}");
                throw;
            }
        }
    }
}