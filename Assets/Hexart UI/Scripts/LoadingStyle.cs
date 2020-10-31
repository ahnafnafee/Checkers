using UnityEngine;

namespace Michsky.UI.Hexart
{
    public class LoadingStyle : MonoBehaviour
    {
        public void SetStyle(string prefabToLoad)
        {
            Michsky.UI.Hexart.LoadingScreen.prefabName = prefabToLoad;
        }
    }
}