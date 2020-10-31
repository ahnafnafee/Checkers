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
                EditorPrefs.SetInt("HexUI.Installed", 1);
                EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Hexart UI.\r\rYou can check Documentation and Read Me file for help, or contact me at isa.steam@outlook.com", "Got it!");
            }
        }
    }
}