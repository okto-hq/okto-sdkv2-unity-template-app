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
            if (_instance == null)
                return;
            CustomLogger.Log("Showing loader");
            _instance.loader.SetActive(true);
        }

        public static void DisableLoader()
        {
            if (_instance == null)
                return;
            _instance.loader.SetActive(false);
        }
    }
}