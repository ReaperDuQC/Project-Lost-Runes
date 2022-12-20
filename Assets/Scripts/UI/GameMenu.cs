using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostRunes.Menu.AudioSettings;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design;

namespace LostRunes
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
        [SerializeField] OptionMenuUI _optionMenu;
        [SerializeField] Button _optionReturnButton;

        [Header("Quit")]
        [SerializeField] Button _quitReturnButton;
        [SerializeField] Image _quitPanel;
        PlayerControls _playerControls;
        private void Awake()
        {
            LoadOptionData();
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
        void LoadOptionData()
        {

        }
        public void ToggleMenuPanel()
        {
            if (_menuPanel == null) return;

            _menuPanel.gameObject.SetActive(!_menuPanel.gameObject.activeSelf);
        }
        public void ToggleOptionPanel()
        {
            if (_optionMenu == null) return;

            _optionMenu.gameObject.SetActive(!_optionMenu.gameObject.activeSelf);
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
            else if (_optionMenu.gameObject.activeSelf)
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