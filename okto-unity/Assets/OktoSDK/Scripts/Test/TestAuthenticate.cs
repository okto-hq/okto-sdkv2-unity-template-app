using OktoSDK.Auth;
using UnityEngine;

namespace OktoSDK
{
    public class TestAuthenticate : MonoBehaviour
    {
        private OktoAuthManager _authManager;

        private void Start()
        {
            // Initialize the OktoAuthManager instance
            _authManager = OktoAuthManager.Instance;

            // Check if auto-login is enabled and set it up if so
            if (Environment.IsAutoLoginEnabled())
            {
                AutoLogin.SetUpAutoLogin();
            }
            else
            {
                _authManager.Initialize(
                    OktoEnv.SANDBOX,
                    Environment.GetClientPrivateKey(OktoEnv.SANDBOX),
                    Environment.GetClientSWA(OktoEnv.SANDBOX)
                );
                CustomLogger.Log("Auto-login is not enabled. Please log in manually.");
            }
        }

        public async void OnClick()
        {
            // Attempt to log in and handle the result
            bool success = await _authManager.Login();

            if (success)
            {
                CustomLogger.Log("Login successful");
            }
            else
            {
                CustomLogger.LogError("Login failed");
            }
        }
    }
}