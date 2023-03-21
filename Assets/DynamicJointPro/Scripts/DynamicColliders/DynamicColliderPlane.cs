using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicColliderPlane : DynamicCollider
    {
        public Vector3 offset = Vector3.zero;
        public Vector3 normalVector = Vector3.forward;

        public override Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            return ResolvePlaneCollision(GetPosition(), transform.TransformDirection(normalVector), point, pointRadius, out collisionDetected, mode);
        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(offset);
        }

        #region DebugDraw
        public void OnDrawGizmosSelected()
        {
            Quaternion rotation = Quaternion.LookRotation(transform.TransformDirection(normalVector));
            Matrix4x4 trs = Matrix4x4.TRS(GetPosition(), rotation, Vector3.one);
            Gizmos.matrix = trs;
            Color32 color = Color.yellow;
            color.a = 125;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, new Vector3(1.0f, 1.0f, 0.0001f));
            Gizmos.DrawLine(Vector3.zero, Vector3.forward);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
        #endregion
    }
}