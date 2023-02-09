using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostRunes
{
    public class UICursor : MonoBehaviour
    {
        // need to change position depending of input control, either mouse position or button control
        [SerializeField] Transform _cursor;
        [SerializeField] Vector2 _cursorPos;

        [SerializeField] bool _isMouse;

        Camera _camera;

        void Start()
        {
            //Cursor.visible= false;
            _isMouse = true; 
            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            FollowMouse();
        }
        void FollowMouse()
        {
            if (!_isMouse) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            _cursorPos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.transform.position.z));
            _cursor.position = _cursorPos;
        }
    }
}