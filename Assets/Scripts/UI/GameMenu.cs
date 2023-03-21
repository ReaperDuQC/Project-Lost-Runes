using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes
{
    public class GameMenu : RPGMenu
    {
        [Header("Menu")]
        [SerializeField] CustomMenuPanel _menuPanel;

        [Header("Equipment")]
        [SerializeField] CustomMenuPanel _equipmentPanel;

        [Header("Inventory")]
        [SerializeField] CustomMenuPanel _inventoryPanel;

        [Header("Quit")]
        [SerializeField] CustomMenuPanel _quitPanel;
        [Header("Cursor")]
        [SerializeField] UICursor _cursor;
        [SerializeField] GameObject _playerUI;

        PlayerControls _playerControls;
        bool _interactable = false;

        private void Start()
        {
            Initialize();
        }
        private void OnEnable()
        {
            if(_playerControls == null)
            {
                _playerControls = new PlayerControls();
               _playerControls.PlayerActions.Start.performed += CloseMenu;
            }
            _playerControls.Enable();
        }

        private void CloseMenu(InputAction.CallbackContext context)
        {
            if (context.performed && _interactable)
            {
                bool show = true;
                if (_activePanel == _menuPanel)
                {
                    show = false;
                    SetMenuPanelActive(false);
                    _activePanel = null;
                }
                else if(_activePanel == null) 
                {
                    SetMenuPanelActive(true);
                }
                else
                {
                    SetPanelActive(_activePanel, false);
                }
                _playerUI.SetActive(_activePanel == null);
                _cursor.ShowCursor(show);
            }
        }
        public void EnableMenuInteraction(bool interactable)
        {
            _interactable = interactable;
            _cursor.ShowCursor(false);
            _playerUI.SetActive(true);
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        public override void Initialize()
        {
            base.Initialize();
            _activePanel = null;
            _basePanel = _menuPanel;
        }
        public void SetMenuPanelActive(bool active)
        {
            SetPanelActive(_menuPanel, active);
        }
        public void SetEquipmentPanelActive(bool active)
        {
            SetPanelActive(_equipmentPanel, active);
        }
        public void SetInventoryPanelActive(bool active)
        {
            SetPanelActive(_inventoryPanel, active);
        }
        public void SetQuitPanelActive(bool active)
        {
            SetPanelActive(_quitPanel, active);
        }
    }
}