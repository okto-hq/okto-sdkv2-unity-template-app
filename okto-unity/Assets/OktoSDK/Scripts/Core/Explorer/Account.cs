using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

// This is an independent script that calls GetWallets API
namespace OktoSDK
{
    public class Account : MonoBehaviour
    {
        public async Task<List<Wallet>> GetWallets()
        {
            try
            {
                var bffWallets = await BffClientRepository.GetWallets();
                return bffWallets;
            }
            catch (Exception error)
            {
                CustomLogger.LogError($"Failed to retrieve wallets: {error}");
                throw new Exception("Unable to fetch wallet information");
            }
        }
    }
}