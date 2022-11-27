using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        public void SetMaxValue(float value)
        {
            _slider.value = value;
        }
        public void SetCurrentValue(float value)
        {
            _slider.value = value;
        }
    }
}