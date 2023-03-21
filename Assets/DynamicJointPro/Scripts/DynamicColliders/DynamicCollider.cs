using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public enum DynamicColliderShape
    {
        SPHERE,
        PLANE,
        CAPSULE,
        CONE,
        HINGE,
        NATIVE,
        NONE
    }

    public enum DynamicColliderProjectionMode
    {
        OUT_OF,
        INTO,
        ONTO
    }

    public abstract class DynamicCollider : MonoBehaviour
    {
        [HideInInspector]
        public DynamicColliderShape shapeType;
        [HideInInspector]
        public float mainDimension = 1;
        [HideInInspector]
        public float secondaryDimension = 1;


        public DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF;

        [HideInInspector]
        public float bounce = 0;
        public float friction = 0;

        [HideInInspector]
        public int particleID = -1;

        //linePnt - point the line passes through
        //lineDir - unit vector in direction of line, either direction works
        //pnt - the point to find nearest on line for
        public static Vector3 FindClosestPointOnLine(Vector3 linePoint, Vector3 lineDir, Vector3 pt)
        {
            lineDir.Normalize();//this needs to be a unit vector
            var v = pt - linePoint;
            var d = Vector3.Dot(v, lineDir);
            return linePoint + lineDir * d;
        }

        public static Vector3 ResolveSphereCollision(Vector3 spherePosition, float sphereRadius, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF)
        {
            collisionDetected = false;
            Vector3 toSphere = point - spherePosition;
            float distance = Vector3.Distance(spherePosition, point);
            float error = (sphereRadius + pointRadius) - distance;
            var projection = (point + toSphere.normalized * (error));

            switch (mode)
            {
                case DynamicColliderProjectionMode.INTO:
                    if (error < 0)
                    {
                        collisionDetected = true;
                        return projection;
                    }
                    break;
                case DynamicColliderProjectionMode.ONTO:
                    collisionDetected = true;
                    return projection;
                default:
                    if (error > 0)
                    {
                        collisionDetected = true;
                        return (point + toSphere.normalized * (error));
                    }
                    break;
            }

            return point;
        }

        public static Vector3 ResolveNativeCollision(Collider collider, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF)
        {
            collisionDetected = false;

            var closestPoint = collider.ClosestPoint(point);
            bool isInside = closestPoint == point;
            float d = Vector3.Distance(point, closestPoint);
            Vector3 projection = point;
            var norm = ((point - closestPoint).normalized * pointRadius);

            switch (mode)
            {
                case DynamicColliderProjectionMode.INTO:
                    if (!isInside)
                    {
                        norm *= -1;
                        projection = closestPoint + norm;
                        collisionDetected = true;
                    }
                    break;
                case DynamicColliderProjectionMode.ONTO:
                    if (isInside)
                        norm *= -1;
                    projection = closestPoint + norm;
                    collisionDetected = true;
                    break;
                default:
                    if (isInside || d < pointRadius)
                    {
                        if (isInside)
                            norm *= -1;
                        projection = closestPoint + norm;
                        collisionDetected = true;
                    }
                    break;
            }

            return projection;
        }

        public virtual void UpdateCollider()
        {
        }

        public virtual Vector3 ResolveCollision(Vector3 point, float pointRadius, out bool collisionDetected)
        {
            collisionDetected = false;
            return point;
        }

        public static Vector3 GetPointOnPlaneProjection(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
        {
            //q_proj = q - dot(q - p, n) * n
            Vector3 normalizedPlaneNormal = planeNormal.normalized;
            Vector3 pointProjection = point - Vector3.Dot(point - planePoint, normalizedPlaneNormal) * normalizedPlaneNormal;
            return pointProjection;
        }

        public static Vector3 ResolvePlaneCollision(Vector3 planePosition, Vector3 planeNormal, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF)
        {
            //collisionDetected = false;
            //return point;
            collisionDetected = false;

            Vector3 toPlane = point - planePosition;
            float dot = Vector3.Dot(planeNormal.normalized, toPlane.normalized);
            Vector3 projectedPoint = GetPointOnPlaneProjection(point, planePosition, planeNormal);

            switch (mode)
            {
                case DynamicColliderProjectionMode.INTO:
                    if (dot < 0)
                        return point;
                    else
                    {
                        collisionDetected = true;
                        return projectedPoint;
                    }
                case DynamicColliderProjectionMode.OUT_OF:
                    if (dot > 0)
                        return point;
                    else
                    {
                        collisionDetected = true;
                        return projectedPoint;
                    }
                case DynamicColliderProjectionMode.ONTO:
                default:
                    collisionDetected = true;
                    return projectedPoint;
            }
        }

        public static Vector3 ResolveHingeLimitCollision(Vector3 planePosition, Vector3 planeNormal, Vector3 hingeForward, float limitAngle, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF)
        {
            collisionDetected = false;

            Vector3 toPlane = point - planePosition;
            float dot = Vector3.Dot(planeNormal.normalized, toPlane.normalized);
            Vector3 projectedPoint = GetPointOnPlaneProjection(point, planePosition, planeNormal);

            Vector3 toCenter = projectedPoint - planePosition;
            float distance = toCenter.magnitude;
            float angle = Vector3.Angle(toCenter.normalized, hingeForward.normalized);
            if (angle < limitAngle)
                return projectedPoint;
            else
            {
                Vector3 clampedVecR = Vector3.RotateTowards(hingeForward.normalized, toCenter.normalized, Mathf.Deg2Rad * limitAngle, 1000f);
                Vector3 clampedVecL = Vector3.RotateTowards(hingeForward.normalized, toCenter.normalized, Mathf.Deg2Rad * -limitAngle, 1000f);
                Vector3 clampedVec = Vector3.Angle(toCenter.normalized, clampedVecR.normalized) < Vector3.Angle(toCenter.normalized, clampedVecL.normalized) ? clampedVecR : clampedVecL;
                collisionDetected = true;
                return planePosition + (clampedVec.normalized * distance);
            }
        }

        public static Vector3 ResolveConeCollision(Vector3 conePosition, Vector3 direction, float limitAngle, float boneLength, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.INTO)
        {
            collisionDetected = false;
            Vector3 toCone = point - conePosition;
            float distanceToCone = toCone.magnitude;
            distanceToCone = boneLength;
            float angle = Vector3.Angle(direction.normalized, toCone.normalized);
            switch (mode)
            {
                case DynamicColliderProjectionMode.INTO:
                default:
                    if (angle < limitAngle)
                        return point;
                    else
                    {
                        collisionDetected = true;
                        Vector3 coneProjection = Vector3.RotateTowards(direction, toCone, Mathf.Deg2Rad * limitAngle, 1000f);
                        return conePosition + coneProjection.normalized * distanceToCone;
                    }
            }
        }

        public static Vector3 ResolveCapsuleCollision(Vector3 capsuleCenter, float capsuleRadius, float capsuleHeight, Vector3 capsuleDirection, Vector3 point, float pointRadius, out bool collisionDetected, DynamicColliderProjectionMode mode = DynamicColliderProjectionMode.OUT_OF)
        {
            //Evaluate line segment
            collisionDetected = false;

            Vector3 p1 = capsuleCenter + capsuleDirection.normalized * capsuleHeight / 2;
            Vector3 p2 = capsuleCenter - capsuleDirection.normalized * capsuleHeight / 2;

            Vector3 direction = p2 - p1;

            Vector3 projectOnLine = FindClosestPointOnLine(p1, direction, point);

            if ((projectOnLine - p1).magnitude < (p2 - p1).magnitude && (projectOnLine - p2).magnitude < (p2 - p1).magnitude)
            {
                float distance = (point - projectOnLine).magnitude;
                float error = (capsuleRadius + pointRadius) - distance;

                var projection = (projectOnLine + ((point - projectOnLine).normalized * (capsuleRadius + pointRadius)));

                switch (mode)
                {
                    case DynamicColliderProjectionMode.INTO:
                        if (distance > (capsuleRadius - pointRadius))
                        {
                            collisionDetected = true;
                            projection = (projectOnLine + ((point - projectOnLine).normalized * (capsuleRadius - pointRadius)));
                            return projection;
                        }
                        break;
                    case DynamicColliderProjectionMode.ONTO:
                        collisionDetected = true;
                        return projection;
                    default:
                        if (distance < (capsuleRadius + pointRadius))
                        {
                            collisionDetected = true;
                            return projection;
                        }
                        break;
                }

            }
            else
            {

                //Evaluate Spheres
                if ((point - p1).magnitude < (point - p2).magnitude)
                {
                    return ResolveSphereCollision(p1, capsuleRadius, point, pointRadius, out collisionDetected, mode);
                }
                else
                {
                    return ResolveSphereCollision(p2, capsuleRadius, point, pointRadius, out collisionDetected, mode);
                }

            }

            return point;
        }

        public virtual Vector3 ResolveCollision(Vector3 point, float pointRadius)
        {
            return point;
        }

    }
}
