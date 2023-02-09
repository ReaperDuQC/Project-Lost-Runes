using LostRunes.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes
{
    public class CharacterCreatorUI : MonoBehaviour
    {
        [Header("Character Creator")]
        [SerializeField] CharacterCreator _characterCreator;

        [SerializeField] GameObject _facialHairGroup;

        [Header("Gender")]
        [SerializeField] GameObject _genderOptionsGroup;

        [Header("Skin")]
        [SerializeField] GameObject _skinOptionsGroup;
        [SerializeField] ColorPicker _skinColorPicker;

        [Header("Face")]
        [SerializeField] GameObject _faceOptionsGroup;
        [SerializeField] TextMeshProUGUI _faceText;
        int _faceCount = 0;

        [Header("Hair")]
        [SerializeField] GameObject _hairOptionsGroup;
        [SerializeField] TextMeshProUGUI _hairText;
        int _hairCount = 0;

        [Header("Eyebrows")]
        [SerializeField] GameObject _eyebrowsOptionsGroup;
        [SerializeField] TextMeshProUGUI _eyebrowsText;
        int _eyebrowsCount = 0;

        [Header("Beard")]
        [SerializeField] GameObject _beardOptionsGroup;
        [SerializeField] TextMeshProUGUI _beardText;
        int _beardCount = 0;

        [Header("Hair Color")]
        [SerializeField] GameObject _hairColorOptionsGroup;
        [SerializeField] ColorPicker _hairColorPicker;

        private void Awake()
        {
            if (_skinColorPicker != null)
            {
                _skinColorPicker._colorChanged += SetSkinColor;
            }

            if (_hairColorPicker != null)
            {
                _hairColorPicker._colorChanged += SetHairColor;
            }
        }
        private void Start()
        {
            _beardCount = _characterCreator.BeardCount;
            _hairCount = _characterCreator.HairCount;
            _eyebrowsCount = _characterCreator.EyebrowsCount;
            _faceCount = _characterCreator.FaceCount;

            UpdateAllTexts();
        }
        void UpdateText(int optionNumber, int allOptions,TextMeshProUGUI text, string baseText)
        {
            text.text = baseText + " " + optionNumber.ToString() + " / " + allOptions.ToString();
        }
        void UpdateAllTexts()
        {
            int index = 1;
            UpdateText(index, _faceCount, _faceText, "Head");
            UpdateText(index, _hairCount,_hairText, "Hair");
            UpdateText(index, _eyebrowsCount,_eyebrowsText, "Eyebrows");
            UpdateText(index, _beardCount,_beardText, "Beard");
        }
        public void HideAllGroup()
        {
            _genderOptionsGroup.SetActive(false);
            _skinOptionsGroup.SetActive(false);
            _faceOptionsGroup.SetActive(false);
            _hairOptionsGroup.SetActive(false);
            _eyebrowsOptionsGroup.SetActive(false);
            _beardOptionsGroup.SetActive(false);
            _hairColorOptionsGroup.SetActive(false);
        }
        public void SetGender(int genderIndex)
        {
            bool isMale = _characterCreator.SetGender(genderIndex);
            EnableFacialHair(isMale);
        }
        public void SetSkinColor(Color color)
        {
            _characterCreator.SetSkinColor(color);
        }
        public void SetHead(int indexToAdd)
        {
            int index = _characterCreator.SetHead(indexToAdd);
            index += 1;
            UpdateText(index, _faceCount,_faceText, "Head");
        }
        public void SetHair(int indexToAdd)
        {
            int index = _characterCreator.SetHair(indexToAdd);
            index += 1;
            UpdateText(index, _hairCount,_hairText, "Hair");
        }
        public void SetEyebrows(int indexToAdd)
        {
            int index = _characterCreator.SetEyebrows(indexToAdd);
            index += 1;
            UpdateText(index, _eyebrowsCount,_eyebrowsText, "Eyebrows");
        }
        void EnableFacialHair(bool enabled)
        {
            _facialHairGroup.SetActive(enabled);
        }
        public void SetFacialHair(int indexToAdd)
        {
            int index = _characterCreator.SetFacialHair(indexToAdd);
            index += 1;
            UpdateText(index, _beardCount, _beardText, "Beard");
        }
        public void SetHairColor(Color color)
        {
            _characterCreator.SetHairColor(color);
        }
        public void SpawnNewCharacter()
        {
            if (_characterCreator == null) return;

            _characterCreator.CreateNewCharacter();
        }
        public void DestroyCharacter()
        {
            if (_characterCreator == null) return;

            _characterCreator.DestroyCharacter();
        }
    }
}