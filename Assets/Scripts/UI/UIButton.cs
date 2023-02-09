using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes
{
    public class UIButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;

        [SerializeField] Image _border;
       //[SerializeField] Image _background;

        [Header("Normal Colors")]
        //Color _normalBackgroundColor;
        Color _normalBorderColor;
        Color _normalTextColor;

        [Header("Highlight Colors")]
        //[SerializeField] Color _highlightBackgroundColor;
        [SerializeField] Color _highlightBorderColor;

        [Header("Pressed Colors")]
        //[SerializeField] Color _pressedBackgroundColor;
        [SerializeField] Color _pressedBorderColor;

        bool _pressed = false;
        bool _mouseOver = false;
        [SerializeField] float _duration = 0.5f;
        private void Awake()
        {
            //_normalBackgroundColor = _background.color;
            _normalBorderColor = _border.color;
            _normalTextColor = _text.color;
        }
        void Highlight()
        {
            StopCoroutine(nameof(SetPanelColor));
            StartCoroutine(SetPanelColor(_border, _border.color, _highlightBorderColor));

            StopCoroutine(nameof(SetTextColor));
            StartCoroutine(SetTextColor(_text, _text.color, _highlightBorderColor));
        }
        void Normal()
        {
            StopCoroutine(nameof(SetPanelColor));
            StartCoroutine(SetPanelColor(_border, _border.color, _normalBorderColor));

            StopCoroutine(nameof(SetTextColor));
            StartCoroutine(SetTextColor(_text, _text.color, _normalTextColor));
        }
        void Pressed()
        {
            StopCoroutine(nameof(SetPanelColor));
            StartCoroutine(SetPanelColor(_border, _border.color, _pressedBorderColor));

            StopCoroutine(nameof(SetTextColor));
            StartCoroutine(SetTextColor(_text, _text.color, _pressedBorderColor));
        }
        public void ButtonEnter()
        {
            Debug.Log("Enter");
            _mouseOver = true;
            Highlight();
        }
        public void ButtonExit()
        {
            Debug.Log("Exit");
            _mouseOver = false;
            Normal();
        }
        public void ButtonPressed()
        {
            Debug.Log("Pressed");
            _pressed = true;

            Pressed();
        }
        public void ButtonReleased()
        {
            Debug.Log("Released");
            _pressed = false;

            if (_mouseOver)
            {
                Highlight();
            }
            else
            {
                Normal();
            }
        }
        IEnumerator SetPanelColor(Image image, Color a, Color b)
        {
            float duration = _duration;

            while(duration > 0f)
            {
                duration -= Time.deltaTime;
                float ratio = 1.0f - (duration / _duration);

                image.color = Color.Lerp(a, b, ratio);

                yield return null;
            }
        }
        IEnumerator SetTextColor(TextMeshProUGUI text, Color a, Color b)
        {
            float duration = _duration;

            while (duration > 0f)
            {
                duration -= Time.deltaTime;
                float ratio = 1.0f - (duration / _duration);

                text.color = Color.Lerp(a, b, ratio);

                yield return null;
            }
        }
    }
}