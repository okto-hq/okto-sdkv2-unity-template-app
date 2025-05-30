using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace OktoSDK
{
    public class EmailOtpView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField otpInput;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button openBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button sendOtpBtn;
        [SerializeField] private Button resendOtpBtn;
        [SerializeField] private Button verifyOtpBtn;

        [Header("Logic Reference")]
        [SerializeField] private EmailAuthentication emailAuth;

        [SerializeField] private string lastToken;

        private void OnEnable()
        {
            openBtn.onClick.AddListener(OpenEmailAuth);
            closeBtn.onClick.AddListener(CloseBtn);
            sendOtpBtn.onClick.AddListener(OnSendOtp);
            resendOtpBtn.onClick.AddListener(OnResendOtp);
            verifyOtpBtn.onClick.AddListener(OnVerifyOtp);
        }

        private void OnDisable()
        {
            openBtn.onClick.RemoveListener(OpenEmailAuth);
            closeBtn.onClick.RemoveListener(CloseBtn);
            sendOtpBtn.onClick.RemoveListener(OnSendOtp);
            resendOtpBtn.onClick.RemoveListener(OnResendOtp);
            verifyOtpBtn.onClick.RemoveListener(OnVerifyOtp);
        }

        public void OpenEmailAuth()
        {
            if (panel != null)
                panel.SetActive(true);
        }

        public void CloseBtn()
        {
            if (panel != null)
                panel.SetActive(false);

            if (emailInput != null)
                emailInput.text = string.Empty;

            if (otpInput != null)
                otpInput.text = string.Empty;

        }

        public async void OnSendOtp()
        {
            string email = emailInput?.text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                ResponsePanel.SetResponse("Please enter your email.");
                return;
            }

            Loader.ShowLoader();
            try
            {
                OtpApiResponse resp = await emailAuth.SendEmailOtpAsync(email);
                lastToken = resp.Token;

                ResponsePanel.SetResponse(resp.Message);
            }
            catch (System.Exception ex)
            {
                ResponsePanel.SetResponse($"Error: {ex.Message}");
            }
        }

        public async void OnResendOtp()
        {
            string email = emailInput?.text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                ResponsePanel.SetResponse("Please enter your email.");
                return;
            }
            if (string.IsNullOrEmpty(lastToken))
            {
                ResponsePanel.SetResponse("No token found. Please send OTP first.");
                return;
            }

            Loader.ShowLoader();
            try
            {
                OtpApiResponse resp = await emailAuth.SendEmailOtpAsync(email);
                lastToken = resp.Token;

                ResponsePanel.SetResponse(resp.Message);
            }
            catch (System.Exception ex)
            {
                ResponsePanel.SetResponse($"Error: {ex.Message}");
            }
        }

        public async void OnVerifyOtp()
        {
            string email = emailInput?.text.Trim();
            string otp = otpInput?.text.Trim();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                ResponsePanel.SetResponse("Please enter both email and OTP.");
                return;
            }
            if (string.IsNullOrEmpty(lastToken))
            {
                ResponsePanel.SetResponse("No token found. Please send OTP first.");
                return;
            }

            Loader.ShowLoader();
            try
            {
                var resp = await emailAuth.VerifyEmailOtpAsync(email, otp, lastToken);
                ResponsePanel.SetResponse("Email verified successfully!");
            }
            catch (System.Exception ex)
            {
                ResponsePanel.SetResponse($"Error: {ex.Message}");
            }
        }
    }
}