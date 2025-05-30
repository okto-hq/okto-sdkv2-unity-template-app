
using OktoSDK.Auth;
using static OktoSDK.LoginOAuthDataModels;

namespace OktoSDK.Utils
{
    /// <summary>
    /// Utility class for fetching stored user email and phone number from OktoAuthManager.
    /// </summary>
    public static class UserDetailsFetcher
    {
        /// <summary>
        /// Retrieves the stored email address of the logged-in user, if available.
        /// </summary>
        /// <returns>
        /// The email address as a string, or null if not available.
        /// </returns>
        public static string GetStoredEmail()
        {
            var userDetails = OktoAuthManager.GetUserDetails();
            if (userDetails?.details is EmailDetail emailDetail)
            {
                CustomLogger.Log($"[UserDetailsFetcher] Email: {emailDetail.email}");
                return emailDetail.email;
            }
            if (userDetails == null)
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetails is null.");
            }
            else if (userDetails.details == null)
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetail (details) is null.");
            }
            else
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetail is not of type EmailDetail.");
            }
            return null;
        }

        /// <summary>
        /// Retrieves the stored phone number of the logged-in user, if available.
        /// </summary>
        /// <returns>
        /// The phone number as a string, or null if not available.
        /// </returns>
        public static string GetStoredPhoneNumber()
        {
            var userDetails = OktoAuthManager.GetUserDetails();
            if (userDetails?.details is WhatsAppDetail phoneDetail)
            {
                CustomLogger.Log($"[UserDetailsFetcher] Phone Number: {phoneDetail.PhoneNumber}");
                return phoneDetail.PhoneNumber;
            }
            if (userDetails == null)
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetails is null.");
            }
            else if (userDetails.details == null)
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetail (details) is null.");
            }
            else
            {
                CustomLogger.LogWarning("[UserDetailsFetcher] UserDetail is not of type WhatsAppDetail.");
            }
            return null;
        }
    }
} 