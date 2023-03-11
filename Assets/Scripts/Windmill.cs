using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class Windmill : MonoBehaviour
    {
        [SerializeField] Transform _transform;
        [SerializeField] Vector3 _rotation;
        [SerializeField] float _rotationSpeed = 2f;
        void Update()
        {
            if (_transform != null)
            {
                Vector3 rotation = _rotation * (_rotationSpeed * Time.deltaTime);
                _transform.Rotate(rotation);
            }
        }
    }
}