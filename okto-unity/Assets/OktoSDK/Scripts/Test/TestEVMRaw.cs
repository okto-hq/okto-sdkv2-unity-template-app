using UnityEngine;

namespace OktoSDK
{
    public class TestEVMRaw : MonoBehaviour
    {
        [SerializeField]
        private EvmRawPrefab evmRawPrefab;

        [SerializeField]
        private NetworkPrefab networkPrefab;

        [SerializeField]
        private string networkName;

        [SerializeField]
        private string from;

        [SerializeField]
        private string to;

        [SerializeField]
        private string data;

        [SerializeField]
        private string value;

        [SerializeField]
        private string feePayer;

        public async void CallEvmRawTransfer()
        {
            string networkId = await networkPrefab.GetSelectedNetwork(networkName);
            if (!string.IsNullOrEmpty(networkId))
            {
                CustomLogger.Log(networkName + " exists");
                evmRawPrefab.SetFeePayer(feePayer);
                evmRawPrefab.CallEvmRawTransfer(from, to, value);
            }
            else
            {
                CustomLogger.Log(networkName + " doesn't exist in active network");
            }
        }

        public async void CallEvmRawSmartContract()
        {
            string networkId = await networkPrefab.GetSelectedNetwork(networkName);

            if (!string.IsNullOrEmpty(networkId))
            {
                CustomLogger.Log(networkName + " exists");
                evmRawPrefab.SetFeePayer(feePayer);
                evmRawPrefab.CallEvmRawTransfer(from, to, data);
            }
            else
            {
                CustomLogger.Log(networkName + " doesn't exist in active network");
            }
        }
    }
}