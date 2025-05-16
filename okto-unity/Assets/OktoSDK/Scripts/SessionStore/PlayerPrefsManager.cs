using UnityEngine;
using System;
using static OktoSDK.LoginOAuthDataModels;
using Newtonsoft.Json;

namespace OktoSDK
{
    public static class PlayerPrefsManager
    {
        private const string USER_SESSION_DETAILS = "UserSessionDetails";

        // Save method for authentication result with details (Email, WhatsApp, etc.)
        public static void SaveAuthenticateResult(string environment, SessionConfig sessionConfig, UserDetailBase details)
        {
            CustomLogger.Log($"[SaveAuthenticateResult] Saving authentication result with details. Environment: {environment}");

            var userDetails = new UserDetails
            {
                env = environment,
                details = details,  // Save specific detail
                sessionData = sessionConfig
            };

            string json = JsonConvert.SerializeObject(userDetails, Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto  // Crucial for interface serialization
                });
            
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

                var userDetails = JsonConvert.DeserializeObject<UserDetails>(json,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });

                CustomLogger.Log($"[LoadAuthenticateResult] Loaded user details: {json}");
                return userDetails;
            }

            CustomLogger.LogWarning("[LoadAuthenticateResult] No user session details found in PlayerPrefs.");
            return null;
        }


        public static void Delete()
        {
            if (PlayerPrefs.HasKey(USER_SESSION_DETAILS))
            {
                PlayerPrefs.DeleteKey(USER_SESSION_DETAILS);
            }
        }
    }

    // UserDetails class without generics
    [Serializable]
    public class UserDetails
    {
        public string env;
        public SessionConfig sessionData;

        [SerializeReference]
        public UserDetailBase details;  // Can store any IUserDetail type (Email, WhatsApp, etc.)
    }
}
