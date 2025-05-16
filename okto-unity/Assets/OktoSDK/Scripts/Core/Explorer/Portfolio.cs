using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

//This is an independent script which calls GetPortfolio() Api
//This is an independent script which calls GetPortfolioActivity() Api
//This is an independent script which calls GetPortfolioNFT() Api

namespace OktoSDK
{
    public class Portfolio : MonoBehaviour
    {

        public async Task<UserPortfolioData> GetPortfolio(OktoClient oc)
        {
            try
            {
                return await BffClientRepository.GetPortfolio();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve portfolio: {error}");
                throw;
            }
        }

        public async Task<List<UserPortfolioActivity>> GetPortfolioActivity(OktoClient oc)
        {
            Loader.ShowLoader();

            try
            {
                return await BffClientRepository.GetPortfolioActivity();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve portfolio: {error}");
                throw;
            }
        }

        public async Task<List<UserNFTBalance>> GetPortfolioNFT(OktoClient oc)
        {
            Loader.ShowLoader();

            try
            {
                return await BffClientRepository.GetPortfolioNft();
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve orders: {error}");
                throw;
            }
        }
    }
}