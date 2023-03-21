using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicColliderCone : DynamicCollider
    {
        //public float radius = 1;
        [Header("Main Properties")]
        public Vector3 offset = Vector3.zero;
        public Vector3 direction = Vector3.forward;
        public float maxDistance = 1f;

        public float angle = 45f;
        public Vector3 forwardDirection;
        public Vector3 upDirection;
        public override Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            return ResolveConeCollision(GetPosition(), direction, angle, maxDistance, point, pointRadius, out collisionDetected, this.mode);
        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(offset);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
        }

    }
}