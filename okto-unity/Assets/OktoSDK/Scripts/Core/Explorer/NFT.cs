using Newtonsoft.Json;
using OktoSDK.BFF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which calls GetNftPotfolio() Api
namespace OktoSDK
{
    public class NFT : MonoBehaviour
    {

        public async Task<List<UserNFTBalance>> GetNftCollections(OktoClient oc)
        {
            try
            {
                var response = await BffClientRepository.GetPortfolioNft();
                return response;
            }
            catch (Exception error)
            {
                CustomLogger.Log(error.Message);
                throw new Exception(error.Message);
            }
        }
    }

}