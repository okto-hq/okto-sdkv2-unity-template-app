using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace OktoSDK.Auth
{
    public class OktoAuthView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Dropdown environment;
        [SerializeField] private TMP_InputField clientPrivateKey;
        [SerializeField] private TMP_InputField clientSWA;
        [SerializeField] private TextMeshProUGUI loggedIn;
        [SerializeField] private Button logOutBtn;

        private OktoAuthManager _authManager;

        private void Start()
        {
            _authManager = OktoAuthManager.Instance;
            SetupEventListeners();
            OnServerInitialized();
        }

        private void OnEnable()
        {
            if (environment != null)
            {
                environment.onValueChanged.AddListener(OnChangeEnvironment);
            }

            logOutBtn.onClick.AddListener(Logout);
        }

        private void OnDisable()
        {
            if (environment != null)
            {
                environment.onValueChanged.RemoveListener(OnChangeEnvironment);
            }

            logOutBtn.onClick.RemoveListener(Logout);

            RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            _authManager.OnLoginStatusChanged += UpdateLoginStatus;
            _authManager.OnResponseReceived += HandleResponse;
            _authManager.OnError += HandleError;
            _authManager.OnAuthenticationComplete += HandleAuthenticationComplete;
        }

        private void RemoveEventListeners()
        {
            _authManager.OnLoginStatusChanged -= UpdateLoginStatus;
            _authManager.OnResponseReceived -= HandleResponse;
            _authManager.OnError -= HandleError;
            _authManager.OnAuthenticationComplete -= HandleAuthenticationComplete;
        }

        private void HandleResponse(string message)
        {
            ResponsePanel.SetResponse(message);
        }

        private void HandleError(string error)
        {
            ResponsePanel.SetResponse(error);
        }

        private void HandleAuthenticationComplete(bool success)
        {
            if (success)
            {
                CustomLogger.Log("Authentication succeeded. Saving credentials to PlayerPrefs...");

                PlayerPrefs.SetString("ClientPrivateKey", clientPrivateKey.text);
                PlayerPrefs.SetString("ClientSWA", clientSWA.text);

            }
            else
            {
                CustomLogger.LogWarning("Authentication failed. Skipping PlayerPrefs save.");
            }

            PlayerPrefs.Save();
            CustomLogger.Log("PlayerPrefs saved.");
            Loader.DisableLoader();
        }

        private void UpdateLoginStatus(string status)
        {
            if (loggedIn != null)
            {
                loggedIn.text = status;
            }
        }

        private void OnChangeEnvironment(int value)
        {
            string selectedEnv = environment.options[value].text;
            clientPrivateKey.text = Environment.GetClientPrivateKey(selectedEnv);
            clientSWA.text = Environment.GetClientSWA(selectedEnv);

            UpdateConfiguration();
        }

        public async void OnCheckValidation()
        {
            if (!ValidateInputs()) return;

            UpdateConfiguration();
            Loader.ShowLoader();
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

        public void SaveConfig()
        {
            if (!ValidateInputs()) return;

            UpdateConfiguration();
            HandleResponse("Configuration Updated Successfully!");
        }

        public void Logout()
        {
            _authManager.Logout();
        }

        private void OnServerInitialized()
        {
            UserDetails userDetails = AutoLogin.GetEnvironment();
            string selectedEnv;
            if (userDetails != null)
            {
                selectedEnv = userDetails.env;
                int index = environment.options.FindIndex(option => option.text == selectedEnv);

                if (index >= 0)
                {
                    environment.value = index;
                    environment.RefreshShownValue(); // Refresh the label
                }

                CustomLogger.Log($"selectedEnv: {selectedEnv}");

                if (PlayerPrefs.HasKey("ClientPrivateKey"))
                {
                    clientPrivateKey.text = PlayerPrefs.GetString("ClientPrivateKey");
                }
                else
                {
                    clientPrivateKey.text = Environment.GetClientPrivateKey(selectedEnv);
                }

                if (PlayerPrefs.HasKey("ClientSWA"))
                {
                    clientSWA.text = PlayerPrefs.GetString("ClientSWA");
                }
                else
                {
                    clientSWA.text = Environment.GetClientSWA(selectedEnv);
                }

                if (Environment.IsAutoLoginEnabled())
                {
                    AutoLogin.SetUpAutoLogin();
                }

            }
            else
            {
                selectedEnv = environment.options[environment.value].text;
                clientPrivateKey.text = Environment.GetClientPrivateKey(selectedEnv);
                clientSWA.text = Environment.GetClientSWA(selectedEnv);
                UpdateConfiguration();
            }
        }

        private bool ValidateInputs()
        {
            return _authManager.ValidateConfig(clientPrivateKey.text, clientSWA.text);
        }

        private void UpdateConfiguration()
        {
            _authManager.Initialize(
                environment.options[environment.value].text,
                clientPrivateKey.text,
                clientSWA.text
            );
        }
    }
}
