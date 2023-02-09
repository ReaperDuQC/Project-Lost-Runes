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
        [SerializeField] GameObject _menuPanel;

        [Header("Equipment")]
        [SerializeField] GameObject _equipmentPanel;

        [Header("Inventory")]
        [SerializeField] GameObject _inventoryPanel;

        [Header("Quit")]
        [SerializeField] GameObject _quitPanel;

        PlayerControls _playerControls;

        private void Start()
        {
            Initialize();
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
        public override void Initialize()
        {
            base.Initialize();
        }
        public void ToggleMenuPanel()
        {
            if (_menuPanel == null) return;

            _menuPanel.SetActive(!_menuPanel.activeSelf);
        }
        public void ToggleEquipmentPanel()
        {
            if (_equipmentPanel == null) return;

            _equipmentPanel.SetActive(!_equipmentPanel.activeSelf);
        }
        public void ToggleInventoryPanel()
        {
            if (_inventoryPanel == null) return;

            _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
        }
        public void ToggleQuitPanel()
        {
            if (_quitPanel == null) return;

            _quitPanel.SetActive(!_quitPanel.activeSelf);
        }
        private void CloseMenu()
        {
            if (_quitPanel.activeSelf)
            {
                ToggleQuitPanel();
                ToggleMenuPanel();
            }
            else if (_inventoryPanel.activeSelf)
            {
                ToggleInventoryPanel();
                ToggleMenuPanel();
            }
            else if (_equipmentPanel.activeSelf)
            {
                ToggleEquipmentPanel();
                ToggleMenuPanel();
            }
            else if (OptionMenuUI.gameObject.activeSelf)
            {
                ToggleOptionPanel();
                ToggleMenuPanel();
            }
            else if (_menuPanel.activeSelf)
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
            //GameManager instance = GameManager.Instance;
            //if (instance != null)
            //{
            //    instance.HideCursor(visible);
            //}
        }
    }
}