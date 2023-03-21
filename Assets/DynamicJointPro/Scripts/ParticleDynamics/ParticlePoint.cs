using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    [System.Serializable]
    public class ParticlePoint
    {
        public Transform trackTransform;
        public Transform parentTransform;
        public bool simulateInParentSpace = false;

        //position
        public Vector3 currentPos;
        public Vector3 oldPos;
        public Vector3 targetPos;

        // frame
        public Vector3 frameVec;

        public float teleportationTolerance = -1f;

        // distance constraints
        //public List<ParticlePoint> neighbors;
        //public List<float> distanceConstraints;

        #region Constructors
        public ParticlePoint(Transform t, float teleportationTolerance = -1f)
        {
            currentPos = oldPos = targetPos = t.position;
            this.teleportationTolerance = teleportationTolerance;
            this.trackTransform = t;
            this.parentTransform = t.parent != null ? t.parent : null;
        }

        public ParticlePoint(Vector3 initialPos, Transform trackTransform, Transform parentFrame, float teleportationTolerance = -1, bool simulateInParentSpace = false)
        {
            currentPos = oldPos = targetPos = initialPos;
            this.teleportationTolerance = teleportationTolerance;
            this.trackTransform = trackTransform;
            this.parentTransform = trackTransform.parent != null ? trackTransform.parent : null;
            if (parentFrame != null) parentTransform = parentFrame;
            this.simulateInParentSpace = simulateInParentSpace;
        }
        #endregion

        #region Getters & Setters
        public void SetTargetPosition(Vector3 targetPos)
        {
            this.targetPos = targetPos;
            if (simulateInParentSpace && parentTransform != null)
                this.targetPos = parentTransform.InverseTransformPoint(this.targetPos);
        }

        public void SetTarget(Vector3 targetPos)
        {
            SetTargetPosition(targetPos);
        }

        public Vector3 GetCurrentPositionParentSpace()
        {
            if (parentTransform != null)
                return parentTransform.InverseTransformPoint(currentPos);
            return currentPos;
        }
        public Vector3 GetCurrentWorldPosition()
        {
            if (parentTransform != null && simulateInParentSpace)
                return parentTransform.TransformPoint(currentPos);
            return currentPos;
        }
        #endregion

        #region Limits
        public void ClampParticlePosition(Quaternion targetSpace, Vector3 limitOrigin, Vector3 limitMin, Vector3 limitMax)
        {
            // Apply Limits to position particle
            Vector3 prevPos = currentPos;
            Vector3 particleTargetSpace = ParticleUtils.InverseTransformPointQuat(GetCurrentWorldPosition(), limitOrigin, targetSpace);

            particleTargetSpace.x = Mathf.Clamp(particleTargetSpace.x, limitMin.x, limitMax.x);
            particleTargetSpace.y = Mathf.Clamp(particleTargetSpace.y, limitMin.y, limitMax.y);
            particleTargetSpace.z = Mathf.Clamp(particleTargetSpace.z, limitMin.z, limitMax.z);

            currentPos = ParticleUtils.TransformPointQuat(particleTargetSpace, limitOrigin, targetSpace);

            if (simulateInParentSpace && parentTransform != null)
                currentPos = parentTransform.InverseTransformPoint(currentPos);

        }

        public void ClampParticleRotation(Quaternion targetFrame, Vector3 limitMin, Vector3 limitMax)
        {
            Vector3 nVec = ParticleUtils.InverseTransformPointQuat(currentPos.normalized, Vector3.zero, targetFrame);

            nVec.x = Mathf.Clamp(nVec.x, limitMin.x, limitMax.x);
            nVec.y = Mathf.Clamp(nVec.y, limitMin.y, limitMax.y);
            nVec.z = Mathf.Clamp(nVec.z, limitMin.z, limitMax.z);

            currentPos = ParticleUtils.TransformPointQuat(nVec, Vector3.zero, targetFrame);
        }
        #endregion

        #region Simulation
        public void UpdateParticlePos(ParticleDynamicsData dynamicParams, Vector3 force, float dt, float dtOld, float freedomFactor = 0, ParticleStateInfo particleState = null)
        {
            targetPos = currentPos + force;
            currentPos = ParticleDynamicTasks.UpdateParticlePosition(currentPos, oldPos, targetPos, dynamicParams, dt, dtOld, out oldPos, freedomFactor, particleState);
        }

        public void UpdateParticlePos(ParticleDynamicsData dynamicParams, float dt, float dtOld, float freedomFactor = 0, ParticleStateInfo particleState = null)
        {
            currentPos = ParticleDynamicTasks.UpdateParticlePosition(currentPos, oldPos, targetPos, dynamicParams, dt, dtOld, out oldPos, freedomFactor, particleState);
        }
        #endregion

        #region Distance Constraints
        /*
        public void AddDistanceConstraint(ParticlePoint neigbor)
        {
            if (neighbors == null)
            {
                neighbors = new List<ParticlePoint>();
                distanceConstraints = new List<float>();
            }

            if (!neighbors.Contains(neigbor))
            {
                neighbors.Add(neigbor);
                distanceConstraints.Add(Vector3.Distance(currentPos, neigbor.currentPos));
            }
        }

        public void ResolveDistanceConstraints()
        {
            if (neighbors == null) return;

            float factor = 1f / (float)neighbors.Count;

            for (int i = 0; i < neighbors.Count; ++i)
            {
                ParticlePoint neigbor = neighbors[i];
                Vector3 delta = currentPos - neigbor.currentPos;
                float error = delta.magnitude - distanceConstraints[i];
                currentPos -= (error) * delta * (factor * 0.5f);
            }
        }
        */
        #endregion
    }
}

