using UnityEngine;
using UnityEngine.SceneManagement;

namespace OktoSDK
{
    public class SceneChange : MonoBehaviour
    {
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}