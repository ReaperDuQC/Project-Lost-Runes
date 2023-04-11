using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostRunes
{

    public class CustomButton : Button
    {

        [SerializeField] MenuNavigationSound _menuNavigationSound;
        public MenuNavigationSound MenuNavigationSound { get { return _menuNavigationSound; } }
        [SerializeField] TextMeshProUGUI _buttonText;
        public TextMeshProUGUI ButtonText { get { return _buttonText; } set { _buttonText = value; } }

        [SerializeField] bool _changeScale;
        public bool ChangeScale { get { return _changeScale; } set { _changeScale = value; } }

        [SerializeField] bool _isReturnButton;
        public  bool IsReturnButton { get { return _isReturnButton; } }
        protected override void OnEnable()
        {
            base.OnEnable();

            if(_menuNavigationSound == null)
            {
                _menuNavigationSound = FindObjectOfType<MenuNavigationSound>();
            }

            if(_buttonText == null)
            {
                _buttonText = transform.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            Color tintColor;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    if(_menuNavigationSound != null)
                    {
                        _menuNavigationSound.PlayOnButtonHighlightNavigationSound();
                    }
                    tintColor = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    if (_menuNavigationSound != null)
                    {
                        if (_isReturnButton)
                        {
                            _menuNavigationSound.PlayBackwardNavigationSound();
                        }
                        else
                        {
                            _menuNavigationSound.PlayForwardNavigationSound();
                        }
                    }
                    tintColor = colors.pressedColor;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    break;
                default:
                    tintColor = Color.black;
                    break;
            }

            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null) return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);

            if (_buttonText == null) return;

            //_buttonText.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }
    }
}