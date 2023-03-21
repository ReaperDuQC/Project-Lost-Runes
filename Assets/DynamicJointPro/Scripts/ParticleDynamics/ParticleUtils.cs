using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public struct ParticleUtils
    {
        public static Vector3 TransformPointQuat(Vector3 localPoint, Vector3 targetSpaceOrigin, Quaternion targetSpaceRotation)
        {
            return targetSpaceOrigin + targetSpaceRotation * localPoint;
        }

        public static Vector3 InverseTransformPointQuat(Vector3 worldPos, Vector3 targetSpaceOrigin, Quaternion targetSpaceRotation)
        {
            Vector3 t = targetSpaceOrigin;
            Quaternion r = targetSpaceRotation;
            Vector3 s = Vector3.one;
            Vector3 sInv = new Vector3(1 / s.x, 1 / s.y, 1 / s.z);
            Vector3 q = Vector3.Scale(sInv, (Quaternion.Inverse(r) * (worldPos - t)));
            return q;
        }

    }
}

