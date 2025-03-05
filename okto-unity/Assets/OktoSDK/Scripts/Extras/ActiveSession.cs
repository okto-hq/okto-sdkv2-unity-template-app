using Newtonsoft.Json;
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
            string jsonString = JsonConvert.SerializeObject(OktoAuthExample.GetSession(), Formatting.Indented);
            Debug.Log("Active Session : " + jsonString);
            if (string.IsNullOrEmpty(jsonString) || jsonString.Equals("null"))
            {
                ResponsePanel.SetResponse("No Active Session Exists!");
                return;
            }
            ResponsePanel.SetResponse(jsonString);
        }

    }
}
