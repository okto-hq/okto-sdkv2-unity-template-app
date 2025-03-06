using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System;

//It creates new object of OKtoClient based on client swa/client private key fed to them
//This script is an authentication entry point
namespace OktoSDK
{
    public class OktoAuthExample : MonoBehaviour
    {
        private OktoClient _oktoClient;
        private OktoClientConfig config;

        public string idToken;

        [SerializeField]
        private TMP_Dropdown environment;

        [SerializeField]
        private TMP_InputField clientPrivateKey;

        [SerializeField]
        private TMP_InputField clientSWA;

        private static OktoAuthExample _instance;

        public Login googleLogin;

        void OnEnable()
        {
            _instance = this;
        }

        public void SaveConfig()
        {
            // Initialize the Okto client
            if (string.IsNullOrEmpty(clientPrivateKey.text))
            {
                ResponsePanel.SetResponse("clientPrivateKey is required");
                return;
            }

            if (string.IsNullOrEmpty(clientSWA.text))
            {
                ResponsePanel.SetResponse("clientSWA is required");
                return;
            }

            config = new OktoClientConfig
            {
                Environment = environment.options[environment.value].text,
                ClientPrivateKey = clientPrivateKey.text,
                ClientSWA = clientSWA.text
            };

            _oktoClient = new OktoClient(config);


            Debug.Log("SaveConfig _oktoClient " + JsonConvert.SerializeObject(_oktoClient));
            Debug.Log("config " + JsonConvert.SerializeObject(config));

            //log out if already logged In
            SilentLogout();
            ResponsePanel.SetResponse("Configuration Updated Sucessfully!");
        }


        public void OnCheckValidation()
        {
            if (string.IsNullOrEmpty(clientPrivateKey.text))
            {
                ResponsePanel.SetResponse("clientPrivateKey is required");
                return;
            }

            if (string.IsNullOrEmpty(clientSWA.text))
            {
                ResponsePanel.SetResponse("clientSWA is required");
                return;
            }

            config = new OktoClientConfig
            {
                Environment = environment.options[environment.value].text,
                ClientPrivateKey = clientPrivateKey.text,
                ClientSWA = clientSWA.text
            };

            _oktoClient = new OktoClient(config);
            
            googleLogin.OnLoginButtonClicked();
        }

        public static void OnLogin()
        {
            LoginWithGoogle(_instance.idToken, "google");
        }

        public static OktoClient getOktoClient()
        {
            return _instance._oktoClient;
        }

        public static OktoClientConfig getOktoClientConfig()
        {
            return _instance.config;
        }

        public static SessionConfig GetSession()
        {
            return _instance._oktoClient._sessionConfig;
        }

        public static async void LoginWithGoogle(string idToken, string provider)
        {
            try
            {
                var authData = new AuthData
                {
                    IdToken = idToken,
                    Provider = provider
                };

                AuthenticateResult authSessionData = await _instance._oktoClient.LoginUsingOAuth(
                authData,
                (session) =>
                {
                    Debug.Log($"Login successful! Session created for user: {session.UserSWA}");
                }

                );

                if (authSessionData.UserSWA != null)
                {
                    string jsonString = JsonConvert.SerializeObject(authSessionData, Formatting.Indented);
                    ResponsePanel.SetResponse(jsonString);
                    BffClientRepository.InitializeApiClient();
                    Debug.Log($"Successfully logged in with user SWA: {authSessionData.UserSWA}");
                }
            }
            catch (System.Exception e)
            {
                Loader.DisableLoader();
                Debug.Log($"Login failed: {e.Message}");
            }
        }

        public void Logout()
        {
            if (_oktoClient != null)
            {
                _oktoClient.SessionClear();
            }

            ResponsePanel.SetResponse("Logged Out Successfully!");
            Debug.Log("Logged out successfully");
        }

        public void SilentLogout()
        {
            if( _oktoClient != null)
            {
                _oktoClient.SessionClear();
            }

            Debug.Log("Logged out successfully");
        }
    }
}