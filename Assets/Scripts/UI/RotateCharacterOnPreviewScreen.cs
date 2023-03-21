using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class RotateCharacterOnPreviewScreen : MonoBehaviour
    {
        PlayerControls _playerControls;
        [SerializeField] float _rotationAmount = 1f;
        [SerializeField] float _rotationSpeed = 5f;

        Vector2 _cameraInput;

        Vector3 _currentRotation;
        Vector3 _targetRotation;
        [SerializeField] Transform _transform;
        private void OnEnable()
        {
            if(_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.CharacterCreator.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
            }
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        private void Start()
        {
            _currentRotation = _transform.eulerAngles;
        }
        private void Update()
        {
            if(_cameraInput.x > 0)
            {
                _targetRotation.y = _targetRotation.y + _rotationAmount;
            }
            else if(_cameraInput.x < 0)
            {
                _targetRotation.y = _targetRotation.y - _rotationAmount;
            }
            _currentRotation = Vector3.Lerp(_currentRotation, _targetRotation, _rotationSpeed * Time.deltaTime);
            _transform.eulerAngles = _currentRotation;
        }
    }
}