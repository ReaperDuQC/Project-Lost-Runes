using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityPBR.Demo
{
    public class InfinityDemoFloatSlider : MonoBehaviour
    {
        public Animator animator;
        public string floatKey;
        public Text keyTitle;

        public void UpdateValue(float value) => animator.SetFloat(floatKey, value);

        public void Setup(string key, Animator newAnimator)
        {
            floatKey = key;
            keyTitle.text = key;
            animator = newAnimator;
        }
    }
    
}
