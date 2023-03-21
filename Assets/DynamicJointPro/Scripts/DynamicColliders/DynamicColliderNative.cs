using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    [RequireComponent(typeof(Collider))]
    public class DynamicColliderNative : DynamicCollider
    {
        public Collider nativeCollider;
        private DynamicColliderShape colliderShapeType = DynamicColliderShape.NONE;
        private float height;
        private float radius;
        private Vector3 pos;
        private Vector3 direction;

        public override Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            switch (colliderShapeType)
            {
                case DynamicColliderShape.SPHERE:
                    return ResolveSphereCollision(pos, radius, point, pointRadius, out collisionDetected, this.mode);
                case DynamicColliderShape.CAPSULE:
                    return ResolveCapsuleCollision(pos, radius, height, direction, point, pointRadius, out collisionDetected, this.mode);
                default:
                    return ResolveNativeCollision(nativeCollider, point, pointRadius, out collisionDetected, this.mode);
            }
        }

        public void Start()
        {
            if (this.nativeCollider == null)
                this.nativeCollider = GetComponent<Collider>();

            if (nativeCollider is SphereCollider)
            {
                colliderShapeType = DynamicColliderShape.SPHERE;
            }
            else if (nativeCollider is CapsuleCollider)
            {
                colliderShapeType = DynamicColliderShape.CAPSULE;
            }
            else
            {
                colliderShapeType = DynamicColliderShape.NATIVE;
            }
        }

        public override void UpdateCollider()
        {
            if (!Application.isPlaying)
                return;

            switch (colliderShapeType)
            {
                case DynamicColliderShape.CAPSULE:
                    var cCol = nativeCollider as CapsuleCollider;
                    this.direction = nativeCollider.transform.forward;
                    if (cCol.direction == 0)
                    {
                        direction = nativeCollider.transform.right;
                    }
                    if (cCol.direction == 1)
                    {
                        direction = cCol.transform.up;
                    }
                    this.pos = cCol.gameObject.transform.TransformPoint(cCol.center);
                    this.radius = cCol.radius * cCol.transform.localScale[0];
                    this.height = cCol.height * (cCol.transform.localScale[0] / 2);
                    break;
                case DynamicColliderShape.SPHERE:
                    var sCol = nativeCollider as SphereCollider;
                    this.pos = nativeCollider.gameObject.transform.TransformPoint(sCol.center);
                    this.radius = sCol.radius * sCol.transform.localScale[0];
                    break;
            }
        }

    }
}
