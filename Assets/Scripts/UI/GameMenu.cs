using RPG.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = RPG.Menu.AudioSettings;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design;

namespace RPG
{
    public class GameMenu : RPGMenu
    {
        [Header("Menu")]
        [SerializeField] Image _menuPanel;
        [Header("Equipment")]
        [SerializeField] Button _equipmentReturnButton;
        [SerializeField] Image _equipmentPanel;
        [Header("Inventory")]
        [SerializeField] Button _inventoryReturnButton;
        [SerializeField] Image _inventoryPanel;
        [Header("Option")]
        [SerializeField] Button _optionReturnButton;
        [SerializeField] Image _optionPanel;
        [SerializeField] Image _graphicOptionPanel;
        [SerializeField] GraphicSettings _graphicSettings;
        [SerializeField] Image _audioOptionPanel;

        [SerializeField] Image _gameplayOptionPanel;
        [SerializeField] GameplaySettings _gameplaySettings;
        [Header("Quit")]
        [SerializeField] Button _quitReturnButton;
        [SerializeField] Image _quitPanel;
        PlayerControls _playerControls;
        private void Awake()
        {
            LoadOptionData();
        }
        private void Start()
        {
            SubscribeAudioSources();
        }
        private void OnEnable()
        {
            if(_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.PlayerActions.Start.performed += i => CloseMenu();
            }
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        protected override void SubscribeAudioSources()
        {
            base.SubscribeAudioSources();
        }
        private void LoadOptionData()
        {
            AudioSettings.LoadAudioSettingsData();
            _graphicSettings.LoadGraphicSettingsData();
            _gameplaySettings.LoadGameplaySettingsData();
        }
        public void ToggleMenuPanel()
        {
            if (_menuPanel == null) return;

            _menuPanel.gameObject.SetActive(!_menuPanel.gameObject.activeSelf);
        }
        public void ToggleOptionPanel()
        {
            if (_optionPanel == null) return;

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
        public void ToggleEquipmentPanel()
        {
            if (_equipmentPanel == null) return;

            _equipmentPanel.gameObject.SetActive(!_equipmentPanel.gameObject.activeSelf);
        }
        public void ToggleInventoryPanel()
        {
            if (_inventoryPanel == null) return;

            _inventoryPanel.gameObject.SetActive(!_inventoryPanel.gameObject.activeSelf);
        }
        public void ToggleQuitPanel()
        {
            if (_quitPanel == null) return;

            _quitPanel.gameObject.SetActive(!_quitPanel.gameObject.activeSelf);
        }
        private void CloseMenu()
        {
            if (_quitPanel.gameObject.activeSelf)
            {
                ToggleQuitPanel();
                ToggleMenuPanel();
            }
            else if (_inventoryPanel.gameObject.activeSelf)
            {
                ToggleInventoryPanel();
                ToggleMenuPanel();
            }
            else if (_equipmentPanel.gameObject.activeSelf)
            {
                ToggleEquipmentPanel();
                ToggleMenuPanel();
            }
            else if (_optionPanel.gameObject.activeSelf)
            {
                ToggleOptionPanel();
                ToggleMenuPanel();
            }
            else if (_menuPanel.gameObject.activeSelf)
            {
                ToggleMenuPanel();
                ToggleCursorVisibility(false);
            }
            else
            {
                ToggleMenuPanel();
                ToggleCursorVisibility(true);
            }
        }
        private void ToggleCursorVisibility(bool visible)
        {
            GameManager instance = GameManager.Instance;
            if (instance != null)
            {
                instance.HideCursor(visible);
            }
        }
    }
}