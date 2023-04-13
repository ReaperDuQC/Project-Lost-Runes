using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;

        PlayerControls _playerControls;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            
        }
        private void OnEnable()
        {
            if(_playerControls == null)
            {

            }
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            _playerControls.Disable();
        }
        private void OnDestroy()
        {
            
        }
        private void Update()
        {
            
        }
    }
}