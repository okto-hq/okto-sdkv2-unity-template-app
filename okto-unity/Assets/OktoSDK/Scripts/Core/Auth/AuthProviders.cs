namespace OktoSDK.Auth
{
    /// <summary>
    /// Authentication provider types supported by the Okto SDK
    /// </summary>
    public static class AuthProvider
    {
        /// <summary>
        /// Google authentication provider
        /// </summary>
        public const string GOOGLE = "google";

        /// <summary>
        /// JWT authentication provider
        /// </summary>
        public const string JWT = "client_jwt";

        /// <summary>
        /// Okto native authentication provider
        /// </summary>
        public const string OKTO = "okto";
    }
} 