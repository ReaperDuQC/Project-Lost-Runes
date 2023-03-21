using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public static class SimpleIKTasks
    {
        public static Quaternion GetSwingRotation(Transform parentTransform, Vector3 originalPos, Vector3 targetPos)
        {
            // SWING ROTATION
            Vector3 v1 = originalPos - parentTransform.position; v1.Normalize();
            Vector3 v2 = targetPos - parentTransform.position; v2.Normalize();

            v1 = parentTransform.InverseTransformVector(v1);
            v2 = parentTransform.transform.InverseTransformVector(v2);

            return Quaternion.FromToRotation(v1.normalized, v2.normalized);
        }

        public static void SwingAlignToParticle(Transform parentTransform, Vector3 originalPos, Vector3 targetPos, float jointLimitStrength = 1, bool IKMode = false, bool solvingForEndNode = false)
        {
            Quaternion swingTargetRotation = GetSwingRotation(parentTransform, originalPos, targetPos);

            Quaternion originalRot = parentTransform.localRotation;
            Quaternion fullRotation = parentTransform.localRotation * swingTargetRotation;
            parentTransform.localRotation = fullRotation;

            //if (enableJointLimits && jointLimitStrength > 0)
            //    ApplyJointLimits(parentNode, jointLimitStrength);
        }

        public static void CCD_Step(Transform endIKNode, Vector3 targetPosition, int numParents, bool useLimits = false, bool forwardMode = true)
        {
            Transform rootIKNode = endIKNode;
            List<Transform> joints = new List<Transform>();

            while (numParents > 0)
            {
                joints.Add(rootIKNode);
                if (rootIKNode.parent == null) break;
                else rootIKNode = rootIKNode.parent;
                numParents--;
            }
            joints.Add(rootIKNode);
            if (forwardMode)
            {
                joints.Reverse();
            }

            for (int i = 0; i < joints.Count; i++)
            {
                if (joints[i] == endIKNode) continue;
                Vector3 originalPosition = endIKNode.position;
                Transform node = joints[i];
                SwingAlignToParticle(node, originalPosition, targetPosition);// jointLimitStrength, true, solvingEndNode);

                if (useLimits)
                {
                    DynamicJointLimit limit = node.GetComponent<DynamicJointLimit>();
                    if (limit != null)
                    {
                        limit.Apply(1f);
                    }
                }
            }
        }
    }
}