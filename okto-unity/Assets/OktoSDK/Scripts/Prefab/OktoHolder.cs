using UnityEngine;
using UnityEngine.SceneManagement;

namespace OktoSDK
{
    public class OktoHolder : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void TestChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}