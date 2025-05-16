using System;
using System.Collections.Generic;
using UnityEngine;

//configured rpc url for supported chains
//use your own keys ay production
namespace OktoSDK.Features.SmartContract
{
    public class ChainUrls : MonoBehaviour
    {
        [System.Serializable]
        public class NetworkInfo
        {
            [Header("Network Details")]
            [SerializeField] private string name;
            [SerializeField] private string rpcUrl;

            public string Name => name;
            public string RpcUrl => rpcUrl;
        }

        [Header("Network Configurations")]
        [SerializeField] private List<NetworkInfo> networks = new();

        private Dictionary<string, NetworkInfo> networkMapping;

        private void Start()
        {
            InitializeNetworkMapping();
        }

        public string GetChainUrl(string chainName)
        {
            try
            {
                if (string.IsNullOrEmpty(chainName))
                    throw new ArgumentException("Chain name cannot be null or empty");

                NetworkInfo networkInfo = GetNetworkInfo(chainName);
                if (networkInfo == null)
                    throw new Exception($"Network info not found for chain {chainName}");

                string url = networkInfo.RpcUrl;

                if (string.IsNullOrEmpty(url))
                    throw new Exception($"RPC URL for chain {chainName} is not configured in Inspector");

                // Additional URL validation
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    throw new Exception($"Invalid RPC URL format for chain {chainName}: URL must start with http:// or https://");

                return url;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error getting chain URL: {ex.Message}");
                return string.Empty;
            }
        }

        public bool IsChainUrlConfigured(string chainName)
        {
            try
            {
                NetworkInfo networkInfo = GetNetworkInfo(chainName);
                if (networkInfo == null)
                    return false;

                string url = networkInfo.RpcUrl;
                return !string.IsNullOrEmpty(url) &&
                       (url.StartsWith("http://") || url.StartsWith("https://"));
            }
            catch
            {
                return false;
            }
        }

        private void InitializeNetworkMapping()
        {
            networkMapping = new Dictionary<string, NetworkInfo>();
            foreach (var network in networks)
            {
                //CustomLogger.Log(network.RpcUrl);
                if (!string.IsNullOrEmpty(network.Name))
                {
                    networkMapping[network.Name] = network;
                }
            }
        }

        public NetworkInfo GetNetworkInfo(string chainName)
        {
            if (networkMapping.TryGetValue(chainName, out NetworkInfo networkInfo))
                return networkInfo;
            throw new Exception($"Chain {chainName} is not supported");
        }

        public string GetRpcUrl(string chainName)
        {
            return GetNetworkInfo(chainName).RpcUrl;
        }

        public List<string> GetAllNetworkNames()
        {
            return new List<string>(networkMapping.Keys);
        }
    }
}
