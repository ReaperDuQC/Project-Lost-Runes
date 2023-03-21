using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicColliderCapsule : DynamicCollider
    {
        [Header("Main Properties")]
        public Vector3 offset = Vector3.zero;
        public Vector3 direction = Vector3.forward;


        public float radius = 1;
        public float height = 2;

        public override Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            return ResolveCapsuleCollision(GetPosition(), radius, height, direction, point, pointRadius, out collisionDetected, this.mode);
        }

        public void AlignToParent()
        {
            if (transform.parent != null)
            {
                Vector3 childPos = transform.position;
                Vector3 parentPos = transform.parent.position;

                offset = transform.InverseTransformPoint((childPos + parentPos) / 2);
                direction = transform.InverseTransformDirection(childPos - parentPos);
                height = Vector3.Distance(childPos, parentPos);
            }
        }


        #region DebugDraw

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = GetPosition();
            Vector3 s1 = center + transform.TransformDirection(direction).normalized * height / 2;
            Vector3 s2 = center - transform.TransformDirection(direction).normalized * height / 2;
            DrawWireCapsule(s1, s2, radius, height, Color.yellow);
        }

        public static void DrawWireCapsule(Vector3 _pos, Vector3 _pos2, float _radius, float _height, Color _color)
        {
            //if (_color != default) Gizmos.color = _color;

            var forward = _pos2 - _pos;
            var _rot = Quaternion.LookRotation(forward);
            var pointOffset = _radius / 2f;
            var length = forward.magnitude;
            var center2 = new Vector3(0f, 0, length);

            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Gizmos.matrix.lossyScale);

            Gizmos.matrix = angleMatrix;
            {
                Gizmos.DrawWireSphere(Vector3.zero, _radius);
                Gizmos.DrawWireSphere(Vector3.forward * _height, _radius);

                DrawLine(_radius, 0f, length);
                DrawLine(-_radius, 0f, length);
                DrawLine(0f, _radius, length);
                DrawLine(0f, -_radius, length);
            }
        }

        private static void DrawLine(float arg1, float arg2, float forward)
        {
            Gizmos.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }

        public void Update()
        {

        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(offset);
        }
        #endregion
    }
}
