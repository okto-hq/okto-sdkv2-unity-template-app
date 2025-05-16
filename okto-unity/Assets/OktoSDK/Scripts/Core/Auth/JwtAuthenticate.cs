using System.Threading.Tasks;
using OktoSDK.BFF;

namespace OktoSDK.Auth
{
    public class JwtAuthenticate : IAuthenticate
    {
        public async Task<bool> Login(string jwtToken)
        {
            return await OktoAuthManager.Authenticate(jwtToken,AuthProvider.JWT);
        }
    }
}
