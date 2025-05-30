using UnityEngine;

namespace OktoSDK.Auth
{
    public class Authenticate : MonoBehaviour
    {
        public enum EnvironmentType
        {
            Sandbox,
            Staging,
            Production
        }

        [Header("Select the environment from Inspector")]
        public EnvironmentType selectedEnvironment = EnvironmentType.Sandbox;

        private string clientPrivateKey;
        private string clientSWA;
        string selectedEnv;

        void Start()
        {
            OnServerInitialized();
        }

        private void OnServerInitialized()
        {
            UserDetails userDetails = AutoLogin.GetEnvironment();

            if (Environment.IsAutoLoginEnabled())
            {
                if (userDetails != null)
                {
                    Debug.Log("found UserDetails");
                    selectedEnv = userDetails.env;
                    clientPrivateKey = Environment.GetClientPrivateKey(selectedEnv);
                    clientSWA = Environment.GetClientSWA(selectedEnv);
                    AutoLogin.SetUpAutoLogin();
                }
                else
                {
                    Debug.Log("UserDetails not found");

                    selectedEnv = selectedEnvironment.ToString().ToLower(); // Converts enum to string (e.g., "sandbox")
                    clientPrivateKey = Environment.GetClientPrivateKey(selectedEnv);
                    clientSWA = Environment.GetClientSWA(selectedEnv);
                    UpdateConfiguration();
                }
            }
            else
            {
                Debug.Log("Auto login not enabled");

                selectedEnv = selectedEnvironment.ToString().ToLower(); // Converts enum to string (e.g., "sandbox")
                clientPrivateKey = Environment.GetClientPrivateKey(selectedEnv);
                clientSWA = Environment.GetClientSWA(selectedEnv);
                UpdateConfiguration();
            }

            Debug.Log($"Selected Environment Enum: {selectedEnvironment}");
        }

        private void UpdateConfiguration()
        {
            OktoAuthManager.Instance.Initialize(
                selectedEnv,
                clientPrivateKey,
                clientSWA
            );
        }
    }
}









