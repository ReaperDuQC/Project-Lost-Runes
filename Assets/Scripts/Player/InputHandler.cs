using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace RPG
{
    public class InputHandler : MonoBehaviour
    {
        public float _horizontal;
        public float _vertical;
        public float _moveAmount;
        public float _mouseX;
        public float _mouseY;

        public bool useConsumable_Input;
        public bool jump_Input;
        public bool roll_Input;
        public bool interact_Input;

        public bool rightLightAttack_Input;
        public bool rightStrongAttack_Input;
        public bool leftLightAttack_Input;
        public bool leftStrongAttack_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        public bool start_Input;
        public bool select_Input;

        public bool lockOn_Input;
        public bool rightStick_Left_Input;
        public bool rightStick_Right_Input;

        public bool _rollFlag;
        public bool _twoHandFlag;
        public bool _jumpFlag;
        public bool _interactFlag;
        public bool _lockOnFlag;
        public bool _sprintFlag;
        public bool _comboFlag;
        public float _rollInputTimer;
        public bool _inventoryFlag;

        PlayerControls _playerControls;

        Vector2 _movementInput;
        Vector2 _cameraInput;

        public void Initialize(bool isOwner)
        {
            if (!isOwner)
            {
                Destroy(this);
                return;
            }
        }

        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();

                _playerControls.PlayerMovement.Movement.performed += _inputActions => _movementInput = _inputActions.ReadValue<Vector2>();
                _playerControls.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();

                _playerControls.PlayerActions.Right_Light_Attack.performed += i => rightLightAttack_Input = true;
                _playerControls.PlayerActions.Right_Strong_Attack.performed += i => rightStrongAttack_Input = true;

                _playerControls.PlayerActions.Left_Light_Attack.performed += i => leftLightAttack_Input = true;
                _playerControls.PlayerActions.Left_Strong_Attack.performed += i => leftStrongAttack_Input = true;

                _playerControls.PlayerQuickSlots.DPad_Right.performed += i => d_Pad_Right = true;
                _playerControls.PlayerQuickSlots.DPad_Left.performed += i => d_Pad_Left = true;
                _playerControls.PlayerQuickSlots.DPad_Up.performed += i => d_Pad_Up = true;
                _playerControls.PlayerQuickSlots.DPad_Down.performed += i => d_Pad_Down = true;

                _playerControls.PlayerActions.Interact.performed += i => interact_Input = true;
                _playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                _playerControls.PlayerActions.Use_Consumable.performed += i => useConsumable_Input = true;

                _playerControls.PlayerActions.Start.performed += i => start_Input = true;
                _playerControls.PlayerActions.Select.performed += i => select_Input = true;

                _playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;

                _playerControls.PlayerMovement.LockOnTargetRight.performed += i => rightStick_Right_Input = true;
                _playerControls.PlayerMovement.LockOnTargetLeft.performed += i => rightStick_Left_Input = true;
            }
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            if (_playerControls != null)
            {
                _playerControls.Disable();
            }
        }
        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotInput();
            HandleInteractionInput();
            HandleJumpInput();
            HandleInventoryInput();
            HandleLockOnInput();
            HandleTwoHandInput();
        }
        private void HandleMoveInput(float delta)
        {
            _horizontal = _movementInput.x;
            _vertical = _movementInput.y;
            _moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));
            _mouseX = _cameraInput.x;
            _mouseY = _cameraInput.y;
        }
        private void HandleRollInput(float delta)
        {
            roll_Input = _playerControls.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            _sprintFlag = roll_Input;
            if (roll_Input)
            {
                _rollInputTimer += delta;
            }
            else
            {
                if (_rollInputTimer > 0f && _rollInputTimer < 0.5f)
                {
                    _sprintFlag = false;
                    _rollFlag = true;
                }
                _rollInputTimer = 0f;
            }
        }
        private void HandleAttackInput(float delta)
        {
            //if (rightLightAttack_Input)
            //{
            //    if (_playerManager._canDoCombo)
            //    {
            //        _comboFlag = true;
            //        _playerAttacker.HandleLightWeaponCombo(_playerInventory._rightWeapon);
            //        _comboFlag = false;
            //    }
            //    else
            //    {
            //        if (_playerManager._isInteracting) { return; }
            //        if (_playerManager._canDoCombo) { return; }
            //        _playerAttacker.HandleLightAttack(_playerInventory._rightWeapon);
            //    }
            //}
            //if (rightStrongAttack_Input)
            //{
            //    if (_playerManager._canDoCombo)
            //    {
            //        _comboFlag = true;
            //        _playerAttacker.HandleHeavyWeaponCombo(_playerInventory._rightWeapon);
            //        _comboFlag = false;
            //    }
            //    else
            //    {
            //        if (_playerManager._isInteracting) { return; }
            //        if (_playerManager._canDoCombo) { return; }
            //        _playerAttacker.HandleHeavyAttack(_playerInventory._rightWeapon);
            //    }
            //}
        }
        private void HandleQuickSlotInput()
        {
            //if (d_Pad_Right)
            //{
            //    _playerInventory.ChangeRightWeapon();
            //}
            //else if (d_Pad_Left)
            //{
            //    _playerInventory.ChangeLeftWeapon();
            //}
        }
        private void HandleInteractionInput()
        {
            _interactFlag = interact_Input;
        }
        private void HandleJumpInput()
        {
            _jumpFlag = jump_Input;
        }
        private void HandleInventoryInput()
        {
            //if (start_Input)
            //{
            //    _inventoryFlag = !_inventoryFlag;

            //    if (_inventoryFlag)
            //    {
            //        _UIManager.OpenSelectWindow();
            //        _UIManager.UpdateUI();
            //        _UIManager._hudWindow.SetActive(false);

            //    }
            //    else
            //    {
            //        _UIManager.CloseSelectWindow();
            //        _UIManager.CloseAllInventoryWindow();
            //        _UIManager._hudWindow.SetActive(true);
            //    }
            //}
        }
        private void HandleLockOnInput()
        {
            //if (lockOn_Input && !_lockOnFlag)
            //{
            //    lockOn_Input = false;
            //    _cameraHandler.HandleLockOn();

            //    if (_cameraHandler._neareastLockOnTarget != null)
            //    {
            //        _cameraHandler._currentLockOnTarget = _cameraHandler._neareastLockOnTarget;
            //        _lockOnFlag = true;
            //    }
            //}
            //else if (lockOn_Input && _lockOnFlag)
            //{
            //    _lockOnFlag = false;
            //    lockOn_Input = false;
            //    _cameraHandler.ClearLockOnTargets();
            //}

            //if (_lockOnFlag)
            //{
            //    if (rightStick_Left_Input)
            //    {
            //        rightStick_Left_Input = false;
            //        _cameraHandler.HandleLockOn();
            //        if (_cameraHandler._leftLockOnTarget != null)
            //        {
            //            _cameraHandler._currentLockOnTarget = _cameraHandler._leftLockOnTarget;
            //        }
            //    }
            //    else if (rightStick_Right_Input)
            //    {
            //        rightStick_Right_Input = false;
            //        _cameraHandler.HandleLockOn();
            //        if (_cameraHandler._rightLockOnTarget != null)
            //        {
            //            _cameraHandler._currentLockOnTarget = _cameraHandler._rightLockOnTarget;
            //        }
            //    }
            //}
            //_cameraHandler.SetCameraHeight();
        }
        private void HandleTwoHandInput()
        {
            //if (interact_Input)
            //{
            //    _twoHandFlag = !_twoHandFlag;

            //    if (_twoHandFlag)
            //    {
            //        _weaponSlotManager.LoadWeaponOnSlot(_playerInventory._rightWeapon, false);
            //    }
            //    else
            //    {
            //        _weaponSlotManager.LoadWeaponOnSlot(_playerInventory._rightWeapon, false);
            //        _weaponSlotManager.LoadWeaponOnSlot(_playerInventory._leftWeapon, true);
            //    }
            //}
        }
        public void ResetInputFlag()
        {
            jump_Input = false;
            useConsumable_Input = false;
            interact_Input = false;

            rightLightAttack_Input = false;
            rightStrongAttack_Input = false;
            leftLightAttack_Input = false;
            leftStrongAttack_Input = false;

            d_Pad_Up = false;
            d_Pad_Down = false;
            d_Pad_Left = false;
            d_Pad_Right = false;

            start_Input = false;
            select_Input = false;

            _rollFlag = false;
            _jumpFlag = false;
            _interactFlag = false;
            _comboFlag = false;
        }
    }
}