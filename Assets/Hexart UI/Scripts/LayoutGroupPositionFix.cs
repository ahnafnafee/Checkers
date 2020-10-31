using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Hexart
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        public LayoutGroup layoutGroup;

        void Start()
        {
            StartCoroutine(ExecuteAfterTime(0.5f));
        }

        public void ManualStart()
        {
            StartCoroutine(ExecuteAfterTime(0.5f));
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
            Destroy(GetComponent<LayoutGroupPositionFix>());
        }
    }
}