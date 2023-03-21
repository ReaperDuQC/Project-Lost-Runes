using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    public class DynamicJointParticle : ParticleRotatable
    {
        public DynamicJointLimit limit;
        public Vector3 offsetEulers = Vector3.zero;

        public Vector3 gravityDirection = Vector3.forward;
        public Vector3 gravitySecondaryDirection = Vector3.right;
        public float gravityLength = 1f;
        public Vector3 gravityPosParentSpace = Vector3.forward;
        public Vector3 gravitySecondaryPosParentSpace = Vector3.right;

        public Vector3 gravityCurrentPos = Vector3.forward;
        public Vector3 gravityOldPos = Vector3.forward;

        public Vector3 gravitySecondaryCurrentPos = Vector3.forward;
        public Vector3 gravitySecondaryOldPos = Vector3.forward;

        public DynamicJointParticle parentParticle;
        public DynamicJointParticle childParticle;

        public float cachedTwistForce = 0;
        public Quaternion cachedTwistRotation;

        public void RefreshProperties()
        {
            limit = trackTransform.GetComponent<DynamicJointLimit>();
        }

        #region Initialization
        public override void Initialize(Transform trackTransform, Transform parentFrame = null, bool useWeightShift = false, float teleportationTolerance = -1F, bool simulateInParentSpace = true)
        {
            base.Initialize(trackTransform, parentFrame, useWeightShift, teleportationTolerance, simulateInParentSpace);
            AutoSetGravityDirection();
            limit = trackTransform.GetComponent<DynamicJointLimit>();
        }

        public void AutoSetGravityDirection()
        {
            var dir = Vector3.forward;
            if (this.childParticle != null)
            {
                dir = this.childParticle.trackTransform.position - this.trackTransform.position;
                if (dir.magnitude == 0) dir = Vector3.forward;
            }
            else if (this.trackTransform.childCount > 0)
            {
                dir = this.trackTransform.GetChild(0).transform.position - this.trackTransform.position;
                if (dir.magnitude == 0) dir = Vector3.forward;
            }
            this.gravityDirection = this.trackTransform.InverseTransformVector(dir);
            this.gravitySecondaryDirection = new Vector3(gravityDirection.y, gravityDirection.z, gravityDirection.x);

            ReEncodeGravityPosition();
        }

        public void ReEncodeGravityPosition()
        {
            this.gravityCurrentPos = this.gravityOldPos = this.trackTransform.position + this.trackTransform.TransformVector(gravityDirection);
            this.gravityLength = gravityDirection.magnitude;
            if (this.trackTransform.parent != null)
                this.gravityPosParentSpace = trackTransform.parent.InverseTransformPoint(gravityCurrentPos);

            this.gravitySecondaryCurrentPos = this.gravitySecondaryOldPos = this.trackTransform.position + this.trackTransform.TransformVector(gravitySecondaryDirection);
            if (this.trackTransform.parent != null)
                this.gravitySecondaryPosParentSpace = trackTransform.parent.InverseTransformPoint(gravitySecondaryCurrentPos);
        }

        #endregion

        #region Simulation
        public override void Simulate(ParticleDynamicsData positionDynamics, ParticleDynamicsData rotationDynamics, float dt, float dtOld = 0)
        {
            Transform parentTR = trackTransform.parent;

            positionParticle.UpdateParticlePos(positionDynamics, dt, dtOld, 1);
            forwardParticle.UpdateParticlePos(rotationDynamics, dt, dtOld, 1);
            upParticle.UpdateParticlePos(rotationDynamics, dt, dtOld, 1);

            UpdateOutputs();
        }

        public void UpdatePositionOutput()
        {
            if (useLimits)
            {
                Quaternion frameRotation = Quaternion.LookRotation(trackTransform.TransformDirection(gravityDirection), trackTransform.TransformDirection(gravitySecondaryDirection));
                if (usePositionLimits)
                    positionParticle.ClampParticlePosition(frameRotation, positionParticle.targetPos, minPositionLimits, maxPositionLimits);
            }
            outPosition = positionParticle.GetCurrentWorldPosition();
        }

        public void UpdateRotationOutputs()
        {
            Vector3 orientationOffset = useWeightShift ? trackTransform.position : Vector3.zero;
            Vector3 forward = forwardParticle.currentPos - orientationOffset;
            Vector3 up = upParticle.currentPos - orientationOffset;
            Vector3.OrthoNormalize(ref forward, ref up);

            forwardParticle.currentPos = forward + orientationOffset;
            upParticle.currentPos = up + orientationOffset;

            Quaternion lookRot = Quaternion.LookRotation(forward, up);

            Quaternion lookRotClamped = lookRot;

            if (useLimits)
            {
                if (useRotationLimits && limit != null)
                {
                    limit.transform.rotation = lookRot;
                    limit.Apply();
                    lookRotClamped = limit.transform.rotation;

                    forwardParticle.currentPos = limit.transform.forward + orientationOffset;
                    upParticle.currentPos = limit.transform.up + orientationOffset;
                }
                else
                {
                    lookRotClamped = Quaternion.LookRotation(forward, up);
                }

            }
            outRotation = lookRotClamped;
        }

        public void UpdateOutputs()
        {
            UpdatePositionOutput();
            UpdateRotationOutputs();
        }

        #endregion

        #region Spatial-relation retargetting

        public void RetargetToParent(Transform parentSpace)
        {
            var frame = parentTransform != null ? parentFrame : localFrame;
            Vector3 posLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition, frame.framePosition, frame.frameRotation);
            Vector3 posTarget = ParticleUtils.TransformPointQuat(posLocal, parentSpace.position, parentSpace.rotation);

            Vector3 forwardLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition + initialForward, frame.framePosition, frame.frameRotation);
            Vector3 forwardTarget = ParticleUtils.TransformPointQuat(forwardLocal, parentSpace.position, parentSpace.rotation) - posTarget;

            Vector3 upLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition + initialUp, frame.framePosition, frame.frameRotation);
            Vector3 upTarget = ParticleUtils.TransformPointQuat(upLocal, parentSpace.position, parentSpace.rotation) - posTarget;

            Vector3.OrthoNormalize(ref forwardTarget, ref upTarget);

            forwardTarget.Normalize();
            upTarget.Normalize();

            SetParticleLimitsFrame(forwardTarget, upTarget);

            if (useWeightShift)
            {
                forwardTarget += posTarget;
                upTarget += posTarget;
            }

            UpdateParticleTargets(posTarget, forwardTarget, upTarget);
        }

        public void ApplyDistanceConstraint()
        {
            Vector3 parentPos = this.trackTransform.position;
            Vector3 toParent = this.gravityCurrentPos - parentPos;
            Vector3 newPos = parentPos + toParent.normalized * gravityLength;
            this.gravityCurrentPos += (newPos - this.gravityCurrentPos) * 1f;
        }

        public void SimulateGravity(Vector3 gravityForce, float inertia, ParticleDynamicsData positionDynamics, ParticleDynamicsData rotationDynamics, float dt, float dtOld = 0)
        {
            // simulate position stretch
            positionParticle.SetTarget(parentTransform.TransformPoint(initialLocalPosition));
            positionParticle.UpdateParticlePos(positionDynamics, dt, dtOld, 1);
            UpdatePositionOutput();
            this.trackTransform.position = positionParticle.GetCurrentWorldPosition();

            // simulate mass particle
            Vector3 inertiaForce = Vector3.zero;
            Vector3 inertiaForceSecondary = Vector3.zero;

            if (inertia > 0)
            {
                var desiredPos = this.trackTransform.parent.TransformPoint(this.gravityPosParentSpace);
                inertiaForce = (desiredPos - gravityCurrentPos) * inertia;

                var desiredPosSecondary = this.trackTransform.parent.TransformPoint(this.gravitySecondaryPosParentSpace);
                inertiaForceSecondary = (desiredPosSecondary - gravitySecondaryCurrentPos) * inertia;
            }

            /*
            if (cachedTwistForce != 0)
            {
                var secondaryAxis = gravitySecondaryCurrentPos - trackTransform.position;
                var target = trackTransform.position + (cachedTwistRotation * secondaryAxis);
                var twistForce = target - gravitySecondaryCurrentPos * 20f;
                Debug.DrawLine(target, gravitySecondaryCurrentPos);
                Debug.DrawLine(trackTransform.position, gravitySecondaryCurrentPos);
                inertiaForceSecondary += twistForce;
            }
            */

            Vector3 finalForce = (gravityForce) + inertiaForce;
            Vector3 finalForceSecondary = inertiaForceSecondary;

            var targetPos = this.gravityCurrentPos + finalForce;
            this.gravityCurrentPos = ParticleDynamicTasks.UpdateParticlePosition(gravityCurrentPos, gravityOldPos, targetPos, rotationDynamics, dt, dtOld, out gravityOldPos);

            // simulate rotation from gravity particle
            Vector3 newGravDir = this.gravityCurrentPos - this.trackTransform.position;
            var desiredTarget = this.gravityCurrentPos;
            Quaternion deltaRotation = Quaternion.FromToRotation((this.trackTransform.TransformVector(this.gravityDirection.normalized)).normalized, newGravDir.normalized);
            this.gravitySecondaryCurrentPos = trackTransform.position + (deltaRotation * (this.gravitySecondaryCurrentPos - trackTransform.position));

            var targetPosSecondary = this.gravitySecondaryCurrentPos + finalForceSecondary;
            this.gravitySecondaryCurrentPos = ParticleDynamicTasks.UpdateParticlePosition(gravitySecondaryCurrentPos, gravitySecondaryOldPos, targetPosSecondary, rotationDynamics, dt, dtOld, out gravitySecondaryOldPos);

            Vector3 newGravDirSecondary = this.gravitySecondaryCurrentPos - this.trackTransform.position;
            Vector3.OrthoNormalize(ref newGravDir, ref newGravDirSecondary);


            var currentGravDir = this.trackTransform.TransformDirection(this.gravityDirection.normalized).normalized;
            var currentSecondaryGravDir = this.trackTransform.TransformDirection(this.gravitySecondaryDirection.normalized).normalized;

            deltaRotation = Quaternion.LookRotation(newGravDir, newGravDirSecondary) * Quaternion.Inverse(Quaternion.LookRotation(currentGravDir, currentSecondaryGravDir));

            var f = trackTransform.forward;
            var u = trackTransform.up;

            forwardParticle.currentPos = deltaRotation * f;
            upParticle.currentPos = deltaRotation * u;

            //ApplyDistanceConstraint();

            UpdateRotationOutputs();

            this.trackTransform.rotation = outRotation;
            SnapParticlePositionsToActualPositions();

            /*
            //currentGravDir = outRotation * this.gravityDirection.normalized;
            //Quaternion remainingRotation = Quaternion.FromToRotation(currentGravDir, newGravDir);
            if (parentParticle != null && parentParticle.parentParticle == null)
            {
                var axis = parentParticle.GetCurrentGravDir().normalized;
                //axis = parentParticle.limit.GetMainAxisWorld();
                var v1 = Vector3.ProjectOnPlane(gravityCurrentPos - parentParticle.trackTransform.position, axis.normalized).normalized;
                var v2 = Vector3.ProjectOnPlane(desiredTarget - parentParticle.trackTransform.position, axis.normalized).normalized;
                var vTarget = Vector3.RotateTowards(v1.normalized, v2.normalized, Time.deltaTime * 100f * Mathf.Deg2Rad, 1000f);

                var twistForce = Vector3.SignedAngle(v1, vTarget, axis.normalized);
                //parentParticle.cachedTwistForce = twistForce;
                Debug.DrawLine(parentParticle.trackTransform.position, parentParticle.trackTransform.position + v1);
                Debug.DrawLine(parentParticle.trackTransform.position, parentParticle.trackTransform.position + v2);
                deltaRotation = Quaternion.FromToRotation(v1, vTarget);

                if (parentParticle.parentParticle == null)
                {
                    //var lookRot = Quaternion.LookRotation(parentParticle.forwardParticle.currentPos, parentParticle.upParticle.currentPos);
                    //parentParticle.forwardParticle.currentPos = deltaRotation * parentParticle.trackTransform.forward;
                    //parentParticle.upParticle.currentPos = deltaRotation * parentParticle.trackTransform.up;
                    //parentParticle.UpdateRotationOutputs();
                    //parentParticle.trackTransform.rotation *= deltaRotation;
                    //parentParticle.limit.Apply(1f);
                    //parentParticle.trackTransform.rotation = parentParticle.outRotation;
                    //parentParticle.SnapParticlePositionsToActualPositions();

                    parentParticle.cachedTwistForce = twistForce;
                    parentParticle.cachedTwistRotation = deltaRotation;

                    //parentParticle.limit.transform.rotation = lookRot;
                    //parentParticle.forwardParticle.currentPos = 
                    //parentParticle.limit.Apply(1f);

                    //parentParticle.UpdateRotationOutputs();
                    //parentParticle.gravityCurrentPos = parentParticle.trackTransform.TransformVector(parentParticle.gravityDirection.normalized * parentParticle.gravityLength);
                    //parentParticle.gravitySecondaryCurrentPos = parentParticle.trackTransform.TransformVector(parentParticle.gravitySecondaryDirection.normalized * 1f);
                }

                //UpdateRotationOutputs();

                //Debug.DrawLine(parentParticle.gravitySecondaryCurrentPos, parentParticle.gravitySecondaryCurrentPos + );
            }

            SnapParticlePositionsToActualPositions();
            */
        }

        public Vector3 GetActualGravPos()
        {
            return trackTransform.position + (trackTransform.TransformDirection(gravityDirection));
        }

        public Vector3 GetActualSecondaryGravPos()
        {
            return trackTransform.position + (trackTransform.TransformDirection(gravitySecondaryDirection));
        }


        public void SnapParticlePositionsToActualPositions()
        {
            //gravityCurrentPos = GetActualGravPos();
            //gravitySecondaryCurrentPos = GetActualSecondaryGravPos();
            gravityCurrentPos = trackTransform.TransformVector(gravityDirection.normalized * gravityLength);
            gravitySecondaryCurrentPos = trackTransform.TransformVector(gravitySecondaryDirection.normalized * 1f);
        }

        public Vector3 GetCurrentGravDir()
        {
            return this.gravityCurrentPos - trackTransform.position;
        }

        public void RetargetToRelativePoints(List<Vector3> initialPositions, List<Vector3> newPositions, List<Quaternion> initialQuats, List<Quaternion> newQuats)
        {
            Vector3 posLocal = Vector3.zero;
            Vector3 posTarget = Vector3.zero;

            int tCount = initialPositions.Count;

            for (int i = 0; i < tCount; ++i)
            {
                posLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition, initialPositions[i], initialQuats[i]);
                posTarget += ParticleUtils.TransformPointQuat(posLocal, newPositions[i], newQuats[i]);
            }

            Vector3 forwardLocal = Vector3.zero;
            Vector3 forwardTarget = Vector3.zero;
            for (int i = 0; i < tCount; ++i)
            {
                forwardLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition + initialForward, initialPositions[i], initialQuats[i]);
                forwardTarget += ParticleUtils.TransformPointQuat(forwardLocal, newPositions[i], newQuats[i]);
            }

            Vector3 upLocal = Vector3.zero;
            Vector3 upTarget = Vector3.zero;
            for (int i = 0; i < tCount; ++i)
            {
                upLocal = ParticleUtils.InverseTransformPointQuat(initialWorldPosition + initialUp, initialPositions[i], initialQuats[i]);
                upTarget += ParticleUtils.TransformPointQuat(upLocal, newPositions[i], newQuats[i]);
            }

            posTarget /= tCount;
            upTarget /= tCount;
            forwardTarget /= tCount;

            if (!useWeightShift)
            {
                forwardTarget -= posTarget;
                upTarget -= posTarget;
            }

            Quaternion targetRotation = Quaternion.LookRotation(forwardTarget.normalized, upTarget.normalized);

            SetParticleLimitsFrame(forwardTarget, upTarget);

            UpdateParticleTargets(posTarget, forwardTarget, upTarget);
        }

        public void RetargetToRelativeTriangulation(List<Vector3> initialPositions, List<Vector3> newPositions, List<Vector3> spatialRelationTris)
        {
            int wheelsCount = initialPositions.Count;
            Quaternion rot = Quaternion.identity;

            List<float> angles = new List<float>();
            Vector3 avgPos = Vector3.zero;
            Vector3 avgForward = Vector3.zero;
            Vector3 avgRight = Vector3.zero;
            Vector3 avgUp = Vector3.zero;
            float weightTotal = 0;

            Quaternion offset = Quaternion.Euler(this.offsetEulers);

            int triCount = spatialRelationTris.Count;
            foreach (Vector3 tri in spatialRelationTris)
            {
                Vector3 outNorm;
                Vector3 nPos = RigidSpatialRelations.GetPointInSpatialRelationSpace(initialPositions, newPositions, tri, initialWorldPosition, out outNorm);
                Vector3 nForward = RigidSpatialRelations.GetPointInSpatialRelationSpace(initialPositions, newPositions, tri, (initialWorldPosition + (offset * initialForward)), out outNorm);
                Vector3 nUp = RigidSpatialRelations.GetPointInSpatialRelationSpace(initialPositions, newPositions, tri, (initialWorldPosition + (offset * initialUp)), out outNorm);

                float weight = 1f / outNorm.magnitude;
                weightTotal += weight;

                if (!useWeightShift)
                {
                    nForward -= nPos;
                    nUp -= nPos;
                }

                avgPos += nPos * weight;
                avgForward += nForward * weight;
                avgUp += nUp * weight;

                //Quaternion q = RigidSpatialRelations.GetSpatialRelationQuat(initialPositions, newPositions, tri);
                //q = Quaternion.Slerp(Quaternion.identity, q, 1f / triCount);
            }

            avgPos /= weightTotal;
            avgForward /= weightTotal;
            avgUp /= weightTotal;

            UpdateParticleTargets(avgPos, avgForward, avgUp);
        }

        #endregion

    }

    [System.Serializable]
    public class RigidSpatialRelations
    {
        public List<Transform> relativePoints;
        [SerializeField]
        [HideInInspector]
        private List<Vector3> tris;
        [SerializeField]
        [HideInInspector]
        private List<Vector3> initialPositions;
        [SerializeField]
        [HideInInspector]
        private List<Quaternion> initialQuats;

        public RigidSpatialRelations(List<Transform> sources)
        {
            this.relativePoints = sources;
            TriangulateSpatialRelations();
        }

        public void SetInitialPositions(List<Vector3> newInitialPositions)
        {
            this.initialPositions = newInitialPositions;
        }

        public List<Vector3> GetTriangulation()
        {
            return tris;
        }

        public List<Vector3> GetCurrentPositions()
        {
            return relativePoints.Select(v => v.position).ToList();
        }

        public List<Vector3> GetInitialPositions()
        {
            return initialPositions;
        }

        public List<Quaternion> GetInitialQuats()
        {
            return initialQuats;
        }

        public List<Quaternion> GetCurrentQuats()
        {
            return relativePoints.Select(v => v.rotation).ToList();
        }


        public void TriangulateSpatialRelations()
        {
            tris = new List<Vector3>();
            initialPositions = new List<Vector3>();
            initialQuats = new List<Quaternion>();


            // triangulate
            for (int i = 0; i < relativePoints.Count; ++i)
            {
                initialPositions.Add(relativePoints[i].position);
                initialQuats.Add(relativePoints[i].rotation);

                for (int j = i + 1; j < relativePoints.Count; ++j)
                {
                    for (int k = j + 1; k < relativePoints.Count; ++k)
                    {

                        tris.Add(new Vector3(i, j, k));
                    }
                }

            }
        }

        public static Quaternion GetSpatialRelationQuat(List<Vector3> initialPositions, List<Vector3> newPositions, Vector3 tri)
        {
            Vector3 v1 = initialPositions[(int)tri[0]];
            Vector3 v2 = initialPositions[(int)tri[1]];
            Vector3 v3 = initialPositions[(int)tri[2]];
            Vector3 oNorm = Vector3.Cross((v2 - v1).normalized, (v3 - v1).normalized);

            Vector3 x1 = newPositions[(int)tri[0]];
            Vector3 x2 = newPositions[(int)tri[1]];
            Vector3 x3 = newPositions[(int)tri[2]];
            Vector3 newNorm = Vector3.Cross((x2 - x1).normalized, (x3 - x1).normalized);

            Quaternion oldRotation = Quaternion.LookRotation(v2 - v1, oNorm);
            Quaternion newRotation = Quaternion.LookRotation(x2 - x1, newNorm);

            return (newRotation * Quaternion.Inverse(oldRotation));
        }

        public static Vector3 GetPointInSpatialRelationSpace(List<Vector3> initialPositions, List<Vector3> newPositions, Vector3 tri, Vector3 originalTargetPointWorld, out Vector3 norm)
        {
            Vector3 v1 = initialPositions[(int)tri[0]];
            Vector3 v2 = initialPositions[(int)tri[1]];
            Vector3 v3 = initialPositions[(int)tri[2]];
            Vector3 oNorm = Vector3.Cross((v2 - v1).normalized, (v3 - v1).normalized);
            Vector3 projectedP = v1 + Vector3.ProjectOnPlane(originalTargetPointWorld - v1, oNorm);

            float nD = Vector3.Distance(originalTargetPointWorld, projectedP);

            var signage = Vector3.Dot((originalTargetPointWorld - projectedP).normalized, oNorm) > 0 ? 1.0f : -1.0f;
            var b = new Barycentric(v1, v2, v3, projectedP);

            Vector3 x1 = newPositions[(int)tri[0]];
            Vector3 x2 = newPositions[(int)tri[1]];
            Vector3 x3 = newPositions[(int)tri[2]];
            norm = Vector3.Cross((x2 - x1).normalized, (x3 - x1).normalized);
            Vector3 nPos = b.Interpolate(x1, x2, x3);
            norm = norm.normalized * nD * signage;
            nPos += norm;

            return nPos;
        }

        public static Vector3 GetPointInSpatialRelationSpace(List<Vector3> initialPositions, List<Vector3> newPositions, List<Vector3> tris, Vector3 originalTargetPointWorld)
        {
            Vector3 fPos = Vector3.zero;
            Vector3 norm = Vector3.zero;

            float weightTotal = 0;

            for (int i = 0; i < tris.Count; ++i)
            {
                Vector3 tri = tris[i];
                Vector3 v1 = initialPositions[(int)tri[0]];
                Vector3 v2 = initialPositions[(int)tri[1]];
                Vector3 v3 = initialPositions[(int)tri[2]];
                Vector3 oNorm = Vector3.Cross((v2 - v1).normalized, (v3 - v1).normalized);
                Vector3 projectedP = v1 + Vector3.ProjectOnPlane(originalTargetPointWorld - v1, oNorm);

                float nD = Vector3.Distance(originalTargetPointWorld, projectedP);

                var signage = Vector3.Dot((originalTargetPointWorld - projectedP).normalized, oNorm) > 0 ? 1.0f : -1.0f;
                var b = new Barycentric(v1, v2, v3, projectedP);

                Vector3 x1 = newPositions[(int)tri[0]];
                Vector3 x2 = newPositions[(int)tri[1]];
                Vector3 x3 = newPositions[(int)tri[2]];
                norm = Vector3.Cross((x2 - x1).normalized, (x3 - x1).normalized);
                Vector3 nPos = b.Interpolate(x1, x2, x3);
                norm = norm.normalized * nD * signage;
                nPos += norm;

                float weight = 1f / norm.magnitude;
                weightTotal += weight;
                fPos += nPos * weight;
            }

            fPos /= weightTotal;

            return fPos;
        }

        public Vector3 GetPointInSpatialRelationSpace(Vector3 originalTargetPointWorld)
        {
            List<Vector3> newPositions = GetCurrentPositions();

            Vector3 fPos = Vector3.zero;
            Vector3 norm = Vector3.zero;

            float weightTotal = 0;

            for (int i = 0; i < tris.Count; ++i)
            {
                Vector3 tri = tris[i];
                Vector3 v1 = initialPositions[(int)tri[0]];
                Vector3 v2 = initialPositions[(int)tri[1]];
                Vector3 v3 = initialPositions[(int)tri[2]];
                Vector3 oNorm = Vector3.Cross((v2 - v1).normalized, (v3 - v1).normalized);
                Vector3 projectedP = v1 + Vector3.ProjectOnPlane(originalTargetPointWorld - v1, oNorm);

                float nD = Vector3.Distance(originalTargetPointWorld, projectedP);

                var signage = Vector3.Dot((originalTargetPointWorld - projectedP).normalized, oNorm) > 0 ? 1.0f : -1.0f;
                var b = new Barycentric(v1, v2, v3, projectedP);

                Vector3 x1 = newPositions[(int)tri[0]];
                Vector3 x2 = newPositions[(int)tri[1]];
                Vector3 x3 = newPositions[(int)tri[2]];
                norm = Vector3.Cross((x2 - x1).normalized, (x3 - x1).normalized);
                Vector3 nPos = b.Interpolate(x1, x2, x3);
                norm = norm.normalized * nD * signage;
                nPos += norm;

                //float weight = 1f / norm.magnitude;
                float weight = norm.magnitude;
                weight = 1f;

                weightTotal += weight;
                fPos += nPos * weight;
            }

            fPos /= weightTotal;

            return fPos;
        }
    }
}

