using LostRunes.Menu;
using LostRunes.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostRunes.Menu
{
    public class CharacterCreatorUI : MonoBehaviour
    {
        [SerializeField] GameMenu _gameMenu;
        [SerializeField] GameObject _createButtonPrefab;
        [SerializeField] Transform _content;
        NameStatus _nameStatus = NameStatus.TooShort;

        [SerializeField] StatRoller _statRoller;
        [SerializeField] GameObject _playerUI;

        PlayerAtlas _playerAtlas;
        [SerializeField] CustomButton _nameButton;

        [Header("Character Creator")]
        [SerializeField] CharacterCreator _characterCreator;

        [SerializeField] GameObject _beardButton;

        [Header("Name Input")]
        [SerializeField] NameInputHandler _nameInputHandler;

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

        [SerializeField] GameObject _cameraPreview;

        private void Awake()
        {
            LoadPlayersAtlas();
            _cameraPreview.SetActive(true);
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

            Material mat =_characterCreator.GetCharacterMaterial();

            _skinColorPicker.SetColorFromColor(mat.GetColor("_Color_Skin"));
            _hairColorPicker.SetColorFromColor( mat.GetColor("_Color_Hair"));
            UpdateAllTexts();
        }
        void UpdateText(int optionNumber, int allOptions,TextMeshProUGUI text, string baseText)
        {
            text.text = baseText + " " + optionNumber.ToString() + " / " + allOptions.ToString();
        }
        void UpdateAllTexts()
        {
            UpdateText(1, _faceCount, _faceText, "Head");
            UpdateText(0, _hairCount,_hairText, "Hair");
            UpdateText(1, _eyebrowsCount,_eyebrowsText, "Eyebrows");
            UpdateText(0, _beardCount,_beardText, "Beard");
        }
        public void ResetInputText()
        {
            UpdateNameButton("Character Name");

            _characterCreator.SetCharacterName("");

            _nameInputHandler.ResetInputText();
        }
        public void SetCharacterName()
        {
            if (_nameInputHandler == null) return;
            string characterName = _nameInputHandler.GetName();

            _characterCreator.SetCharacterName(characterName);

            UpdateNameButton(characterName);
        }

        private void UpdateNameButton(string text)
        {
            if (_nameButton == null) return;

            _nameButton.ButtonText.text = text;
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
            _beardButton.SetActive(enabled);
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
        public void GenerateNewPlayer(TMP_InputField inputField)
        {
            PlayerData playerData = _characterCreator.GetPlayerData();
            playerData._statData.SetRolledStats( _statRoller.SaveStats());

            SaveSystem.SaveSystem.SavePlayerData(playerData);
            _playerAtlas.Players.Insert(0, inputField.text);
            CreateButtons();
        }
        void SavePlayersAtlas()
        {
            SaveSystem.SaveSystem.SavePlayerAtlas(_playerAtlas);
        }
        void LoadPlayersAtlas()
        {
            _playerAtlas = SaveSystem.SaveSystem.LoadPlayerAtlas();

            CreateButtons();
        }
        public void CheckName(string name)
        {
            _nameInputHandler.IsNameValid(name, _playerAtlas.Players);
        }

        private void CreateButtons()
        {
            List<GameObject> listToDestroy = new();
            for (int i = 1; i < _content.childCount; i++)
            {
                listToDestroy.Add(_content.GetChild(i).gameObject);
            }

            foreach(GameObject obj in listToDestroy)
            {
                Destroy(obj);
            }

            foreach (string worldName in _playerAtlas.Players)
            {
                CreateNewButton(worldName);
            }
        }

        public void LoadCharacter(string playerName)
        {
            PlayerData playerData = SaveSystem.SaveSystem.LoadPlayerData(playerName);

            _characterCreator.CreateCharacter(playerData);
        }
        void CreateNewButton(string playerName)
        {
            if (_content == null) return;
            if (_createButtonPrefab == null) return;

            CustomButton[] buttons = Instantiate(_createButtonPrefab, _content).GetComponentsInChildren<CustomButton>();

            buttons[0].ButtonText.text = playerName;
            buttons[0].onClick.AddListener(new UnityAction(() => LoadCharacter(playerName)));
            buttons[0].onClick.AddListener(new UnityAction(() => Play()));

            buttons[1].onClick.AddListener(new UnityAction(() => DeleteExistingPlayer(playerName)));
        }
        public void DeleteExistingPlayer(string fileName)
        {
            SaveSystem.SaveSystem.DeleteExistingPlayer(fileName);

            LoadPlayersAtlas();
        }
        private void OnDestroy()
        {
            SavePlayersAtlas();
        }
        private void Play()
        {
            this.gameObject.SetActive(false);
            // Enable usage of normal Menu
            _gameMenu.EnableMenuInteraction(true);

            // Initialize Player

            _cameraPreview.SetActive(false);
            _playerUI.SetActive(true);
        }
        public void ResetNewCharacter()
        {
            _hairColorPicker.Reset();
            _skinColorPicker.Reset();
            ResetInputText();
            DestroyCharacter();
            SpawnNewCharacter();
            UpdateAllTexts();
        }
    }
}