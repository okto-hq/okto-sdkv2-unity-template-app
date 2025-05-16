using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

//This is an independent script which called GetToken() Api
namespace OktoSDK
{
    public class GetToken : MonoBehaviour
    {
        public async Task<List<Token>> GetTokens(OktoClient oc)
        {
            try
            {
                var response = await BffClientRepository.GetSupportedTokens();
                return response;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Error fetching supported tokens: {error}");
                throw new Exception("Failed to fetch supported tokens from the backend.");
            }
        }
    }
}