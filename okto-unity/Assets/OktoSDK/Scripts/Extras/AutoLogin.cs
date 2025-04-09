using Newtonsoft.Json;

//script to setup autologin if enabled in environments
namespace OktoSDK
{
    public static class AutoLogin
    {
        public static void SetUpAutoLogin()
        {
            UserDetails userDetails = PlayerPrefsManager.LoadAuthenticateResult();

            if (userDetails != null)
            {
                CustomLogger.Log("AutoLogin:user session found. " + JsonConvert.SerializeObject(userDetails));
                OktoAuthExample.SetUpAutoLogin(userDetails.env, userDetails.sessionData, userDetails);
            }
            else
            {
                CustomLogger.Log("AutoLogin: No saved session found.");
            }
        }
    }
}