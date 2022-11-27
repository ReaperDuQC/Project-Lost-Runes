using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.Menu
{
    public class MainMenu : RPGMenu
    {
        [Header("Main")]
        [SerializeField] Image _mainPanel;

        [Header("Start")]
        [SerializeField] Image _startPanel;
        [SerializeField] Button _continueButton;

        [Header("Character Creation")]
        [SerializeField] Image _characterCreationPanel;
        [SerializeField] CharacterCreator _characterCreator;

        [Header("Option")]
        [SerializeField] Image _optionPanel;
        [SerializeField] Image _graphicOptionPanel;
        [SerializeField] GraphicSettings _graphicSettings;
        [SerializeField] Image _audioOptionPanel;
     
        [SerializeField] Image _gameplayOptionPanel;
        [SerializeField] GameplaySettings _gameplaySettings;

        [Header("Scene Transition")]
        [SerializeField] SceneTransition _sceneTransition;

        [SerializeField] AudioSource _backgroundMusicSource;
        [SerializeField] AudioClip _backgroundMusic;

        static MainMenu _instance;
        public static MainMenu Instance { get { return _instance; } }

        private void Awake()
        {
            if( _instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            SubscribeAudioSources();
            LoadOptionData();
            PlayBackgroundMusic();
        }
        protected override void SubscribeAudioSources()
        {
            base.SubscribeAudioSources();
            AudioSettings.SubscribeToMusicAudioSource(_backgroundMusicSource);
        }
        private void LoadOptionData()
        {
            AudioSettings.LoadAudioSettingsData();
            _graphicSettings.LoadGraphicSettingsData();
            _gameplaySettings.LoadGameplaySettingsData();
        }
        public void ToggleMainPanel()
        {
            if (_mainPanel == null) { return; }

            _mainPanel.gameObject.SetActive(!_mainPanel.gameObject.activeSelf);
        }
        public void ToggleStartPanel()
        {
            if (_startPanel == null) { return; }

            _startPanel.gameObject.SetActive(!_startPanel.gameObject.activeSelf);

            if (_startPanel.gameObject.activeSelf)
            {
                if (_continueButton != null)
                {
                    bool isContinue = SaveSystem.SaveSystem.LoadContinueData();
                    _continueButton.interactable = isContinue;
                    _continueButton.gameObject.SetActive(isContinue);
                }
            }
        }
        public void ToggleCharacterCreationPanel()
        {
            if (_characterCreationPanel == null) { return; }

            _characterCreationPanel.gameObject.SetActive(!_characterCreationPanel.gameObject.activeSelf);
        }
        public void ToggleOptionPanel()
        {
            if (_optionPanel == null) { return; }

            _optionPanel.gameObject.SetActive(!_optionPanel.gameObject.activeSelf);
        }
        public void DisplayVideoOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(true);
            _audioOptionPanel.gameObject.SetActive(false);
            _gameplayOptionPanel.gameObject.SetActive(false);
        }
        public void DisplayAudioOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(false);
            _audioOptionPanel.gameObject.SetActive(true);
            _gameplayOptionPanel.gameObject.SetActive(false);
        }
        public void DisplayGameplayOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(false);
            _audioOptionPanel.gameObject.SetActive(false);
            _gameplayOptionPanel.gameObject.SetActive(true);
        }
        void PlayBackgroundMusic()
        {
            if (_backgroundMusic == null) return;
            if (_backgroundMusicSource == null) return;
            _backgroundMusicSource.loop = true;
            _backgroundMusicSource.clip = _backgroundMusic;

            _backgroundMusicSource.Play();
        }
    }
}