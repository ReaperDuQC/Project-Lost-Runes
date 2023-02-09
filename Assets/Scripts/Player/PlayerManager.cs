using LostRunes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerManager : CharacterManager
    {
        InputHandler _inputHandler;
        CameraHandler _cameraHandler;
        PlayerLocomotion _playerLocomotion;
        Animator _animator;
        CharacterStats _characterStats;
        
        [Header("Interacting")]
        InteractableUI _interactableUI;
        IInteractable _interactableObject;
        [SerializeField] float _interactingDistance =1f;

       [Header("Player Flags")]
        public bool _isSprinting;
        public bool _isInAir;
        public bool _isGrounded;
        public bool _isInteracting;
        public bool _canDoCombo;
        private void Awake()
        {
            _interactableUI = FindObjectOfType<InteractableUI>();
        }
        public void Initialize(bool isOwner,CameraHandler cameraHandler, InputHandler inputHandler, CharacterStats stats, PlayerLocomotion locomotion, Animator animator)
        {
            if(!isOwner)
            {
                Destroy(this);
                return;
            }

            _cameraHandler = cameraHandler;
            _inputHandler = inputHandler;
            _characterStats = stats;
            _playerLocomotion = locomotion;
            _animator = animator;

            //LoadPlayerStats();
        }
        private void Update()
        {
            float delta = Time.deltaTime;
            _isInteracting = _animator.GetBool("IsInteracting");
            _canDoCombo = _animator.GetBool("CanDoCombo");
            _animator.SetBool("IsInAir", _isInAir);

            _inputHandler.TickInput(delta);
            _playerLocomotion.HandleRollingAndSprinting(delta);
            _playerLocomotion.HandleJumping();

            CheckForInteractableObject();
        }
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            _playerLocomotion.HandleMovement(delta);
            _playerLocomotion.HandleFalling(delta, _playerLocomotion._moveDirection);
        }
        private void LateUpdate()
        {
            _inputHandler.ResetInputFlag();

            float delta = Time.deltaTime;
            if (_cameraHandler != null)
            {
                _cameraHandler.FollowTarget(delta);
                _cameraHandler.HandleCameraRotation(delta, _inputHandler._mouseX, _inputHandler._mouseY);
            }

            if (_isInAir)
            {
                _playerLocomotion._inAirTimer = _playerLocomotion._inAirTimer + Time.deltaTime;
            }
        }
        public void CheckForInteractableObject()
        {
            if (_interactableUI == null) return;

            RaycastHit hit;

            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, _interactingDistance, _cameraHandler._ignoreLayer))
            {
                if (hit.collider.tag == "Interactable")
                {
                    _interactableObject = hit.collider.GetComponent<IInteractable>();

                    if (_interactableObject != null)
                    {
                        string interactableText = _interactableObject.GetInteractText();
                        _interactableUI.SetInteractText(interactableText);
                        _interactableUI.ShowText(true);

                        if (_inputHandler._interactFlag)
                        {
                            _interactableObject.Interact();
                        }
                    }
                }
            }
            else
            {
                _interactableUI.ShowText(false);
            }
        }
    }
}