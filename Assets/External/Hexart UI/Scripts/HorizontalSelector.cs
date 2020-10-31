using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Michsky.UI.Hexart
{
    public class HorizontalSelector : MonoBehaviour
    {
        private Text label;
        private Text labeHelper;
        private Animator selectorAnimator;

        [Header("SETTINGS")]
        private int index = 0;
        public int defaultIndex = 0;

        [Header("ELEMENTS")]
        public List<string> elements = new List<string>();

        [Header("EVENT")]
        public UnityEvent onValueChanged;

        void Start()
        {
            selectorAnimator = gameObject.GetComponent<Animator>();
            label = transform.Find("Text").GetComponent<Text>();
            labeHelper = transform.Find("Text Helper").GetComponent<Text>();
            label.text = elements[defaultIndex];
            labeHelper.text = label.text;
        }

        public void PreviousClick()
        {
            labeHelper.text = label.text;

            if (index == 0)
            {
                index = elements.Count - 1;
            }

            else
            {
                index--;
            }

            onValueChanged.Invoke();
            label.text = elements[index];

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();
            selectorAnimator.Play("Previous");
        }

        public void ForwardClick()
        {
            labeHelper.text = label.text;

            if ((index + 1) >= elements.Count)
            {
                index = 0;
            }

            else
            {
                index++;
            }

            onValueChanged.Invoke();
            label.text = elements[index];

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();
            selectorAnimator.Play("Forward");
        }
    }
}