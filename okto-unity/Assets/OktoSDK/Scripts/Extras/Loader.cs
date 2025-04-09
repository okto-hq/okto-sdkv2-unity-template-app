using UnityEngine;


namespace OktoSDK
{
    //It handles loader mechanism
    public class Loader : MonoBehaviour
    {
        [SerializeField]
        private GameObject loader;

        public static Loader _instance;

        private void OnEnable()
        {
            _instance = this;
        }

        public static void ShowLoader()
        {
            CustomLogger.Log("Showing loader");
            _instance.loader.SetActive(true);
        }

        public static void DisableLoader()
        {
            _instance.loader.SetActive(false);
        }
    }
}