using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes
{
    public class CharacterShowRoom : MonoBehaviour
    {
        [SerializeField] List<Transform> _characters = new List<Transform>();
        Transform _currentCharacter = null;
        [SerializeField] int _currentIndex = 0;
        [SerializeField] Vector2 _scroll;

        PlayerControls _playerInput;

        Camera _camera;
        float _offset = -10f;

        // Start is called before the first frame update
        void Start()
        {
            _camera = Camera.main;

            foreach (Transform t in transform)
            {
                _characters.Add(t);
            }

            Show(_currentIndex);
        }

        private void OnEnable()
        {
            if(_playerInput == null)
            {
                _playerInput = new PlayerControls();
                _playerInput.CharacterShowRoom.Next.performed += i => ShowNext();
                _playerInput.CharacterShowRoom.Previous.performed += i => ShowPrevious();
            }
            _playerInput.Enable();
        }
        private void OnDisable()
        {
            _playerInput.Disable();
        }
        private void Update()
        {
            _scroll = _playerInput.CharacterShowRoom.Offset.ReadValue<Vector2>();
        }
        void ShowNext()
        {
            Hide(_currentIndex);
            _currentIndex = GetNextIndex(1);
            Show(_currentIndex);
        }

        void Show(int index)
        {
            _currentCharacter = _characters[index];
            _currentCharacter.gameObject.SetActive(true);
        }
        int GetNextIndex(int add)
        {
            int index = _currentIndex + add >= _characters.Count ? 0 : _currentIndex + add;
            index = _currentIndex + add <= 0 ? _characters.Count - 1 : _currentIndex + add;
            return index;
        }
        void Hide(int index)
        {
            _characters[index].gameObject.SetActive(false);
        }
        void ShowPrevious()
        {
            Hide(_currentIndex);
            _currentIndex = GetNextIndex(-1);
            Show(_currentIndex);
        }
        void SetCamerasPosition(Vector3 targetPosition)
        {
            targetPosition += _currentCharacter.forward * _offset;
            _camera.transform.position = targetPosition;
        }
    }
}