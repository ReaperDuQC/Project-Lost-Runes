using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace LostRunes.Menu
{
    public class MainMenuUI : RPGMenu
    {
        [Header("Main")]
        [SerializeField] GameObject _mainPanel;

        [Header("Start")]
        [SerializeField] GameObject _lobbyPanel;
        [SerializeField] Button _lobbyReturnButton;

        [Header("Join Games")]
        [SerializeField] GameObject _joinGamesPanel;

        [Header("Host Games")]
        [SerializeField] GameObject _hostGamePanel;

        [Header("Credits")]
        [SerializeField] GameObject _creditsPanel;

        static MainMenuUI _instance;
        public static MainMenuUI Instance { get { return _instance; } }

        [SerializeField] bool _needOnline;
        private void Awake()
        {
            if (_needOnline)
            {
                if (NetworkManager.Singleton == null)
                {
                    SceneManager.LoadScene("Intro");
                    return;
                }
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
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public void ToggleMainPanel()
        {
            if (_mainPanel == null) { return; }

            _mainPanel.SetActive(!_mainPanel.activeSelf);
        }
        public void ToggleStartPanel()
        {
            if (_lobbyPanel == null) { return; }

            _lobbyPanel.SetActive(!_lobbyPanel.activeSelf);
        }
        public void ToggleJoinPanel()
        {
            if (_joinGamesPanel.gameObject == null) { return; }

            _joinGamesPanel.SetActive(!_joinGamesPanel.activeSelf);
        }
        public void ToggleHostPanel()
        {
            if (_hostGamePanel.gameObject == null) { return; }

            _hostGamePanel.SetActive(!_hostGamePanel.activeSelf);
        }
        public void ToggleCreditsPanel()
        {
            if (_creditsPanel.gameObject == null) { return; }

            _creditsPanel.SetActive(!_creditsPanel.activeSelf);
        }
       
        public void LinkHostPanelToReturnButton()
        {
            if(_lobbyReturnButton == null) return;

            _lobbyReturnButton.onClick.AddListener(OnReturnButtonClickedFromHost);
        }
        public void OnReturnButtonClickedFromHost()
        {
            _lobbyReturnButton.onClick.RemoveListener(OnReturnButtonClickedFromHost);
            ToggleHostPanel();
        }
        public void LinkJoinPanelToReturnButton()
        {
            if (_lobbyReturnButton == null) return;

            _lobbyReturnButton.onClick.AddListener(OnReturnButtonClickedFromJoin);
        }
        public void OnReturnButtonClickedFromJoin()
        {
            _lobbyReturnButton.onClick.RemoveListener(OnReturnButtonClickedFromJoin);
            ToggleJoinPanel();
        }
    }
}