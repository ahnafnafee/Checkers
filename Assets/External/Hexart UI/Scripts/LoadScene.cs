using UnityEngine;
using UnityEngine.SceneManagement;

namespace Michsky.UI.Hexart
{
    public class LoadScene : MonoBehaviour
    {
        public void ChangeToScene(string sceneName)
        {
            Michsky.UI.Hexart.LoadingScreen.LoadScene(sceneName);
        }
    }
}