using OktoSDK.Auth;
using UnityEngine;
using UnityEngine.UI;

//script to manage onboarding ui
namespace OktoSDK
{
    public class OnBoardingUIManager : MonoBehaviour
    {
        [SerializeField]
        private Button onBoardingBtn;

        [SerializeField]
        private OnboardingWebview onboardingWebview;


        private void OnEnable()
        {
            if (onBoardingBtn != null)
                onBoardingBtn.onClick.AddListener(OnClickOnBoardingBtn);
        }

        private void OnDisable()
        {
            if (onBoardingBtn != null)
                onBoardingBtn.onClick.RemoveListener(OnClickOnBoardingBtn);
        }

        void OnClickOnBoardingBtn()
        {
            OktoAuthManager.SetConfig();
            onboardingWebview.OpenOnBoardingScreen();
        }

    }

}
