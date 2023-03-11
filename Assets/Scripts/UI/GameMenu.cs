using UnityEngine;

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
               // _playerControls.PlayerActions.Start.performed += i => CloseMenu();
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
            _activePanel = _menuPanel;
            _basePanel = _activePanel;
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