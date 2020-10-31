using UnityEngine;

namespace Michsky.UI.Hexart
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public GameObject splashScreen;
        public GameObject mainPanels;
        public GameObject homePanel;
        private Animator mainPanelsAnimator;
        private Animator homePanelAnimator;
        private BlurManager bManager;

        [Header("SETTINGS")]
        public bool disableSplashScreen;

        void Start()
        {
            bManager = gameObject.GetComponent<BlurManager>();

            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator = mainPanels.GetComponent<Animator>();
                mainPanelsAnimator.Play("Main Panel Opening");
                homePanelAnimator = homePanel.GetComponent<Animator>();
                homePanelAnimator.Play("MP Fade-in Start");
                bManager.BlurInAnim();
            }

            else
            {
                splashScreen.SetActive(true);
                mainPanels.SetActive(false);
            }
        }
    }
}