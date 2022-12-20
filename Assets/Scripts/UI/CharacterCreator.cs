using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LostRunes.Menu
{
    public class CharacterCreator : MonoBehaviour
    {
        string _characterName;
        public string CharacterName { get { return _characterName; } }
        bool _isMale = true;
        public bool IsMale { get { return _isMale; } }

        [SerializeField] TMP_InputField _inputField;
        [SerializeField] Button _startButton;
        [SerializeField] TextMeshProUGUI[] _sexTexts;
        [SerializeField] Color[] _colors;
        [SerializeField] GameObject[] _characters;

        private void Awake()
        {
            SelectCharacterSex(0);
            SetStartButtonActive(false);
        }
        public void OnNameChanged(string currentName)
        {
            bool isNameValid = currentName.Length > 0;

            if (isNameValid) 
            {
                _characterName = currentName;
                SetStartButtonActive(isNameValid);
            }
        }
        public void SetStartButtonActive(bool isActive)
        {
            _startButton.interactable = isActive;
        }
        public void SelectCharacterSex(int sexIndex)
        {
            sexIndex = sexIndex % _characters.Length;
            _isMale = sexIndex == 0;
            _characters[sexIndex].SetActive(true);
            _sexTexts[sexIndex].color = _colors[0];
            int nextIndex = (sexIndex + 1) % _characters.Length;
            _characters[nextIndex].SetActive(false);
            _sexTexts[nextIndex].color = _colors[1];
        }
    }
}