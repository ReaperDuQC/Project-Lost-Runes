using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes
{
    public class UICursor : MonoBehaviour
    {
        // need to change position depending of input control, either mouse position or button control
        [SerializeField] GameObject _cursorGraphic;
        [SerializeField] RectTransform _transform;
        [SerializeField] Vector2 _cursorPos;
        [SerializeField] Vector2 _offsetPos;

        [SerializeField] bool _isMouse;
        [SerializeField] bool _cursorVisible;


        private void Awake()
        {
            Cursor.visible = false;
            _isMouse = true;
        }
        void Start()
        {
            ShowCursor(_cursorVisible);
        }

        // Update is called once per frame
        void Update()
        {
            FollowMouse();
        }
        void FollowMouse()
        {
            if (!_cursorVisible) return;
            if (!_isMouse) return;

            _cursorPos = Mouse.current.position.ReadValue();
            _transform.position = _cursorPos + _offsetPos;
        }
        public void ShowCursor(bool show)
        {
            _cursorVisible = show;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            _cursorGraphic.gameObject.SetActive(_cursorVisible);
        }
    }
}