using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        protected float _current;
        protected float _max;
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] RectTransform _uiBar;
        float _duration = 1.5f;

        float _baseWidth = 100f;
        public virtual void Initialize(CharacterStats stats)
        {
        }
        public void SetMaxValue(float value)
        {
            _max = value;
            _slider.maxValue = _max;

            UpdateText();

            StartCoroutine(ResizeUiBar(value));
        }
        public void SetCurrentValue(float value)
        {
            _current = value;
            _slider.value = _current;
            UpdateText();
        }

        protected  void UpdateText()
        {
            _text.text = ((int)_current).ToString() + " / " + ((int)_max).ToString(); ;
        }

        IEnumerator ResizeUiBar(float newMax)
        {
            if(_uiBar != null) 
            {
                float currentMax = _max;

                float current = _duration;

                while (current > 0f)
                {
                    float t = 1f - (current / _duration);
                    t = t * t * (3f - 2f * t);
                    _max =   Mathf.Lerp(currentMax, newMax, t);
                    _uiBar.sizeDelta = new(_baseWidth + _max, _uiBar.rect.height);
                    UpdateText();
                    yield return null;
                    current -= Time.deltaTime;
                }
            }
        }
    }
}