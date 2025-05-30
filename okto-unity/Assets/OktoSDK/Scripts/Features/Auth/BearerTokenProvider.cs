using UnityEngine;
using OktoSDK.Auth;

namespace OktoSDK.Features.Auth
{
    public class BearerTokenProvider : MonoBehaviour
    {
        public void FetchBearerToken()
        {
            if (OktoAuthManager.GetOktoClient() == null)
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return;
            }

            if (!OktoAuthManager.GetOktoClient().IsLoggedIn())
            {
                ResponsePanel.SetResponse("You are not logged In!");
                return;
            }

            string bearerToken = OktoAuthManager.GetOktoClient().GetAuthorizationToken();
            ResponsePanel.SetResponse(bearerToken);
        }
    }
} 