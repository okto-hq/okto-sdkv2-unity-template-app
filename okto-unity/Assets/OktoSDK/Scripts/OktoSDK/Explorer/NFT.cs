using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which calls GetNftPotfolio() Api
namespace OktoSDK
{
    public class NFT : MonoBehaviour
    {

        public async Task<object> GetNftCollections(OktoClient oc)
        {

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                //ResponsePanel.DisplayResponse();
                return "You are not logged In!";
            }

            try
            {
                var response = await BffClientRepository.GetBffClientRepository().GetPortfolioNft();
                return response;
            }
            catch (Exception error)
            {
                Debug.LogError($"Error fetching NFT collections: {error}");
                throw new Exception("Failed to fetch NFT collections from the backend.");
            }
        }
    }

}