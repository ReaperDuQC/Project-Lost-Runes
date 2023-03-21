using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class SimpleIKNode : MonoBehaviour
    {
        public Transform startJoint;
        public Transform endJoint;

        public Transform target;
        public Vector3 targetPosition;

        public int numParents = 4;

        public bool useJointLimits = false;

        private List<Transform> joints;

        private void Start()
        {
            if (endJoint == null) return;


        }
        private void LateUpdate()
        {
            if (endJoint == null) return;

            targetPosition = target != null ? target.position : targetPosition;
            for (int i = 0; i < 3; ++i)
            {
                SimpleIKTasks.CCD_Step(endJoint, targetPosition, numParents, useJointLimits, true);
                SimpleIKTasks.CCD_Step(endJoint, targetPosition, numParents, useJointLimits, false);
            }
        }
    }
}
