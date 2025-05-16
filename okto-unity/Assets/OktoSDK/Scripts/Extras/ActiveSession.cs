using Newtonsoft.Json;
using OktoSDK.Auth;
using UnityEngine;
using UnityEngine.UI;

//This script returns active session of user
namespace OktoSDK
{
    public class ActiveSession : MonoBehaviour
    {
        [SerializeField]
        private Button activeSession;

        private void OnEnable()
        {
            activeSession.onClick.AddListener(ShowActiveSession);
        }

        private void OnDisable()
        {
            activeSession.onClick.RemoveListener(ShowActiveSession);
        }

        void ShowActiveSession()
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


            string jsonString = JsonConvert.SerializeObject(OktoAuthManager.GetOktoClient()._userDetails, Formatting.Indented);
            CustomLogger.Log("Active Session : " + jsonString);
            if (string.IsNullOrEmpty(jsonString) || jsonString.Equals("null"))
            {
                ResponsePanel.SetResponse("No Active Session Exists!");
                return;
            }
            ResponsePanel.SetResponse(jsonString);
        }

    }
}
