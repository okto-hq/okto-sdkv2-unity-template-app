using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which called GetToken() Api
namespace OktoSDK
{
    public class GetToken : MonoBehaviour
    {
        public async Task<object> GetTokens(OktoClient oc)
        {
            if (oc == null)
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return "You are not logged In!";
            }

            if (!oc.IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                //ResponsePanel.DisplayResponse();
                return "You are not logged In!";
            }

            try
            {
                var response = await BffClientRepository.GetBffClientRepository().GetSupportedTokens();
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