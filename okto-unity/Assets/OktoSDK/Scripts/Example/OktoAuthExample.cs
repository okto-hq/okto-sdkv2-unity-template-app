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

        private string idToken;

        [SerializeField]
        private TMP_Dropdown environment;

        [SerializeField]
        private TMP_InputField clientPrivateKey;

        [SerializeField]
        private TMP_InputField clientSWA;

        private static OktoAuthExample _instance;

        public Login googleLogin;

        public static bool isWhatsAppLogin;

        private WhatsApp currentWhatsAppDetails;

        [SerializeField]
        private TextMeshProUGUI loggedIn;

        void Start()
        {
            _instance = this;
            OnServerInitialized();
        }

        private void OnEnable()
        {
            environment.onValueChanged.AddListener(OnChangeEnviornment);
        }

        private void OnDisable()
        {
            environment.onValueChanged.RemoveListener(OnChangeEnviornment);
        }

        public static void HandleSessionExpired()
        {
            //Handle Session Expire Here
            CustomLogger.LogError("=========Session Expired,please login Again===========");
        }

        private void OnChangeEnviornment(int value)
        {
            if (Environment.GetTestMode())
            {
                string selectedEnv = environment.options[value].text;
                clientPrivateKey.text = Environment.GetClientPrivateKey(selectedEnv);
                clientSWA.text = Environment.GetClientSWA(selectedEnv);

                config = new OktoClientConfig
                {
                    Environment = environment.options[environment.value].text,
                    ClientPrivateKey = clientPrivateKey.text,
                    ClientSWA = clientSWA.text
                };
            }
        }

        private void OnServerInitialized()
        {
            if (Environment.GetTestMode())
            {
                string selectedEnv = environment.options[environment.value].text;
                clientPrivateKey.text = Environment.GetClientPrivateKey(selectedEnv);
                clientSWA.text = Environment.GetClientSWA(selectedEnv);
            }

            if (Environment.IsAutoLoginEnabled())
            {
                AutoLogin.SetUpAutoLogin();
            }
        }

        public static void SetWhatsAppDeatils(string phoneNumber)
        {
            _instance.currentWhatsAppDetails = new WhatsApp
            {
                number = phoneNumber
            };
        }

        public static void SetToken(string token)
        {
            _instance.idToken = token;
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

            SetConfig();
            
            googleLogin.OnLoginButtonClicked();

        }

        public static void SetConfig() {

            _instance.SilentLogout();
            _instance.config = new OktoClientConfig
            {
                Environment = _instance.environment.options[_instance.environment.value].text,
                ClientPrivateKey = _instance.clientPrivateKey.text,
                ClientSWA = _instance.clientSWA.text
            };

            _instance._oktoClient = new OktoClient(_instance.config);

        }


        public static void SetUpAutoLogin(string env,SessionConfig sessionConfig,UserDetails userDetails)
        {
            if (string.IsNullOrEmpty(_instance.clientPrivateKey.text))
            {
                ResponsePanel.SetResponse("clientPrivateKey is required");
                return;
            }

            if (string.IsNullOrEmpty(_instance.clientSWA.text))
            {
                ResponsePanel.SetResponse("clientSWA is required");
                return;
            }

            _instance.config = new OktoClientConfig
            {
                Environment = env,
                ClientPrivateKey = _instance.clientPrivateKey.text,
                ClientSWA = _instance.clientSWA.text
            };

            _instance.loggedIn.text = "Logged In : " + userDetails.sessionData.UserSWA;

            _instance._oktoClient = new OktoClient(_instance.config,sessionConfig,userDetails);
            BffClientRepository.InitializeApiClient();

        }

        public static Login GetGoogleInstance()
        {
            return _instance.googleLogin;
        }

        public static void OnLogin()
        {
            Authenticate(Environment.GetTokenId(), AuthProvider.GOOGLE);
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

        public static UserDetails GetUserDetails ()
        {
            return _instance._oktoClient._userDetails;
        }

        public static async void Authenticate(string idToken, string provider)
        {
            Loader.ShowLoader();

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
                    CustomLogger.Log($"Login successful! Session created for user: {session.UserSWA}");
                }
                );

                if (authSessionData.UserSWA != null)
                {
                    string jsonString = JsonConvert.SerializeObject(authSessionData, Formatting.Indented);
                    ResponsePanel.SetResponse(jsonString);
                    BffClientRepository.InitializeApiClient();

                    CustomLogger.Log(JsonConvert.SerializeObject(GetSession()));

                    if (_instance.currentWhatsAppDetails != null)
                    {
                        PlayerPrefsManager.SaveAuthenticateResult(getOktoClientConfig().Environment, GetSession(), _instance.currentWhatsAppDetails);

                    }
                    else
                    {
                        PlayerPrefsManager.SaveAuthenticateResult(getOktoClientConfig().Environment, GetSession(), null);

                    }

                    _instance._oktoClient._userDetails = PlayerPrefsManager.LoadAuthenticateResult();
                    _instance.loggedIn.text = "Logged In : " + _instance._oktoClient._userDetails.sessionData.UserSWA;

                    CustomLogger.Log($"Successfully logged in with user SWA: {authSessionData.UserSWA}");
                }
            }
            catch (System.Exception e)
            {
                Loader.DisableLoader();
                CustomLogger.Log($"Login failed: {e.Message}");
            }
        }

        public void Logout()
        {
            if (_oktoClient != null)
            {
                _oktoClient.SessionClear();
            }

            _instance.loggedIn.text = string.Empty;
            ResponsePanel.SetResponse("Logged Out Successfully!");
            CustomLogger.Log("Logged out successfully");
        }

        public void SilentLogout()
        {
            if( _oktoClient != null)
            {
                _oktoClient.SessionClear();
            }

            _instance.loggedIn.text = string.Empty;
            CustomLogger.Log("Logged out successfully");
        }
    }
}

[Serializable]
public class WhatsApp
{
    public string number;
}