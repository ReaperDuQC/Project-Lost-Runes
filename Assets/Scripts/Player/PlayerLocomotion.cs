using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerLocomotion : MonoBehaviour
    {
        CameraHandler _cameraHandler;
        Transform _cameraObject;
        InputHandler _inputHandler;
        PlayerManager _playerManager;
        public Vector3 _moveDirection;

        [HideInInspector]
        public Transform _transform;
        [HideInInspector]
        public PlayerAnimatorHandler _animatorHandler;

        public Rigidbody _rigidbody;
        public GameObject _normalCamera;
        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float _groundDetectionRayStartPoint = 0.5f;
        [SerializeField]
        float _minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float _groundDirectionRayDistance = 0.2f;
        LayerMask _ignoreForGroundCheck;
        public float _inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        float _movementSpeed = 5f;
        [SerializeField]
        float _walkingSpeed = 5f;
        [SerializeField]
        float _sprintSpeed = 7f;
        [SerializeField]
        float _rotationSpeed = 10f;
        [SerializeField]
        float _fallingSpeed = 400f;

        public void Initialize(bool isOwner, CameraHandler cameraHandler, InputHandler inputHandler, PlayerManager manager)
        {
            if(!isOwner)
            {
                Destroy(this);
                return;
            }

            _cameraHandler = cameraHandler;
            _inputHandler = inputHandler;
            _playerManager = manager;

            _animatorHandler = GetComponentInChildren<PlayerAnimatorHandler>();
            _rigidbody = GetComponent<Rigidbody>();

            _cameraObject = Camera.main.transform;
            _transform = transform;

            _animatorHandler.Initialize();

            _playerManager._isGrounded = true;
            _ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        #region Movement
        Vector3 _normalVector;
        Vector3 _targetPosition;

        private void HandleRotation(float delta)
        {
            if (_inputHandler._lockOnFlag)
            {
                if (_inputHandler._sprintFlag || _inputHandler._rollFlag)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = _cameraHandler._cameraTransform.forward * _inputHandler._vertical;
                    targetDirection += _cameraHandler._cameraTransform.right * _inputHandler._horizontal;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = _transform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targetDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, _rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
                else
                {
                    Vector3 rotationDirection = _moveDirection;
                    rotationDirection = _cameraHandler._currentLockOnTarget.transform.position - transform.position;
                    rotationDirection.Normalize();
                    rotationDirection.y = 0;
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, _rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            else
            {

                Vector3 targetDirection = Vector3.zero;
                float moveOverride = _inputHandler._moveAmount;

                targetDirection = _cameraObject.forward * _inputHandler._vertical;
                targetDirection += _cameraObject.right * _inputHandler._horizontal;

                targetDirection.Normalize();
                targetDirection.y = 0f;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = _transform.forward;
                }

                float rs = _rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(_transform.rotation, tr, rs * delta);

                _transform.rotation = targetRotation;
            }
        }
        public void HandleMovement(float delta)
        {
            if(_inputHandler._rollFlag) { return; }
            if(_playerManager._isInteracting) { return; }

            _moveDirection = _cameraObject.forward * _inputHandler._vertical;
            _moveDirection += _cameraObject.right * _inputHandler._horizontal;
            _moveDirection.Normalize();
            _moveDirection.y = 0f;

            float speed = _movementSpeed;

            if(_inputHandler._sprintFlag && _inputHandler._moveAmount > 0.5f)
            {
                speed = _sprintSpeed;
                _playerManager._isSprinting = true;
            }
            else
            {
                if(_inputHandler._moveAmount < 0.5f)
                {
                    _moveDirection *= _walkingSpeed;
                }
                _playerManager._isSprinting = false;
            }
            _moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            _rigidbody.velocity = projectedVelocity;

            if(_inputHandler._lockOnFlag && !_inputHandler._sprintFlag)
            {
                _animatorHandler.UpdateAnimatorValue(_inputHandler._vertical, _inputHandler._horizontal, _playerManager._isSprinting);
            }
            else
            {
                _animatorHandler.UpdateAnimatorValue(_inputHandler._moveAmount, 0, _playerManager._isSprinting);
            }

            if (_animatorHandler._canRotate)
            {
                HandleRotation(delta);
            }
        }
        public void HandleRollingAndSprinting(float delta)
        { 
            if(_animatorHandler._animator.GetBool("IsInteracting")) { return; }

            if(_inputHandler._rollFlag)
            {
                _moveDirection = _cameraObject.forward * _inputHandler._vertical;
                _moveDirection += _cameraObject.right * _inputHandler._horizontal;

                if(_inputHandler._moveAmount > 0)
                {
                    _animatorHandler.PlayTargetAnimation("Rolling", true);
                    _moveDirection.y = 0f;
                    Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                    _transform.rotation = rollRotation;
                }
                else
                {
                    _animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }
        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            _playerManager._isGrounded = false;
            RaycastHit hit;
            Vector3 origin = _transform.position;
            origin.y += _groundDetectionRayStartPoint;

            if(Physics.Raycast(origin, _transform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if(_playerManager._isInAir)
            {
                _rigidbody.AddForce(-Vector3.up * _fallingSpeed);
                _rigidbody.AddForce(moveDirection * _fallingSpeed / 10f);
            }

            Vector3 direction = moveDirection;
            direction.Normalize();
            origin = origin + direction * _groundDirectionRayDistance;

            _targetPosition = _transform.position;

            Debug.DrawRay(origin, -Vector3.up * _minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            if(Physics.Raycast(origin, -Vector3.up, out hit, _minimumDistanceNeededToBeginFall, _ignoreForGroundCheck))
            {
                _normalVector = hit.normal;
                Vector3 tmp = hit.point;
                _playerManager._isGrounded = true;
                _targetPosition.y = tmp.y;

                if(_playerManager._isInAir)
                {
                    if(_inAirTimer > 0.5f)
                    {
                        Debug.Log("you were in _inAirTimer for " + _inAirTimer);
                        _animatorHandler.PlayTargetAnimation("Landing", true);
                    }
                    else
                    {
                        _animatorHandler.PlayTargetAnimation("Empty", false);
                    }

                    _inAirTimer = 0f;
                    _playerManager._isInAir = false;
                }
            }
            else
            {
                if(_playerManager._isGrounded)
                {
                    _playerManager._isGrounded = false;
                }

                if(!_playerManager._isInAir)
                {
                    if(_playerManager._isInteracting == false)
                    {
                        _animatorHandler.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 velocity = _rigidbody.velocity;
                    velocity.Normalize();
                    _rigidbody.velocity = velocity * (_movementSpeed / 2);
                    _playerManager._isInAir = true;

                }
            }

            if (_playerManager._isInteracting || _inputHandler._moveAmount > 0)
            {
                _transform.position = Vector3.Lerp(_transform.position, _targetPosition, Time.deltaTime / 0.1f);
            }
            else 
            {
                _transform.position = _targetPosition;
            }
        }
        public void HandleJumping()
        {
            if(_playerManager._isInteracting) { return; }

            if(_inputHandler._jumpFlag)
            {
                _moveDirection = _cameraObject.forward * _inputHandler._vertical;
                _moveDirection += _cameraObject.right * _inputHandler._horizontal;

                if (_inputHandler._moveAmount > 0f)
                {
                    _moveDirection = _cameraObject.forward * _inputHandler._vertical;
                    _moveDirection += _cameraObject.right * _inputHandler._horizontal;

                    _animatorHandler.PlayTargetAnimation("JumpWhileRunning", true);

                    Quaternion jumpRotation = Quaternion.LookRotation(_moveDirection);
                    _transform.rotation = jumpRotation;
                }
                else
                {
                    _animatorHandler.PlayTargetAnimation("Jump", true);
                }

                _moveDirection.y = 0f;
            }
        }
        #endregion
    }
}