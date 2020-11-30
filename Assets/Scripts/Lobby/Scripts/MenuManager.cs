using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

        [SerializeField] Menu[] menus;
        [SerializeField] Button startGameButton;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            startGameButton.interactable = false;
        }

        public void OpenMenu(string menuName)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].menuName == menuName)
                {
                    menus[i].Open();
                }
                else if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }
        }

        public void OpenMenu(Menu menu)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].open)
                {
                    CloseMenu(menus[i]);
                }
            }

            menu.Open();
        }

        public void CloseMenu(Menu menu)
        {
            menu.Close();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Update()
        {
            // Checks if room has 2 players or not
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.Players.Count == 2)
            {
                startGameButton.interactable = true;
            }
            else
            {
                startGameButton.interactable = false;
            }
        }
    }
}