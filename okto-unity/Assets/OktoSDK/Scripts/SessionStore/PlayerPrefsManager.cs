using UnityEngine;
using OktoSDK;
using System;

namespace OktoSDK
{
    public static class PlayerPrefsManager
    {
        private const string USER_SESSION_DETAILS = "UserSessionDetails";

        // Save method for authentication result without WhatsApp details
        public static void SaveAuthenticateResult(string environment, SessionConfig sessionConfig)
        {
            CustomLogger.Log($"[SaveAuthenticateResult] Saving authentication result without WhatsApp details. Environment: {environment}");

            UserDetails userDetails = new UserDetails
            {
                env = environment,
                whatsapp = new WhatsApp { number = "" },  // Empty if not provided
                sessionData = sessionConfig
            };

            string json = JsonUtility.ToJson(userDetails);
            PlayerPrefs.SetString(USER_SESSION_DETAILS, json);
            PlayerPrefs.Save();

            CustomLogger.Log($"[SaveAuthenticateResult] Data saved successfully: {json}");
        }

        // Save method for authentication result with WhatsApp details
        public static void SaveAuthenticateResult(string environment, SessionConfig sessionConfig, WhatsApp whatsAppDetails = null)
        {
            CustomLogger.Log($"[SaveAuthenticateResult] Saving authentication result with WhatsApp details. Environment: {environment}");

            UserDetails userDetails = new UserDetails
            {
                env = environment,
                whatsapp = whatsAppDetails,
                sessionData = sessionConfig
            };

            string json = JsonUtility.ToJson(userDetails);
            PlayerPrefs.SetString(USER_SESSION_DETAILS, json);
            PlayerPrefs.Save();

            CustomLogger.Log($"[SaveAuthenticateResult] Data saved successfully: {json}");
        }

        // Load method for retrieving the saved user details
        public static UserDetails LoadAuthenticateResult()
        {
            if (PlayerPrefs.HasKey(USER_SESSION_DETAILS))
            {
                string json = PlayerPrefs.GetString(USER_SESSION_DETAILS);
                CustomLogger.Log($"[LoadAuthenticateResult] Loaded user details: {json}");
                return JsonUtility.FromJson<UserDetails>(json);
            }

            CustomLogger.LogWarning("[LoadAuthenticateResult] No user session details found in PlayerPrefs.");
            return null; // Return null if no data exists
        }

        public static void Delete()
        {
            if (PlayerPrefs.HasKey(USER_SESSION_DETAILS))
            {
                PlayerPrefs.DeleteKey(USER_SESSION_DETAILS);
            }

        }
    }


    [Serializable]
    public class UserDetails
    {
        public string env;
        public WhatsApp whatsapp;
        public SessionConfig sessionData;
    }

}