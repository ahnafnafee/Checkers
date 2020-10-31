using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Hexart
{
    public class TimedAction : MonoBehaviour
    {
        [Header("TIMING (SECONDS)")]
        public float timer = 4;

        [Header("END ACTION")]
        public UnityEvent timerAction;

        void Start()
        {
            StartCoroutine(TimedEvent());
        }

        IEnumerator TimedEvent()
        {
            yield return new WaitForSeconds(timer);
            timerAction.Invoke();
        }
    }
}
