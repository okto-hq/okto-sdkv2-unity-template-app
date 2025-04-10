using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which called GetChain() Api
namespace OktoSDK
{
    public class Chain : MonoBehaviour
    {

        public async Task<object> GetChains(OktoClient oc)
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
                var supportedNetworks = await BffClientRepository.GetBffClientRepository().GetSupportedNetworks();
                return supportedNetworks;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve supported networks: {error}");
                throw new Exception("Unable to fetch supported networks");
            }
        }
    }
}