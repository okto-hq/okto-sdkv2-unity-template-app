using System;
using System.Threading.Tasks;
using UnityEngine;

//This is an independent script which called GetWallets() Api
namespace OktoSDK
{
    public class Account : MonoBehaviour
    {
        public async Task<object> GetAccount(OktoClient oc)
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
                return await BffClientRepository.GetBffClientRepository().GetWallets();

            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve wallets: {error}");
                throw;
            }
        }
    }
}