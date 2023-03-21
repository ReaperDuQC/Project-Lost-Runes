using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicColliderSphere : DynamicCollider
    {
        public Vector3 offset = Vector3.zero;
        public Vector3 direction = Vector3.forward;

        public float radius = 1;

        public override Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            return ResolveSphereCollision(GetPosition(), this.radius * this.transform.localScale[0], point, pointRadius, out collisionDetected, this.mode);
        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(offset);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetPosition(), radius * transform.localScale[0]);
        }
    }
}