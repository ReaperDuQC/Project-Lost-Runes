using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LostRunes.Menu
{
    public enum NameStatus { Valid, AlreadyExist, TooShort, TooLong }
    [System.Serializable]
    public class WorldData
    {
        public string _worldName;
        public WorldData(string worldName)
        {
            _worldName = worldName;
        }
    }

    [System.Serializable]
    public class WorldAtlas
    {
        public List<string> Worlds = new List<string>();
        public WorldAtlas() { }

        public WorldAtlas(List<string> worlds)
        {
            Worlds = worlds;
        }
    }
    public class WorldSelectionHandler : MonoBehaviour
    {
        [SerializeField] GameObject _worldButtonPrefab;
        [SerializeField] Transform _content;
        NameStatus _nameStatus = NameStatus.TooShort;

        [SerializeField] CustomButton _validateButton;
        [SerializeField] TextMeshProUGUI _errorMessage;
        [SerializeField] SceneLoaderManager _sceneLoaderManager;

        WorldAtlas _worldAtlas;
        private void Awake()
        {
            LoadWorldsAtlas();
            SetValidateNameButtonInteractable(false);
        }

        public void GenerateNewWorld(TMP_InputField inputField)
        {
            WorldData worldData = new WorldData(inputField.text);
            _worldAtlas.Worlds.Insert(0, inputField.text);
            CreateButtons();
        }
        public void IsWorldValid(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 1)
            {
                _nameStatus = NameStatus.TooShort;
                UpdateErrorMessage("The name of the world is too short.");
                SetValidateNameButtonInteractable(false);
                return;
            }
            if (value.Length > 20)
            {
                _nameStatus = NameStatus.TooLong;
                UpdateErrorMessage("The name of the world is too long.");
                SetValidateNameButtonInteractable(false);
                return;
            }

            foreach (string name in _worldAtlas.Worlds)
            {
                if (name == value)
                {
                    _nameStatus = NameStatus.AlreadyExist;
                    UpdateErrorMessage("Another world alread have that name.");
                    SetValidateNameButtonInteractable(false);
                    return;
                }
            }

            _nameStatus = NameStatus.Valid;
            SetValidateNameButtonInteractable(true);
            UpdateErrorMessage("");
        }
        void UpdateErrorMessage(string message)
        {
            if (_errorMessage == null) return;

            if (string.IsNullOrEmpty(message) || message.Length == 0)
            {
                _errorMessage.text = "";
                return;
            }

            _errorMessage.text = "Error : " + message;
        }
        void SaveWorldsAtlas()
        {
            SaveSystem.SaveSystem.SaveWorldAtlas(_worldAtlas);
        }
        void SetValidateNameButtonInteractable(bool interactable)
        {
            if (_validateButton == null) return;

            _validateButton.interactable = interactable;

        }
        void LoadWorldsAtlas()
        {
            _worldAtlas = SaveSystem.SaveSystem.LoadWorldAtlas();

            CreateButtons();
        }

        private void CreateButtons()
        {
            for (int i = 1; i < _content.childCount; i++)
            {
                Destroy(_content.GetChild(i));
            }

            foreach (string worldName in _worldAtlas.Worlds)
            {
                CreateNewButton(worldName);
            }
        }

        public void LoadWorld(string worldName)
        {
            if(_sceneLoaderManager != null)
            {
                _sceneLoaderManager.LoadScene();
            }
        }
        void CreateNewButton(string worldName)
        {
            if (_content == null) return;
            if (_worldButtonPrefab == null) return;

            CustomButton button = Instantiate(_worldButtonPrefab, _content).GetComponent<CustomButton>();

            button.ButtonText.text = worldName;
            button.onClick.AddListener(new UnityAction(() => LoadWorld(worldName)));

            //CustomButton destroyButton;
            // destroyButton.onClick.AddListener(new UnityAction(() => DeleteExistingWorld(worldName)));
        }
        public void DeleteExistingWorld(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Successfully deleted file: " + fileName);
            }
            else
            {
                Debug.LogError("File not found: " + fileName);
            }
        }
        private void OnDestroy()
        {
            SaveWorldsAtlas();
        }
    }
}