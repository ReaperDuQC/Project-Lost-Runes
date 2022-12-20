using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LostRunes
{
    public class InteractableUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        public void SetInteractText(string text)
        {
            if (_text == null) return;
            _text.text = text;
        }
        public void ShowText(bool show)
        {
            if (_text == null) return;
            _text.gameObject.SetActive(show);
        }
    }
}