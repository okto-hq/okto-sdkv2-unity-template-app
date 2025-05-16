using System.Threading.Tasks;

namespace OktoSDK.Auth
{
    /// <summary>
    /// Interface for authentication providers
    /// </summary>
    public interface IAuthenticate
    {
        /// <summary>
        /// Login with the provided data
        /// </summary>
        /// <param name="data">Authentication data like JWT token, email, etc.</param>
        /// <returns>True if login is successful, false otherwise</returns>
        Task<bool> Login(string data);
    }
} 