using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OktoSDK.BFF;
using UnityEngine;
using static OktoSDK.LoginOAuthDataModels;

namespace OktoSDK.Auth
{
    /// <summary>
    /// Manages authentication with the Okto SDK
    /// </summary>
    public class OktoAuthManager : MonoBehaviour
    {
        private static OktoAuthManager _instance;
        public static OktoAuthManager Instance => _instance ??= new GameObject("OktoAuthManager").AddComponent<OktoAuthManager>();

        private OktoClient _oktoClient;
        private OktoClientConfig _config;
        private UserDetailBase _currentUserDetails;
        private string _idToken;

        private TaskCompletionSource<bool> loginTask;
        private TaskCompletionSource<bool> authenticateTask;

        public event Action<string> OnLoginStatusChanged;
        public event Action<string> OnResponseReceived;
        public event Action<string> OnError;
        public event Action<bool> OnAuthenticationComplete;
        public static event Action<bool> OnAuthStateChanged;
        public static event Action<bool> HandleAuthStateChanged;

        public OktoClient Client => _oktoClient;
        public OktoClientConfig Config => _config;
        public static bool isWhatsAppLogin { get; set; }

        public void Initialize(string environment = OktoEnv.SANDBOX, string clientPrivateKey = null, string clientSWA = null)
        {
            clientPrivateKey ??= Environment.GetClientPrivateKey(environment);
            clientSWA ??= Environment.GetClientSWA(environment);

            if (!ValidateConfig(clientPrivateKey, clientSWA)) return;

            _config = new OktoClientConfig
            {
                Environment = environment,
                ClientPrivateKey = clientPrivateKey,
                ClientSWA = clientSWA
            };

            CreateNewClient();
        }

        public bool ValidateConfig(string clientPrivateKey, string clientSWA)
        {
            if (string.IsNullOrEmpty(clientPrivateKey))
            {
                OnError?.Invoke("clientPrivateKey is required");
                return false;
            }

            if (string.IsNullOrEmpty(clientSWA))
            {
                OnError?.Invoke("clientSWA is required");
                return false;
            }

            return true;
        }

        private void CreateNewClient()
        {
            SilentLogout();
            _oktoClient = new OktoClient(_config);
        }

        public static Task<bool> OLogin()
        {
            Instance.loginTask = new TaskCompletionSource<bool>();
            Instance.OnLoginInternal();
            return Instance.loginTask.Task;
        }

        public Task<bool> Login()
        {
            loginTask = new TaskCompletionSource<bool>();
            OnLoginInternal();
            return loginTask.Task;
        }

        private async void OnLoginInternal()
        {
            if (_oktoClient == null)
            {
                OnError?.Invoke("Client not initialized");
                loginTask.TrySetResult(false);
                return;
            }

            var loginTaskCompletionSource = new TaskCompletionSource<bool>();
            AuthenticateInternal(Environment.GetTokenId(), AuthProvider.GOOGLE, loginTaskCompletionSource);

            bool result = await loginTaskCompletionSource.Task;
            loginTask.TrySetResult(result);
        }

        public static Task<bool> Authenticate(string idToken, string provider)
        {
            var authenticateTask = new TaskCompletionSource<bool>();

            // Ensure the instance is initialized
            var instance = Instance;

            instance.AuthenticateInternal(idToken, provider, authenticateTask);

            return authenticateTask.Task;
        }

        private async void AuthenticateInternal(string idToken, string provider, TaskCompletionSource<bool> taskCompletionSource)
        {
            try
            {
                var authData = new AuthData
                {
                    IdToken = idToken,
                    Provider = provider
                };

                AuthenticateResult authSessionData = await _oktoClient.LoginUsingOAuth(
                    authData,
                    (session) => CustomLogger.Log($"Login successful! Session created for user: {session.UserSWA}")
                );

                if (authSessionData.UserSWA != null)
                {
                    HandleSuccessfulAuthentication(authSessionData);
                    OnAuthenticationComplete?.Invoke(true);
                    taskCompletionSource.TrySetResult(true);
                    return;
                }
            }
            catch (Exception e)
            {
                OnAuthenticationComplete?.Invoke(false);
            }

            taskCompletionSource.TrySetResult(false);
        }

        private void HandleSuccessfulAuthentication(AuthenticateResult authSessionData)
        {
            string jsonString = JsonConvert.SerializeObject(authSessionData, Formatting.Indented);
            OnResponseReceived?.Invoke(jsonString);

            BffClientRepository.InitializeApiClient();
            CustomLogger.Log(JsonConvert.SerializeObject(_oktoClient._sessionConfig));

            SaveAuthenticationResult(authSessionData);

            _oktoClient._userDetails = PlayerPrefsManager.LoadAuthenticateResult();
            OnLoginStatusChanged?.Invoke($"Logged In : {_oktoClient._userDetails.sessionData.UserSWA}");

            CustomLogger.Log($"Successfully logged in with user SWA: {authSessionData.UserSWA}");
        }

        public void Logout()
        {
            if (_oktoClient != null)
            {
                _oktoClient.SessionClear();
            }

            OnLoginStatusChanged?.Invoke(string.Empty);
            OnResponseReceived?.Invoke("Logged Out Successfully!");
            CustomLogger.Log("Logged out successfully");
        }

        private void SilentLogout()
        {
            if (_oktoClient != null)
            {
                _oktoClient.SessionClear();
            }
            OnLoginStatusChanged?.Invoke(string.Empty);
            CustomLogger.Log("Logged out successfully");
        }

        public void SetToken(string token)
        {
            _idToken = token;
        }

        private void SaveAuthenticationResult(AuthenticateResult authSessionData)
        {
            CustomLogger.Log("_currentUserDetails " + _currentUserDetails?.GetDetail());
            PlayerPrefsManager.SaveAuthenticateResult(_config.Environment, _oktoClient._sessionConfig, _currentUserDetails);
        }

        public static void SetUserDetails(UserDetailBase details)
        {
            Instance._currentUserDetails = details;
        }

        // Add missing static methods
        public static OktoClient GetOktoClient() => Instance._oktoClient;
        public static OktoClientConfig GetOktoClientConfig() => Instance._config;
        public static SessionConfig GetSession() => Instance._oktoClient?._sessionConfig;
        public static UserDetails GetUserDetails() => Instance._oktoClient?._userDetails;

        public static void HandleSessionExpired()
        {
            CustomLogger.LogError("=========Session Expired, please login Again===========");
            Instance.OnError?.Invoke("Session Expired, please login Again");
        }

        public static void SetConfig() => Instance.SetConfigInternal();

        private void SetConfigInternal()
        {
            if (_config == null)
            {
                OnError?.Invoke("Configuration not initialized");
                return;
            }

            SilentLogout();
            CreateNewClient();
        }

        public static void SetUpAutoLogin(string env, SessionConfig sessionConfig, UserDetails userDetails)
        {
            Instance.SetUpAutoLoginInternal(env, sessionConfig, userDetails);
        }

        private void SetUpAutoLoginInternal(string env, SessionConfig sessionConfig, UserDetails userDetails)
        {
            if (Environment.IsAutoLoginEnabled())
            {
                Initialize(env, Environment.GetClientPrivateKey(env), Environment.GetClientSWA(env));
            }

            _oktoClient = new OktoClient(_config, sessionConfig, userDetails);
            BffClientRepository.InitializeApiClient();

            OnLoginStatusChanged?.Invoke($"Logged In : {userDetails.sessionData.UserSWA}");
        }
    }
}
