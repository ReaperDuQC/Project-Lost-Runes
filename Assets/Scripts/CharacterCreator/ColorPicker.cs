using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using ColorUtility = UnityEngine.ColorUtility;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

namespace LostRunes.Menu
{
    public class ColorPicker : MonoBehaviour
    {
        public delegate void OnColorChanged(Color color);
        public OnColorChanged _colorChanged;

        public enum ColorPickerTypes { RGB, HSV };

        [SerializeField] ColorPickerTypes _currentColorType;

        [Header("Sliders")]
        [SerializeField] Slider _sliderTop;
        [SerializeField] Slider _sliderMiddle;
        [SerializeField] Slider _sliderBottom;

        [SerializeField] TextMeshProUGUI _textTop;
        [SerializeField] TextMeshProUGUI _textMiddle;
        [SerializeField] TextMeshProUGUI _textBottom;

        [SerializeField] Image _backgroundTop;
        [SerializeField] Image _backgroundMiddle;
        [SerializeField] Image _backgroundBottom;

        [SerializeField] Color _color;
        Color Color { get { return _color; } set { value.a = 1f; _color = value; } }

        [SerializeField] Sprite _grayScaleBackground;
        [SerializeField] Sprite _hueBackground;
        [SerializeField] Sprite _saturationBackground;
        [SerializeField] Sprite _valueBackground;

        [Header("Preview")]
        [SerializeField] Transform _presetsContainer;
        [SerializeField] List<Color> _presetColors = new();
        [SerializeField] GameObject _presetPrefab;
        [SerializeField] Image _previewImage;
        [SerializeField] string _hexCode;
        Color _initialColor;

        Material _baseMaterial;
        Material _saturationMat;

        bool _initialized = false;

        // refactor to use the same set of sliders instead of a seperate set of slider

        private void Awake()
        {
            _sliderTop.onValueChanged.AddListener(SetTopValue);
            SetSliderMinMaxValue(_sliderTop, 0f, 1f);
             _sliderMiddle.onValueChanged.AddListener(SetMiddleValue);
            SetSliderMinMaxValue(_sliderMiddle, 0f, 1f);
            _sliderBottom.onValueChanged.AddListener(SetBottomValue);
            SetSliderMinMaxValue(_sliderBottom, 0f, 1f);

            SetColorType((int)_currentColorType);
            CreatePresets();
        }
        void CreatePresets()
        {
            if (_presetPrefab == null) return;
            if (_presetsContainer == null) return;

            foreach(var color in _presetColors)
            {
                GameObject preset = Instantiate(_presetPrefab, _presetsContainer);

                CustomButton button = preset.GetComponent<CustomButton>();
                if(button != null)
                {
                    button.onClick.AddListener(new UnityAction(() => SetColorFromColor(color)));
                }

                Image image = preset.GetComponentInChildren<Image>();

                if(image != null)
                {
                    image.color = color;
                }
            }
        }
        public void SetColorFromColor(Color color)
        {
            Color = color;
            _initialColor = !_initialized ? _color : _initialColor;
            if (_currentColorType == ColorPickerTypes.RGB)
            {
                UpdateSlidersFromRGB();
            }
            else
            {
                UpdateSlidersFromHSV();
            }
            SetColorPreview();
        }
        public void SetColorType(int ColorPickerType)
        {
            //if (ColorPickerType == _currentColorType) return; // need a way to initialize

            _currentColorType = (ColorPickerTypes)ColorPickerType;

            _sliderTop.onValueChanged.RemoveAllListeners();
            _sliderMiddle.onValueChanged.RemoveAllListeners();
            _sliderBottom.onValueChanged.RemoveAllListeners();

            if (_currentColorType == ColorPickerTypes.RGB)
            {
                SetSliderMinMaxValue(_sliderTop, 0f, 1f);
                SetSliderMinMaxValue(_sliderMiddle, 0f, 1f);
                SetSliderMinMaxValue(_sliderBottom, 0f, 1f);

                UpdateTextForRGB();

                // need to update slider value from rgb value
                UpdateSlidersFromRGB();
                // update HSV Value From RGB

                // SetBackgroundsFromRGB
                SetBackgroundsFromRGB();
            }
            else
            {
                SetSliderMinMaxValue(_sliderTop, 0f, 1f); // need to check upper clamp to 0.99f
                SetSliderMinMaxValue(_sliderMiddle, 0f, 1f);// need to check bottom clamp 0.01f
                SetSliderMinMaxValue(_sliderBottom, 0f, 1f);// need to check bottom clamp 0.01f

                UpdateTextForHSV();

                UpdateSlidersFromHSV();

                SetBackgroundsFromHSV();
                UpdateBackgroundsFromHSV();
            }

            _sliderTop.onValueChanged.AddListener(SetTopValue);
            _sliderMiddle.onValueChanged.AddListener(SetMiddleValue);
            _sliderBottom.onValueChanged.AddListener(SetBottomValue);

            //set value in slide for color types
        }
        private void UpdateTextForRGB()
        {
           _textTop.text = "R";
            _textMiddle.text = "G";
            _textBottom.text = "B";
        }
        private void UpdateTextForHSV()
        {
            _textTop.text = "H";
            _textMiddle.text = "S";
            _textBottom.text = "V";
        }
        private void SetBackgroundsFromHSV()
        {
            _backgroundTop.color = Color.white;
            _backgroundTop.sprite = _hueBackground;

            if(_backgroundMiddle.material == _baseMaterial)
            {
                _backgroundMiddle.color = Color.white;
                _backgroundMiddle.sprite = null;

                _saturationMat = new Material(Shader.Find("Custom/SaturationGradient"));
                _backgroundMiddle.material = _saturationMat;
                _saturationMat.SetColor("_Color", _color);
            }

            _backgroundBottom.color = Color.white;
            _backgroundBottom.sprite = _valueBackground;
        }
        private void SetBackgroundsFromRGB()
        {
            _backgroundTop.color = Color.red;
            _backgroundTop.sprite = _grayScaleBackground;
            if (_baseMaterial == null)
            {
                _baseMaterial = _backgroundMiddle.material;
            }
            _backgroundMiddle.material = _baseMaterial;
            _backgroundMiddle.color = Color.green;
            _backgroundMiddle.sprite = _grayScaleBackground;
                
            _backgroundBottom.color = Color.blue;
            _backgroundBottom.sprite = _grayScaleBackground;
        }
        private void UpdateSlidersFromHSV()
        {
            Color.RGBToHSV(Color, out float h, out float s, out float v);
            _sliderTop.value = h;
            _sliderMiddle.value = s;
            _sliderBottom.value = v;
        }
        private void UpdateSlidersFromRGB()
        {
            _sliderTop.value = Color.r;
            _sliderMiddle.value = Color.g;
            _sliderBottom.value = Color.b;
        }
        void UpdateSaturationMaterial() 
        {
            if (_saturationMat == null) return;

            _saturationMat.SetColor("_Color", _color);
        }
        void SetSliderMinMaxValue(Slider slider,float min, float max)
        {
            if (slider == null) return;

            slider.minValue = min;   
            slider.maxValue = max;
        }
        void SetTopValue(float value)
        {
            if(_currentColorType == ColorPickerTypes.RGB)
            {
                Color color = Color;
                color.r = value;
                Color = color;

                UpdateSlidersFromRGB();
            }
            else
            {
                Color.RGBToHSV(Color, out float h, out float s, out float v);
                Color = Color.HSVToRGB(value, s, v);
                UpdateSlidersFromHSV();
                UpdateBackgroundsFromHSV();
            }
            SetColorPreview();
        }

        private void UpdateBackgroundsFromHSV()
        {
            UpdateSaturationMaterial();
            _backgroundBottom.color = _color;
        }

        void SetMiddleValue(float value)
        {
            if (_currentColorType == ColorPickerTypes.RGB)
            {
                Color color = Color;
                color.g = value;
                Color = color;
            }
            else
            {
                Color.RGBToHSV(Color, out float h, out float s, out float v);
                Color = Color.HSVToRGB(h, value, v);
                UpdateSlidersFromHSV();
                UpdateBackgroundsFromHSV();
            }
            SetColorPreview();
        }
        void SetBottomValue(float value)
        {
            if (_currentColorType == ColorPickerTypes.RGB)
            {
                Color color = Color;
                color.b = value;
                Color = color;
            }
            else
            {
                Color.RGBToHSV(Color, out float h, out float s, out float v);
                Color = Color.HSVToRGB(h, s, value);
                UpdateSlidersFromHSV();
                UpdateBackgroundsFromHSV();
            }

            SetColorPreview();
        }
        public void SetColorFromHex(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);

            SetColorPreview();
        }
        void SetColorPreview()
        {
            DisplayHexCode(Color);

            //_previewImage.color = Color;

            _colorChanged?.Invoke(Color);
        }
        void DisplayHexCode(Color color)
        {
            _hexCode = ColorUtility.ToHtmlStringRGBA(color);
        }
        public void Reset()
        {
            _color = _initialColor;
            _currentColorType = ColorPickerTypes.RGB;
            SetColorType((int)_currentColorType);
        }
    }
}
