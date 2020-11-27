using UnityEditor;
using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class InitMUIP
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("MUIPv4.Installed"))
                {
                    Debug.Log("Welcome to Checkers Party!");
                }
            }
        }
    }
}