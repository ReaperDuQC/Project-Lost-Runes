using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager _playerManager;

        [SerializeField] float _verticalMovement;
        [SerializeField] float _horizontalMovement;
        [SerializeField] float _moveAmount;

        Vector3 _moveDirection;

        protected override void Awake()
        {
            base.Awake();

            _playerManager = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {

        }
        void HandleGroundedMovement()
        {
            _moveDirection = PlayerCamera.Instance.transform.forward * _verticalMovement;
            _moveDirection = _moveDirection + PlayerCamera.Instance.transform.right * _horizontalMovement;
            _moveDirection.Normalize();
            _moveDirection.y = 0f;

            if(_playerManager)
            {

            }
        }
    }
}