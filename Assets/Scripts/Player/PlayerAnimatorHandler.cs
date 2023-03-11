using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerAnimatorHandler : AnimatorHandler
    {
        PlayerManager _playerManager;
        InputHandler _inputHandler;
        PlayerLocomotion _playerLocomotion;
        int _vertical;
        int _horizontal;
        public bool _canRotate;

        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            _animator = GetComponent<Animator>();
            _inputHandler = GetComponentInParent<InputHandler>();
            _playerManager = GetComponentInParent<PlayerManager>();
            _playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            _vertical = Animator.StringToHash("Vertical");
            _horizontal = Animator.StringToHash("Horizontal");
        }
        public void UpdateAnimatorValue(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0f;
            if (verticalMovement > 0f && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1f;
            }
            else if (verticalMovement < 0f && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1f;
            }
            else
            {
                v = 0f;
            }
            #endregion

            #region Horizontal
            float h = 0f;
            if (horizontalMovement > 0f && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0f && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1f;
            }
            else
            {
                h = 0f;
            }
            #endregion

            if (isSprinting)
            {
                v = 2f;
                h = horizontalMovement;
            }

            _animator.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            _animator.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
        }
        public void CanRotate()
        {
            _canRotate = true;
        }
        public void StopRotation()
        {
            _canRotate = false;
        }
        public void EnableCombo()
        {
            _animator.SetBool("CanDoCombo", true);
        }
        public void DisableCombo()
        {
            _animator.SetBool("CanDoCombo", false);
        }
        private void OnAnimatorMove()
        {
            if (_playerManager._isInteracting == false) { return; }

            _animator.applyRootMotion = true;

            float delta = Time.deltaTime;
            _playerLocomotion._rigidbody.drag = 0f;
            Vector3 deltaPosition = _animator.deltaPosition;
            deltaPosition.y = 0f;
            Vector3 velocity = deltaPosition / delta;
            _playerLocomotion._rigidbody.velocity = velocity;
        }
    }
}