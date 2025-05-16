using UnityEngine;
using UnityEngine.UI;

namespace OktoSDK.Auth
{
    public class GoogleAuthUI : MonoBehaviour
    {
        public Button loginButton;

        void Start()
        {
            loginButton.onClick.AddListener(() => GoogleAuthManager.Instance.LoginUsingGoogleAuth());
        }
    }
}
