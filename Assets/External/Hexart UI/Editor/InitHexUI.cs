using UnityEngine;
using UnityEditor;

public class InitHexUI : MonoBehaviour
{
    [InitializeOnLoad]
    public class InitOnLoad
    {
        static InitOnLoad()
        {
            if (!EditorPrefs.HasKey("HexUI.Installed"))
            {
                Debug.Log("Welcome to Checkers Party!");
            }
        }
    }
}