using Google;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Net.Http;

//Handles google login
//This code is migrated from v1 version of unity sdk
namespace OktoSDK
{
    public class Login : MonoBehaviour
    {
#if GOOGLE_LOGIN
        private GoogleSignInConfiguration configuration;
#endif
#if GOOGLE_LOGIN
        [SerializeField] private String webClientId;
#endif

        void InitializePlayGamesLogin()
        {
#if GOOGLE_LOGIN

            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestEmail = true,
                RequestIdToken = true,
            };

#else
            Debug.Log("Google Sign-In is not enabled. Please add GOOGLE_LOGIN to scripting define symbols.");

#endif
        }
        private static HttpClient httpClient;

        private void OnEnable()
        {
            InitializePlayGamesLogin();
        }

        public bool loginManually;

        public async void OnLoginButtonClicked()
        {
            Loader.ShowLoader();
            if (loginManually)
            {
                OktoAuthExample.OnLogin();
                return;
            }


            string authenticationData;
            Exception error;
            (authenticationData, error) = await LoginGoogle();

            if (error != null)
            {
                Debug.LogError($"Login failed with error: {error.Message}");
            }
            else
            {
                Debug.Log("Login successful!");
                Debug.Log("loginDone " + authenticationData);

            }

        }

        public async Task<(string result, Exception error)> LoginGoogle()
        {
#if GOOGLE_LOGIN
            GoogleSignIn.Configuration = configuration;

            try
            {
                GoogleSignInUser user = await GoogleSignIn.DefaultInstance.SignIn();
                if (user != null)
                {
                    Debug.Log($"Signed in successfully! Welcome: {user.DisplayName}");
                    Debug.Log($"ID Token: {user.IdToken}");
                    OktoAuthExample.LoginWithGoogle(user.IdToken, "google");

                    // Use the ID token to authenticate with your backend
                    return (user.IdToken, null); // Returns (AuthDetails, Exception)
                }
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    if (innerException is GoogleSignIn.SignInException signInError)
                    {
                        Debug.LogError($"Google Sign-In Error: {signInError.Status} - {signInError.Message}");
                        return (null, signInError);
                    }
                }
            }
            catch (Exception ex)
            {
                Loader.DisableLoader();
                Debug.LogError($"Unexpected Error: {ex.Message}");
                return (null, ex);
            }

            Debug.Log("Sign-In was canceled.");
            return (null, null); // Return null if sign-in is canceled
#else
            Debug.LogWarning("Google Sign-In is not enabled. Please add GOOGLE_LOGIN to scripting define symbols.");
            return (null, null);
#endif
        }

    }


    [System.Serializable]
    public class AuthDetails
    {
        public string authToken { get; set; }
        public string refreshToken { get; set; }
        public string deviceToken { get; set; }
    }
}