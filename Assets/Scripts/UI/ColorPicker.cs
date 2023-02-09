using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace LostRunes
{
    public class ColorPicker : MonoBehaviour
    {
        public delegate void OnColorChanged(Color color);
        public OnColorChanged _colorChanged;

        [Header("RGBA")]
        [SerializeField][Range(0f, 1f)] float _r;
        [SerializeField][Range(0f, 1f)] float _g;
        [SerializeField][Range(0f, 1f)] float _b;
        [Range(0f, 1f)] float _a = 1f;

        [Header("RGBA UI")]
        [SerializeField] Slider _sliderR;
        [SerializeField] Slider _sliderG;
        [SerializeField] Slider _sliderB;

        [Header("HSV")]
        [SerializeField][Range(0f, 1f)] float _hue;
        [SerializeField][Range(0f, 1f)] float _saturation;
        [SerializeField][Range(0.0001f, 1f)] float _value;

        [Header("HSV UI")]
        [SerializeField] Slider _sliderH;
        [SerializeField] Slider _sliderS;
        [SerializeField] Slider _sliderV;

        [Header("Image")]
        [SerializeField] Image _preview;

        [Header("Settings")]
        [SerializeField] string _hexCode;

        private void Awake()
        {
            _sliderR?.onValueChanged.AddListener(SetRValue);
            _sliderG?.onValueChanged.AddListener(SetGValue);
            _sliderB?.onValueChanged.AddListener(SetBValue);

            _sliderH?.onValueChanged.AddListener(SetHueValue);
            _sliderS?.onValueChanged.AddListener(SetSaturationValue);
            _sliderV?.onValueChanged.AddListener(SetValueValue);
        }
        private void Start()
        {
            SetColorFromColor(new Color(_r, _g, _b));
        }
        private void SetHSVValue(Color color)
        {
            Color.RGBToHSV(color, out _hue, out _saturation, out _value);
            UpdateHSVSliders(_hue, _saturation, _value);
        }

        void UpdateRGBSliders(Color color)
        {
            _sliderR.value = color.r;
            _sliderG.value = color.g;
            _sliderB.value = color.b;
        }
        void SetRValue(float value)
        {
            _r = value;
            SetColorFromColor(new Color(_r, _g, _b));
        }
        void SetGValue(float value)
        {
            _g = value;
            SetColorFromColor(new Color(_r, _g, _b));
        }
        void SetBValue(float value)
        {
            _b = value;
            SetColorFromColor(new Color(_r,_g,_b));
        }
        void UpdateHSVSliders(float h, float s, float v)
        {
            _sliderH.value = h;
            _sliderS.value = s;
            _sliderV.value = v;
        }

        void SetHueValue(float value)
        {
            _hue = value;
            SetColorFromColor(Color.HSVToRGB(_hue, _saturation, _value));
        }
        void SetSaturationValue(float value)
        {
            _saturation = value;
            SetColorFromColor(Color.HSVToRGB(_hue, _saturation, _value));
        }
        void SetValueValue(float value)
        {
            _value = value;
            SetColorFromColor(Color.HSVToRGB(_hue, _saturation, _value));
        }
        void SetRGBValue(Color color)
        {
            _r = color.r;
            _g = color.g;
            _b = color.b;
            _a = color.a;

            UpdateRGBSliders(color);
        }
        public void SetColorFromHex(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);

            SetRGBValue(color);
            SetHSVValue(color);
            SetColor(color);
        }

        public void SetColorFromColor(Color color)
        {
            SetRGBValue(color);
            SetHSVValue(color);
            SetColor(color);
        }
        void SetColor(Color color)
        {
            DisplayHexCode(color);

            _preview.color = color;

            _colorChanged?.Invoke(color);
        }
        void DisplayHexCode(Color color)
        {
            _hexCode = ColorUtility.ToHtmlStringRGBA(color);
        }
    }
}
