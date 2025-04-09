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
            onBoardingBtn.onClick.AddListener(OnClickOnBoardingBtn);
        }

        private void OnDisable()
        {
            onBoardingBtn.onClick.RemoveListener(OnClickOnBoardingBtn);
        }

        void OnClickOnBoardingBtn()
        {

            OktoAuthExample.SetConfig();
            onboardingWebview.OpenOnBoardingScreen();
        }

    }

}
