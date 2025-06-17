using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OktoSDK.Auth
{
    public class JwtAuthenticateView : MonoBehaviour
    {
        private JwtAuthenticate jwtAuthenticate;

        [SerializeField]
        private TMP_InputField jwtTokenField;

        [SerializeField]
        private Button jwtAuthBtn;

        [SerializeField]
        private Button jwtAuthCloseBtn;

        [SerializeField]
        private Button jwtAuthOpenBtn;

        [SerializeField]
        private GameObject jwtAuthPanel;

        private void OnEnable()
        {
            jwtAuthenticate = new JwtAuthenticate();
            jwtAuthBtn.onClick.AddListener(OnClickAuthenticate);
            jwtAuthCloseBtn.onClick.AddListener(OnClickClose);
            jwtAuthOpenBtn.onClick.AddListener(OnClickOpen);
        }

        private void OnDisable()
        {
            jwtAuthBtn.onClick.RemoveListener(OnClickAuthenticate);
            jwtAuthCloseBtn.onClick.AddListener(OnClickClose);
            jwtAuthOpenBtn.onClick.AddListener(OnClickOpen);
        }

        void OnClickAuthenticate()
        {
            if (!string.IsNullOrEmpty(jwtTokenField.text))
            {
                Loader.ShowLoader();
                jwtAuthenticate.Login(jwtTokenField.text);
            }
            else
            {
                ResponsePanel.SetResponse("JWT Token Field can't be Empty!");
            }
        }

        void OnClickClose()
        {
            jwtTokenField.text = string.Empty;
            jwtAuthPanel.SetActive(false);
        }

        void OnClickOpen()
        {
            jwtTokenField.text = string.Empty;
            jwtAuthPanel.SetActive(true);
        }
    }
}
