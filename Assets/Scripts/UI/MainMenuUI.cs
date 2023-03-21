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
        [SerializeField] CustomMenuPanel _mainPanel;

        [Header("Multiplayer")]
        [SerializeField] CustomMenuPanel _multiplayerPanel;

        [Header("World Selection")]
        [SerializeField] CustomMenuPanel _worldSelectionPanel;

        [Header("World Name")]
        [SerializeField] CustomMenuPanel _worldNamePanel;

        [Header("Achievements")]
        [SerializeField] CustomMenuPanel _achievementsPanel;

        [Header("Credits")]
        [SerializeField] CustomMenuPanel _creditsPanel;

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
            _activePanel = _mainPanel;
            _basePanel = _activePanel;
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public void SetMainPanelActive(bool active)
        {
            SetPanelActive(_mainPanel, active);
        }
        public void SetMultiplayerPanelActive(bool active)
        {
            SetPanelActive(_multiplayerPanel, active);
        }
        public void SetWorldSelectionPanelActive(bool active)
        {
            SetPanelActive(_worldSelectionPanel, active);
        }
        public void SetWorldNamePanelActive(bool active)
        {
            SetPanelActive(_worldNamePanel, active);
        }
        public void SetAchievementsPanelActive(bool active)
        {
            SetPanelActive(_achievementsPanel, active);
        }
        public void SetCreditsPanelActive(bool active)
        {
            SetPanelActive(_creditsPanel, active);
        }
    }
}