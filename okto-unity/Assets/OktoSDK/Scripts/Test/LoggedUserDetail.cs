using OktoSDK.Auth;
using UnityEngine;

namespace OktoSDK
{
    public class LoggedUserDetail : MonoBehaviour
    {
        [SerializeField]
        private UserDetails loggedInUserDetail;

        private void OnEnable()
        {
            OktoAuthManager.Instance.OnLoginStatusChanged += OnLoggedIn;
        }

        private void OnDisable()
        {
            OktoAuthManager.Instance.OnLoginStatusChanged -= OnLoggedIn;
        }

        void OnLoggedIn(string status)
        {
            loggedInUserDetail = OktoAuthManager.GetUserDetails();
        }

    }
}