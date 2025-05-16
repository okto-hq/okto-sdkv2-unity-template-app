using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.BFF;

namespace OktoSDK
{
    public class NetworkPrefab : MonoBehaviour
    {
        public async Task<string?> GetSelectedNetwork(string desiredChain)
        {
            List<NetworkData> tokenList = await BffClientRepository.GetSupportedNetworks();

            for (int i = 0;i < tokenList.Count;i++) {
                if (desiredChain.ToLower().Equals(tokenList[i].networkName.ToLower()))
                {
                    TransactionConstants.CurrentChain = tokenList[i];
                    string json = JsonConvert.SerializeObject(TransactionConstants.CurrentChain, Formatting.Indented);
                    CustomLogger.Log("TransactionConstants.CurrentChain " + json);
                    return TransactionConstants.CurrentChain.caipId;
                }
            }
            return null;
        }

        // Fetches all supported networks
        public async Task<List<NetworkData>> GetNetworksApi()
        {
            var bffNetworks = await BffClientRepository.GetSupportedNetworks();
            
            // Convert BFF networks to model networks
            List<NetworkData> modelNetworks = new List<NetworkData>();
            foreach (var network in bffNetworks)
            {
                modelNetworks.Add(new NetworkData
                {
                    chainId = network.chainId,
                    networkId = network.networkId,
                    networkName = network.networkName,
                    gsnEnabled = network.gsnEnabled,
                    sponsorshipEnabled = network.sponsorshipEnabled
                });
            }
            
            string json = JsonConvert.SerializeObject(modelNetworks, Formatting.Indented);
            CustomLogger.Log("Networks: " + json);
            return modelNetworks;
        }
    }
}