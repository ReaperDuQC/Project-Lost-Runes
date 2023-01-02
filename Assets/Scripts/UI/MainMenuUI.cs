using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace LostRunes.Menu
{

    // Get rid of the word Browse -> use Join Instead (Dylan)
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] Image _mainPanel;

        [Header("Start")]
        [SerializeField] Image _startPanel;
        [SerializeField] Button _startReturnButton;
        [SerializeField] Button _continueButton;

        [Header("Join Games")]
        [SerializeField] Image _joinGamesPanel;

        [Header("Host Games")]
        [SerializeField] Image _hostGamePanel;

        [Header("Option")]
        [SerializeField] OptionMenuUI _optionMenu;
        public OptionMenuUI OptionMenuUI { get { return _optionMenu; } }

        [Header("Credits")]
        [SerializeField] Image _creditsPanel;

        [Header("Scene Transition")]
        [SerializeField] SceneTransition _sceneTransition;

        [Header("Audio Components")]
        [SerializeField] BackgroundMusicPlayer _backgroundMusicPlayer;
        [SerializeField] MenuNavigationSound _menuNavigationSound;

        static MainMenuUI _instance;
        public static MainMenuUI Instance { get { return _instance; } }

        [Header("UI Buttons")]
        [SerializeField] List<Button> _returnButtons;
        [SerializeField] List<Button> _normalButtons;
        private void Awake()
        {
            if(NetworkManager.Singleton == null)
            {
                SceneManager.LoadScene("Intro");
            }

            if (_instance == null)
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
            LinkButtons();
            LoadOptionData();
            PlayBackgroundMusic();
        }
        private void LoadOptionData()
        {
            _optionMenu.LoadOptionData();
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
        public void ToggleOptionPanel()
        {
            if (_optionMenu == null) { return; }

            _optionMenu.gameObject.SetActive(!_optionMenu.gameObject.activeSelf);
        }
        public void ToggleJoinPanel()
        {
            if (_joinGamesPanel.gameObject == null) { return; }

            _joinGamesPanel.gameObject.SetActive(!_joinGamesPanel.gameObject.activeSelf);
        }
        public void ToggleHostPanel()
        {
            if (_hostGamePanel.gameObject == null) { return; }

            _hostGamePanel.gameObject.SetActive(!_hostGamePanel.gameObject.activeSelf);
        }
        public void ToggleCreditsPanel()
        {
            if (_creditsPanel.gameObject == null) { return; }

            _creditsPanel.gameObject.SetActive(!_creditsPanel.gameObject.activeSelf);
        }
        void PlayBackgroundMusic()
        {
            if (_backgroundMusicPlayer == null) return;

            _backgroundMusicPlayer.InitializeAudioSource();
        }
        void SubscribeAudioSources()
        {
            if (_backgroundMusicPlayer != null) 
            { 
                _backgroundMusicPlayer.SubscribeAudioSource();
            }

            if (_menuNavigationSound != null)
            {
                _menuNavigationSound.SubscribeAudioSource();
            }
        }
        public void LinkHostPanelToReturnButton()
        {
            if(_startReturnButton == null) return;

            _startReturnButton.onClick.AddListener(OnReturnButtonClickedFromHost);
        }
        public void OnReturnButtonClickedFromHost()
        {
            _startReturnButton.onClick.RemoveListener(OnReturnButtonClickedFromHost);
            ToggleHostPanel();
        }
        public void LinkJoinPanelToReturnButton()
        {
            if (_startReturnButton == null) return;

            _startReturnButton.onClick.AddListener(OnReturnButtonClickedFromJoin);
        }
        public void OnReturnButtonClickedFromJoin()
        {
            _startReturnButton.onClick.RemoveListener(OnReturnButtonClickedFromJoin);
            ToggleJoinPanel();
        }
        void LinkButtons()
        {
            foreach(Button b in _normalButtons)
            {
                b.onClick.AddListener(_menuNavigationSound.PlayForwardNavigationSound);
            }
            foreach (Button b in _returnButtons)
            {
                b.onClick.AddListener(_menuNavigationSound.PlayBackwardNavigationSound);
            }
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}