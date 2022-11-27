using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Windmill : MonoBehaviour
    {
        Transform _transform;
        [SerializeField] Vector3 _rotation;
        [SerializeField] float _rotationSpeed = 2f;
        private void Awake()
        {
            _transform = transform;
        }
        void Update()
        {
            Vector3 rotation = _rotation * (_rotationSpeed * Time.deltaTime);
            _transform.Rotate(rotation);
        }
    }
}