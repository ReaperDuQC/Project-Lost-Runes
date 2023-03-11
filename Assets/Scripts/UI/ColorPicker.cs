using Google.Protobuf.WellKnownTypes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using ColorUtility = UnityEngine.ColorUtility;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

namespace LostRunes
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
        [SerializeField] Image _previewImage;
        [SerializeField] string _hexCode;

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
        }
        public void SetColorFromColor(Color color)
        {
            Color = color;

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

            _sliderTop.onValueChanged.AddListener(SetTopValue); 
            _sliderMiddle.onValueChanged.AddListener(SetMiddleValue);
            _sliderBottom.onValueChanged.AddListener(SetBottomValue);

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
                SetSliderMinMaxValue(_sliderTop, 0f, 0.99f);
                SetSliderMinMaxValue(_sliderMiddle, 0.01f, 1f);
                SetSliderMinMaxValue(_sliderBottom, 0.01f, 1f);

                UpdateTextForHSV();

                UpdateSlidersFromHSV();

                SetBackgroundsFromHSV();
            }

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
            _backgroundMiddle.color = Color.white; 
            _backgroundMiddle.sprite = _saturationBackground;
            _backgroundBottom.color = Color.white;
            _backgroundBottom.sprite = _valueBackground;
        }
        private void SetBackgroundsFromRGB()
        {
            _backgroundTop.color = Color.red;
            _backgroundTop.sprite = _grayScaleBackground;
            _backgroundMiddle.color = Color.blue;
            _backgroundMiddle.sprite = _grayScaleBackground;
            _backgroundBottom.color = Color.green;
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
            _sliderMiddle.value = Color.b;
            _sliderBottom.value = Color.g;
        }
        void CreateBackgroundSprite(Image image) // supposed to create saturation backgroud image
        {
            if (image == null) return;

            Sprite currentsprite =  image.sprite;
            Texture2D backgroundTexture = new Texture2D(currentsprite.texture.width, currentsprite.texture.height);
            Rect backgroundRect = new Rect(currentsprite.rect.x, currentsprite.rect.y, currentsprite.rect.width, currentsprite.rect.height);


            for (int i = 0; i < backgroundTexture.width; i++)
            {
                float r = (float)i / (float)backgroundTexture.width;
                Color color = Color.Lerp(new Color(1f, 1f, 1f), Color, r);

                for (int j = 0; j < backgroundTexture.height; j++)
                {
                    backgroundTexture.SetPixel(i, j, new Color(color.r, color.g, color.b, 1f));
                }
            }

            backgroundTexture.Apply();
            Sprite newSprite = Sprite.Create(backgroundTexture, backgroundRect, new Vector2(0.5f, 0.5f), currentsprite.pixelsPerUnit);

            image.sprite = newSprite;

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
            }
            SetColorPreview();
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

            _previewImage.color = Color;

            _colorChanged?.Invoke(Color);
        }
        void DisplayHexCode(Color color)
        {
            _hexCode = ColorUtility.ToHtmlStringRGBA(color);
        }
    }
}
