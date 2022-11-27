using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler _inputHandler;
        PlayerManager _playerManager;

        public Transform _targetTransform;
        public Transform _cameraTransform;
        public Transform _cameraPivotTransform;
        private Transform _transform;
        private Vector3 _cameraTransformPosition;
        public LayerMask _ignoreLayer;
        public LayerMask _environmentLayer;
        private Vector3 _cameraFollowVelocity = Vector3.zero;

        public static CameraHandler _singleton;

        public float _lookSpeed = 0.1f;
        public float _followSpeed = 0.1f;
        public float _pivotSpeed = 0.3f;

        private float _targetPosition;
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;
        private float _minimumPivot = -35f;
        private float _maximumPivot = 35f;

        public float _cameraSphereRadius = 0.2f;
        public float _cameraCollisionOffset = 0.2f;
        public float _minimumCollisionOffset = 0.2f;

        public float _lockedPivotPosition = 2.25f;
        public float _unlockedPivotPosition = 1.65f;
        public CharacterManager _currentLockOnTarget;

        List<CharacterManager> _availableTargets = new List<CharacterManager>();
        public CharacterManager _neareastLockOnTarget;
        public CharacterManager _leftLockOnTarget;
        public CharacterManager _rightLockOnTarget;
        public float _maximumDistanceLockOn = 30;

        public void Initialize(PlayerManager manager, InputHandler inputHandler )
        {
            _singleton = this;

            _transform = transform;
            _defaultPosition = _cameraTransform.localPosition.z;
            _ignoreLayer = ~(1 << 8 | 1 << 9 | 1 << 10);
            _playerManager = manager;

            _targetTransform = _playerManager.transform;
            _inputHandler = inputHandler;

            _environmentLayer = LayerMask.NameToLayer("Environment");
        }
        public void FollowTarget(float delta)
        {
            Vector3 targetposition = Vector3.SmoothDamp(_transform.position, _targetTransform.position, ref _cameraFollowVelocity, delta / _followSpeed);
            _transform.position = targetposition;

            HandleCameraCollision(delta);
        }
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (!_inputHandler._lockOnFlag && _currentLockOnTarget == null)
            {
                _lookAngle += (mouseXInput * _lookSpeed) * delta;
                _pivotAngle -= (mouseYInput * _pivotSpeed) * delta;
                _pivotAngle = Mathf.Clamp(_pivotAngle, _minimumPivot, _maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = _lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                _transform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = _pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                _cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                //float velocity = 0;

                Vector3 direction = _currentLockOnTarget.transform.position - transform.position;
                direction.Normalize();
                direction.y = 0f;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;

                direction = _currentLockOnTarget.transform.position - _cameraPivotTransform.position;
                direction.Normalize();

                targetRotation = Quaternion.LookRotation(direction);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                _cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }
        private void HandleCameraCollision(float delta)
        {
            _targetPosition = _defaultPosition;
            RaycastHit hit;
            Vector3 direction = _cameraTransform.position - _cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(_cameraPivotTransform.position, _cameraSphereRadius, direction, out hit, Mathf.Abs(_targetPosition), _ignoreLayer))
            {
                float dis = Vector3.Distance(_cameraPivotTransform.position, hit.point);
                _targetPosition = -(dis - _cameraCollisionOffset);
            }

            if (Mathf.Abs(_targetPosition) < _minimumCollisionOffset)
            {
                _targetPosition = -_minimumCollisionOffset;
            }

            _cameraTransformPosition.z = Mathf.Lerp(_cameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
            _cameraTransform.localPosition = _cameraTransformPosition;
        }
        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(_targetTransform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - _targetTransform.position;

                    float distanceFromTarget = Vector3.Distance(_targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, _cameraTransform.forward);

                    RaycastHit hit;

                    if (character.transform.root != _targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= _maximumDistanceLockOn)
                    {
                        if (Physics.Linecast(_playerManager._lockOnTransform.position, character._lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(_playerManager._lockOnTransform.position, character._lockOnTransform.position);

                            if (hit.transform.gameObject.layer == _environmentLayer)
                            {
                                // cannot Lock onto target, object in the way
                            }
                            else
                            {
                                _availableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < _availableTargets.Count; k++)
            {
                float distanceFromTarget = Vector3.Distance(_targetTransform.position, _availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    _neareastLockOnTarget = _availableTargets[k];
                }

                if (_inputHandler._lockOnFlag)
                {
                    //Vector3 relativeEnemyPosition = _currentLockOnTarget.transform.InverseTransformPoint(_availableTargets[k].transform.position);
                    //float distanceFromLeftTarget = _currentLockOnTarget.transform.position.x - _availableTargets[k].transform.position.x;
                    //float distanceFromRightTarget = _currentLockOnTarget.transform.position.x + _availableTargets[k].transform.position.x;

                    Vector3 relativeEnemyPosition = _inputHandler.transform.InverseTransformPoint(_availableTargets[k].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.x;

                    if (relativeEnemyPosition.x <= 0f && distanceFromLeftTarget > shortestDistanceOfLeftTarget && _availableTargets[k] != _currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        _leftLockOnTarget = _availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x >= 0f && distanceFromRightTarget < shortestDistanceOfRightTarget && _availableTargets[k] != _currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        _rightLockOnTarget = _availableTargets[k];
                    }
                }
            }
        }
        public void ClearLockOnTargets()
        {
            _availableTargets.Clear();
            _neareastLockOnTarget = null;
            _currentLockOnTarget = null;
        }
        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0f, _lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0f, _unlockedPivotPosition);

            if (_currentLockOnTarget != null)
            {
                _cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(_cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                _cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(_cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}
