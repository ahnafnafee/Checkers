using UnityEngine;

namespace Michsky.UI.Hexart
{
	public class HUDManager : MonoBehaviour 
	{
		[Header("RESOURCES")]
		public Animator pauseMenu;
		public Animator pausePanel;

        private BlurManager bManager;

		bool isOpen;

        void Start()
        {
            bManager = gameObject.GetComponent<BlurManager>();
        }

		public void OpenCloseMenu () 
		{
			if(isOpen == false)
			{
				pauseMenu.Play("Pause Menu Opening");
				pausePanel.Play("Pause Menu In");
				isOpen = true;
                bManager.BlurInAnim();
			}

			else
			{
				pauseMenu.Play("Pause Menu Closing");
				pausePanel.Play("MP Fade-out");
				isOpen = false;
                bManager.BlurOutAnim();
            }
		}
	}
}