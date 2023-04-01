using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class RotateCharacterOnPreviewScreen : MonoBehaviour
    {
        PlayerControls _playerControls;
        [Header("Rotation")]
        [SerializeField] float _rotationAmount = 1f;
        [SerializeField] float _rotationSpeed = 5f;
        [Header("Zoom")]
        [SerializeField] float _zoomAmount = 1f;
        [SerializeField] float _zoomSpeed = 5f;
        float _currentZoom;

        [SerializeField] float _minZoomAmount = 1f;
        [SerializeField] float _maxZoomAmount = 10f;

        Vector2 _cameraInput;
        Vector2 _zoomInput;

        Vector3 _currentCameraPosition;
        Vector3 _targetCameraPosition;

        Vector3 _currentRotation;
        Vector3 _targetRotation;
        [SerializeField] Transform _target;
        [SerializeField] Transform _camera;
        private void OnEnable()
        {
            if(_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.CharacterCreator.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
                _playerControls.CharacterCreator.Zoom.performed += i => _zoomInput = i.ReadValue<Vector2>();
            }
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        private void Start()
        {
            _currentCameraPosition = _camera.position;
            _currentRotation = _target.eulerAngles;
        }
        private void Update()
        {
            Rotate();
            Zoom();
        }
        void Zoom()
        {
            if (_zoomInput.y > 0 || _zoomInput.x > 0)
            {
                _targetCameraPosition = _camera.position + _camera.forward * _zoomAmount;
            }
            else if (_zoomInput.y < 0 || _zoomInput.x < 0)
            {
                _targetCameraPosition = _camera.position + _camera.forward * _zoomAmount;
            }

            //_currentCameraPosition = Vector3.Lerp(_currentCameraPosition, _targetCameraPosition, _zoomSpeed * Time.deltaTime);
            //_camera.position = _currentCameraPosition;
        }
        void Rotate()
        {
            if (_cameraInput.x > 0)
            {
                _targetRotation.y += _rotationAmount;
            }
            else if (_cameraInput.x < 0)
            {
                _targetRotation.y -= _rotationAmount;
            }
            _currentRotation = Vector3.Lerp(_currentRotation, _targetRotation, _rotationSpeed * Time.deltaTime);
            _target.eulerAngles = _currentRotation;
        }
    }
}