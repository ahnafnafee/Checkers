using UnityEngine;

namespace Michsky.UI.Hexart
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
            Debug.Log("It's working :)");
            Application.Quit();
        }
    }
}