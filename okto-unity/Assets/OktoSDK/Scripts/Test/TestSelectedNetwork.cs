using UnityEngine;

namespace OktoSDK
{
    public class TestSelectedNetwork : MonoBehaviour
    {
        [SerializeField]
        private NetworkPrefab networkPrefab;

        [SerializeField]
        private string networkName;

        public void CallSelectedNetwork()
        {
            networkPrefab.GetSelectedNetwork(networkName);
        }
    }
}