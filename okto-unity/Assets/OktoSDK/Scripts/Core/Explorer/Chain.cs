using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

//This is an independent script which called GetChain() Api
namespace OktoSDK
{
    public class Chain : MonoBehaviour
    {
        public async Task<List<NetworkData>> GetChains()
        {
            try
            {
                var supportedNetworks = await BffClientRepository.GetSupportedNetworks();
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