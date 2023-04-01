using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostRunes.Menu
{
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
        public List<string> Worlds = new();
        public WorldAtlas() { }

        public WorldAtlas(List<string> worlds)
        {
            Worlds = worlds;
        }
    }
    public class WorldSelectionHandler : MonoBehaviour
    {
        [SerializeField] GameObject _createButtonPrefab;
        [SerializeField] Transform _content;

        [SerializeField] SceneLoaderManager _sceneLoaderManager;

        [SerializeField] NameInputHandler _nameInputHandler;

        WorldAtlas _worldAtlas;
        private void Awake()
        {
            LoadWorldsAtlas();
        }

        public void GenerateNewWorld(TMP_InputField inputField)
        {
            string worldName = inputField.text;
            WorldData worldData = new(worldName);
            SaveSystem.SaveSystem.SaveWorldData(worldData);
            _worldAtlas.Worlds.Insert(0, worldName);
            CreateButtons();
        }
        void SaveWorldsAtlas()
        {
            SaveSystem.SaveSystem.SaveWorldAtlas(_worldAtlas);
        }
        void LoadWorldsAtlas()
        {
            _worldAtlas = SaveSystem.SaveSystem.LoadWorldAtlas();

            CreateButtons();
        }
        public void CheckName(string name)
        {
            _nameInputHandler.IsNameValid(name, _worldAtlas.Worlds);
        }

        private void CreateButtons()
        {
            List<GameObject> listToDestroy = new();
            for (int i = 1; i < _content.childCount; i++)
            {
                listToDestroy.Add(_content.GetChild(i).gameObject);
            }

            foreach (GameObject obj in listToDestroy)
            {
                Destroy(obj);
            }

            foreach (string worldName in _worldAtlas.Worlds)
            {
                CreateNewButton(worldName);
            }
        }

        public void LoadWorld(string worldName)
        {
            WorldData data = SaveSystem.SaveSystem.LoadWorldData(worldName);
            if(_sceneLoaderManager != null)
            {
                //_sceneLoaderManager.LoadScene();
            }
        }
        void CreateNewButton(string worldName)
        {
            if (_content == null) return;
            if (_createButtonPrefab == null) return;

            CustomButton[] buttons = Instantiate(_createButtonPrefab, _content).GetComponentsInChildren<CustomButton>();

            buttons[0].ButtonText.text = worldName;
            buttons[0].onClick.AddListener(new UnityAction(() => LoadWorld(worldName)));
            buttons[0].onClick.AddListener(new UnityAction(() => GameNetworkManager.Instance.StartHost()));

            buttons[1].onClick.AddListener(new UnityAction(() => DeleteExistingWorld(worldName)));
        }
        public void DeleteExistingWorld(string fileName)
        {
            SaveSystem.SaveSystem.DeleteExistingWorld(fileName);

            LoadWorldsAtlas();
        }
        private void OnDestroy()
        {
            SaveWorldsAtlas();
        }
    }
}