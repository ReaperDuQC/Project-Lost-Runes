using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    [System.Serializable]
    public class ParticleRelativeConstraint
    {
        public ParticleConstraintType constraintType;
        public float constraintValue;
        public DynamicChainParticle otherParticle;

        public ParticleRelativeConstraint(DynamicChainParticle otherParticle, ParticleConstraintType constraintType, float value)
        {
            this.constraintType = constraintType;
            this.constraintValue = value;
            this.otherParticle = otherParticle;
        }
    }

    [System.Serializable]
    public class ParticlePosQuatConstraint
    {
        public PositionConstraintType constraintType;
        public Vector3 targetPos;
        public Transform targetTr;
        public float targetForceStrength = 0;
        public bool constrainRotation = false;

        public void SetConstraint(PositionConstraintType constraintType, Transform target)
        {
            this.targetTr = target;
            this.targetPos = target.position;
            this.constraintType = constraintType;
        }
    }

    [System.Serializable]
    public class ParticleStateInfo
    {
        public bool isColliding = false;
        public bool isJointLimited = false;
        public Vector3 lastBounceVector = Vector3.zero;
        public float bounce = 0;
        public float friction = 0;
    }

    [System.Serializable]
    public class DynamicChainParticle : ParticleRotatable
    {
        #region Variables
        //gravity particle
        public ParticlePoint gravityParticle;
        public Vector3 gravityDirection = Vector3.right;
        public Vector3 gravityDirectionLocal = Vector3.right;
        public Vector3 gravityDirectionRoot = Vector3.right;

        // connective hierarchy
        public Transform childTransform = null;
        public DynamicChainParticle parentParticle;
        public DynamicChainParticle childParticle;

        // hierarchy params
        public float boneLength = 0;
        public float distanceToRoot = 0;
        //public float freedomFactor = 0;

        // applied forces
        public Vector3 gravityForce = Vector3.down * 10f;
        public Vector3 externalForces = Vector3.zero;
        public float massInertia = 0;
        public DynamicChainParticle rootParticle;
        public float dynamicTwistRate = 45f;

        // main force params
        public bool useGravity = false;

        // initial local positions/orientations in different spaces
        public Transform rootSpaceTransform;
        public Quaternion rootInitLocalRotation;
        public Vector3 initialWorldPos = Vector3.right;
        public Vector3 gravityInitialParentSpace;
        public Vector3 gravityInitialRootSpace;

        // IK
        public bool enableIK = false;

        // Joint limit
        public DynamicJointLimit limit;
        public float jointLimitStrength;

        //collisions
        public List<DynamicCollider> dynamicColliders;

        // Constraints
        public List<ParticleRelativeConstraint> constraints;
        public bool enablePosQuatConstraint = false;
        public ParticlePosQuatConstraint posQuatConstraint = new ParticlePosQuatConstraint();
        public bool IsStrictlyConstrained { get { return enablePosQuatConstraint && posQuatConstraint.constraintType == PositionConstraintType.POSITION_CONSTRAINT; } }


        // cached previous state variables
        public Vector3 lastParentPosition = Vector3.zero;
        public Quaternion lastRotation = Quaternion.identity;
        public ParticleStateInfo particleState = new ParticleStateInfo();

        #endregion

        #region Initialization & Resetting
        public override void Initialize(Transform trackTransform, Transform parentFrame = null, bool useWeightShift = false, float teleportationTolerance = -1f, bool simulateInParentSpace = true)
        {
            this.trackTransform = trackTransform;
            this.positionParticle = new ParticlePoint(trackTransform.position, trackTransform, parentFrame, teleportationTolerance, simulateInParentSpace: false);
            this.forwardParticle = new ParticlePoint(trackTransform.forward, trackTransform, parentFrame, simulateInParentSpace: false);
            this.upParticle = new ParticlePoint(trackTransform.up, trackTransform, parentFrame, simulateInParentSpace: false);


            // cache initial orientation config
            initialLocalOrientation = trackTransform.localRotation;
            initialLocalPosition = trackTransform.localPosition;
            initialForward = trackTransform.forward;
            initialUp = trackTransform.up;
            initialRight = trackTransform.right;

            parentTransform = trackTransform.parent;

            limit = trackTransform.GetComponent<DynamicJointLimit>();

            this.localFrame = new ParticleFrame(trackTransform.position, trackTransform.localRotation);
            this.boneLength = parentTransform != null ? Vector3.Distance(trackTransform.position, parentTransform.position) : 0;

            constraints = new List<ParticleRelativeConstraint>();
            if (parentParticle != null)
            {
                constraints.Add(new ParticleRelativeConstraint(parentParticle, ParticleConstraintType.FIXED_DISTANCE, boneLength));
            }

            this.particleState = new ParticleStateInfo();
        }

        public void SetupGravityParticle(Vector3 gravityVec, DynamicChainParticle root)
        {
            boneLength = gravityVec.magnitude;
            useGravity = true;
            gravityDirectionLocal = ParticleUtils.InverseTransformPointQuat(trackTransform.position + gravityVec, trackTransform.position, trackTransform.rotation);

            if (root != null)
                gravityDirectionRoot = ParticleUtils.InverseTransformPointQuat(trackTransform.position + gravityVec, root.trackTransform.position, root.trackTransform.rotation);
            else
                gravityDirectionRoot = gravityVec;

            if (trackTransform.parent != null)
                gravityInitialParentSpace = ParticleUtils.InverseTransformPointQuat(trackTransform.position + gravityVec, trackTransform.parent.position, trackTransform.parent.rotation);
            else
                gravityInitialParentSpace = trackTransform.position + gravityVec;

            this.rootParticle = root;

            this.gravityParticle = new ParticlePoint(this.trackTransform.position + gravityVec, trackTransform, parentTransform, simulateInParentSpace: false);
        }

        public void ResetToLastFrameOrientation()
        {
            trackTransform.rotation = lastRotation;
            trackTransform.position = lastParentPosition;
        }

        public void ResetToInitialStaticRotation()
        {
            trackTransform.localRotation = initialLocalOrientation;
        }

        public void UpdateInertiaTargetsFromAnimation()
        {
            gravityInitialRootSpace = ParticleUtils.InverseTransformPointQuat(GetActualPosition(), rootSpaceTransform.position, rootSpaceTransform.rotation);
        }
        #endregion

        #region Setters & Getters
        public Vector3 GetCurrentParticlePos()
        {
            return gravityParticle.currentPos;
        }

        public Vector3 GetParentParticlePos()
        {
            return parentParticle != null ? parentParticle.GetCurrentParticlePos() : trackTransform.position;
        }

        public Vector3 GetOldMassPos()
        {
            return gravityParticle.currentPos;
        }

        public void SetCurrentParticlePos(Vector3 position)
        {
            gravityParticle.currentPos = position;
        }

        public void SetOldMassPos(Vector3 position)
        {
            gravityParticle.oldPos = position;
        }

        public Transform GetParentTransform()
        {
            return (parentParticle != null ? parentParticle.trackTransform : trackTransform.parent);
        }

        public Vector3 GetActualPosition()
        {
            if (childTransform != null) return childTransform.position;
            return ParticleUtils.TransformPointQuat(gravityDirectionLocal, trackTransform.position, trackTransform.rotation);
        }
        #endregion

        #region Distance Constraint
        public void ApplyDistanceConstraint()
        {
            Vector3 parentPos = (parentParticle != null ? parentParticle.GetCurrentParticlePos() : trackTransform.position);
            Vector3 toParent = gravityParticle.currentPos - parentPos;
            Vector3 newPos = parentPos + toParent.normalized * boneLength;
            gravityParticle.currentPos += (newPos - gravityParticle.currentPos) * 1f;
        }

        public void RemoveDistanceConstraint(ParticleRelativeConstraint constraint)
        {
            constraints.Remove(constraint);
        }

        public void AddDistanceConstraint(DynamicChainParticle neigbor, float distanceConstraint, ParticleConstraintType constraintMode = ParticleConstraintType.FIXED_DISTANCE)
        {
            if (neigbor != null)
            {
                if (constraints == null)
                {
                    constraints = new List<ParticleRelativeConstraint>();
                }
                var constraint = constraints.Find(c => c.otherParticle == neigbor);
                if (constraint != null)
                {
                    constraints.Remove(constraint);
                }
                constraints.Add(new ParticleRelativeConstraint(neigbor, constraintMode, distanceConstraint));
            }
        }

        public Vector3 GetConstraintTargetPosition()
        {
            if (posQuatConstraint.targetTr != null) return posQuatConstraint.targetTr.position;
            return posQuatConstraint.targetPos;
        }

        #endregion

        #region Joint Hierarchy & Alignment
        public bool HasParent { get { return parentTransform != null; } }

        public void EncodeInRootSpace(Transform rootSpace)
        {
            if (rootSpace != null)
            {
                rootSpaceTransform = rootSpace;
                rootInitLocalRotation = rootSpace.localRotation;
                gravityInitialRootSpace = ParticleUtils.InverseTransformPointQuat(GetActualPosition(), rootSpace.position, rootSpace.rotation);
            }
        }

        public void AlignTransformToTarget(Vector3 fromPoint, Vector3 toPoint)
        {
            Vector3 currentGravVec = fromPoint - trackTransform.position;
            Vector3 newGravVec = toPoint - trackTransform.position;

            Quaternion deltaRotation = Quaternion.FromToRotation(currentGravVec.normalized, newGravVec.normalized);
            Quaternion currentRot = trackTransform.rotation;

            forwardParticle.targetPos = deltaRotation * trackTransform.forward;
            upParticle.targetPos = deltaRotation * trackTransform.up;

            forwardParticle.currentPos = forwardParticle.targetPos;
            upParticle.currentPos = upParticle.targetPos;

            Quaternion lookRot = Quaternion.LookRotation(forwardParticle.currentPos, upParticle.currentPos);
            Quaternion lookRotClamped = lookRot;

            if (useLimits)
            {
                if (useRotationLimits && limit != null)
                {
                    limit.transform.rotation = lookRot;
                    limit.Apply(jointLimitStrength);
                    lookRotClamped = limit.transform.rotation;
                }
                else
                {
                    trackTransform.rotation = lookRot;
                }
            }
            else
            {
                trackTransform.rotation = lookRot;
            }

            gravityParticle.currentPos = GetActualPosition();
        }

        private void ResolveStretch()
        {
            //if (childParticle == null) return;
            var stretchedLength = (GetCurrentParticlePos() - GetParentParticlePos()).magnitude;
            var toParent = GetActualPosition() - trackTransform.position;
            float stretch = stretchedLength - boneLength;

            if (childParticle != null)
            {
                childParticle.trackTransform.localPosition = childParticle.initialLocalPosition;
                childParticle.trackTransform.position += (stretch * toParent.normalized);
            }
        }

        private void AlignTransformToTarget(Vector3 target, bool solveTwist = false)
        {
            var initPos = GetActualPosition();

            Vector3 currentGravVec = GetActualPosition() - trackTransform.position;
            Vector3 newGravVec = target - trackTransform.position;

            Quaternion deltaRotation = Quaternion.FromToRotation(currentGravVec.normalized, newGravVec.normalized);
            Quaternion currentRot = trackTransform.rotation;

            forwardParticle.targetPos = deltaRotation * trackTransform.forward;
            upParticle.targetPos = deltaRotation * trackTransform.up;

            forwardParticle.currentPos = forwardParticle.targetPos;
            upParticle.currentPos = upParticle.targetPos;

            Quaternion lookRot = Quaternion.LookRotation(forwardParticle.currentPos, upParticle.currentPos);
            Quaternion lookRotClamped = lookRot;

            if (useLimits && limit != null)
            {
                limit.transform.rotation = lookRot;
                particleState.isJointLimited = limit.Apply(jointLimitStrength);
                lookRotClamped = limit.transform.rotation;
                initPos = GetActualPosition();

                var d = Vector3.Distance(initPos, target);

                //Resolve Additional Twist
                Quaternion rotDelta = lookRotClamped * Quaternion.Inverse(lookRot);

                if (solveTwist && this.parentParticle != null && this.parentParticle.useLimits && this.parentParticle.limit != null)
                {
                    var parentP = this.parentParticle;
                    var twistAxis = this.parentParticle.limit.GetMainAxisWorld();
                    twistAxis = this.trackTransform.position - parentParticle.trackTransform.position;
                    twistAxis.Normalize();

                    var v1 = Vector3.ProjectOnPlane(initPos - parentP.trackTransform.position, twistAxis.normalized);
                    var v2 = Vector3.ProjectOnPlane(target - parentP.trackTransform.position, twistAxis.normalized);

                    Vector3 vTarget = Vector3.RotateTowards(v1, v2, Time.deltaTime * dynamicTwistRate, 100f);
                    deltaRotation = Quaternion.FromToRotation(v1.normalized, vTarget);
                    currentRot = parentP.trackTransform.rotation;

                    parentP.forwardParticle.targetPos = deltaRotation * parentP.trackTransform.forward;
                    parentP.upParticle.targetPos = deltaRotation * parentP.trackTransform.up;

                    parentP.forwardParticle.currentPos = parentP.forwardParticle.targetPos;
                    parentP.upParticle.currentPos = parentP.upParticle.targetPos;

                    lookRot = Quaternion.LookRotation(parentP.forwardParticle.currentPos, parentP.upParticle.currentPos);
                    {
                        parentP.limit.transform.rotation = lookRot;
                        parentP.particleState.isJointLimited = parentP.limit.Apply(parentP.jointLimitStrength);

                        //revert if additional twist doesn't bring joint closer to target

                        if (Vector3.Distance(GetActualPosition(), target) >= d)
                        {
                            parentP.trackTransform.rotation = currentRot;
                            parentP.forwardParticle.currentPos = parentP.trackTransform.forward;
                            parentP.upParticle.currentPos = parentP.trackTransform.up;
                        }
                        else
                        {
                            parentP.gravityParticle.currentPos = parentP.GetActualPosition();
                        }

                    }
                }

            }
            else
            {
                trackTransform.rotation = lookRot;
            }

            /*
            var stretchedLength = (GetCurrentMassPos() - trackTransform.position).magnitude;
            trackTransform.localPosition = initialLocalPosition;
            var toParent = GetActualPosition() - trackTransform.position;
            float stretch = stretchedLength - boneLength;
            this.trackTransform.position += (stretch * toParent.normalized);
            */
            //trackTransform.rotation = lookRot;
            //gravityParticle.currentPos = GetActualPosition();
        }

        #endregion

        #region Forces
        public Vector3 ComputeLocalForce(Vector3 localForce, float amplitude, float frequencyTime, float frequencyAlongChain, float branchTimeOffset, ExternalForceMode mode, float dt)
        {
            Vector3 localTentacleForce = localForce;
            float value = (frequencyTime * Time.time) + (frequencyAlongChain * freedomFactor) + (branchTimeOffset * (branchID + 1));
            Vector3 localTentacleForceAlongChain = Vector3.zero;
            switch (mode)
            {
                case ExternalForceMode.CONSTANT:
                    localTentacleForceAlongChain = amplitude * localTentacleForce;// * dt;
                    break;
                case ExternalForceMode.SIN_WAVE:
                    localTentacleForceAlongChain = amplitude * localTentacleForce * Mathf.Sin(value) * frequencyTime;// * dt;
                    break;
                case ExternalForceMode.PERLIN_NOISE:
                    float noise = Mathf.PerlinNoise(value, 0f);
                    noise = Mathf.Clamp(noise, 0, 1);
                    localTentacleForceAlongChain = amplitude * localTentacleForce * ((noise * 2) - 1) * frequencyTime;// * dt;
                    break;
                default:
                    break;
            }

            return localTentacleForceAlongChain;
        }

        public void SetupForces(Vector3 gravity, Vector3 externalForces, float massInertia)
        {
            this.gravityForce = gravity;
            this.externalForces = externalForces;

            if (enablePosQuatConstraint)
            {
                this.gravityForce = posQuatConstraint.constraintType == PositionConstraintType.TARGET_FORCE
                    ? (GetConstraintTargetPosition() - GetCurrentParticlePos()) * posQuatConstraint.targetForceStrength
                        : Vector3.zero;
            }

            this.massInertia = massInertia;
        }

        protected Vector3 ComputeInertiaForce(ParticleDynamicsData dynamicParams)
        {
            Vector3 gravLocalSpace = ParticleUtils.TransformPointQuat(gravityDirectionLocal, lastParentPosition, lastRotation);

            var parentTr = trackTransform.parent;
            Quaternion parentRotation = parentTr != null ? parentTr.rotation : Quaternion.identity;

            if (parentTr == rootSpaceTransform && parentTr.parent != null)
                parentRotation = parentTr.parent.rotation * rootInitLocalRotation;

            if (rootSpaceTransform == null)
                return Vector3.zero;
            var rootRotation = rootSpaceTransform.parent != null ? rootSpaceTransform.parent.rotation * rootInitLocalRotation : rootInitLocalRotation;
            Vector3 gravParentSpace = ParticleUtils.TransformPointQuat(gravityInitialRootSpace, rootSpaceTransform.position, rootRotation);

            Vector3 inertiaForce = (gravParentSpace - GetCurrentParticlePos()) * massInertia;
            return inertiaForce;
        }

        private void ResolveParticleForces(ParticleDynamicsData dynamicParams, float dt, float dtOld)
        {
            Vector3 inertiaForce = ComputeInertiaForce(dynamicParams);
            Vector3 finalForce = (gravityForce * dt) + (externalForces * dt) + inertiaForce;
            gravityParticle.UpdateParticlePos(dynamicParams, finalForce, dt, dtOld, freedomFactor, particleState);
        }
        #endregion

        #region Collisions
        public void SetDynamicColliders(List<DynamicCollider> colliders)
        {
            this.dynamicColliders = colliders;
        }

        public void ResolveCustomParticleCollisions()
        {
            var oldPos = gravityParticle.currentPos;
            if (dynamicColliders != null)
            {
                bool collisionDetected;
                foreach (var collider in dynamicColliders)
                {
                    gravityParticle.currentPos = collider.ResolveCollision(gravityParticle.currentPos, massRadius, out collisionDetected);
                    if (collisionDetected)
                    {
                        this.particleState.isColliding = true;
                        this.particleState.bounce = collider.bounce;
                        this.particleState.friction = collider.friction;
                        this.particleState.lastBounceVector = gravityParticle.currentPos - oldPos;
                    }
                }
            }
        }

        public void ResolveLayerCollisions(LayerMask collisionLayer)
        {
            var hitColliders = Physics.OverlapSphere(GetCurrentParticlePos(), massRadius, collisionLayer);
            foreach (var c in hitColliders)
            {
                bool collisionDetected;

                if (c is SphereCollider)
                {
                    var sCol = c as SphereCollider;
                    var pos = c.gameObject.transform.TransformPoint(sCol.center);
                    var radius = sCol.radius * sCol.transform.localScale[0];
                    SetCurrentParticlePos(DynamicCollider.ResolveSphereCollision(pos, radius, GetCurrentParticlePos(), massRadius, out collisionDetected));
                    particleState.isColliding = collisionDetected;
                }
                else if (c is CapsuleCollider)
                {
                    var cCol = c as CapsuleCollider;
                    var direction = c.transform.forward;
                    if (cCol.direction == 0)
                    {
                        direction = c.transform.right;
                    }
                    if (cCol.direction == 1)
                    {
                        direction = cCol.transform.up;
                    }
                    var pos = cCol.gameObject.transform.TransformPoint(cCol.center);
                    var radius = cCol.radius * cCol.transform.localScale[0];
                    var height = cCol.height * (cCol.transform.localScale[0] / 2);
                    SetCurrentParticlePos(DynamicCollider.ResolveCapsuleCollision(pos, radius, height, direction, GetCurrentParticlePos(), massRadius, out collisionDetected));
                    particleState.isColliding = collisionDetected;
                }
                else
                {
                    SetCurrentParticlePos(DynamicCollider.ResolveNativeCollision(c, GetCurrentParticlePos(), massRadius, out collisionDetected));
                }
                particleState.isColliding = collisionDetected;
                particleState.friction = 1f;
            }
        }

        #endregion

        #region Simulation

        public void SimulateForces(ParticleDynamicsData positionDynamics, float dt, float dtOld)
        {
            if (!enableIK)
            {
                ResolveParticleForces(positionDynamics, dt, dtOld);
            }
        }

        public void SetupHierarchy()
        {
            constraints = new List<ParticleRelativeConstraint>();
            if (parentParticle != null)
            {
                AddDistanceConstraint(parentParticle, boneLength);
                parentParticle.AddDistanceConstraint(this, boneLength);
            }
        }

        public void ResolveDistanceConstraints(ParticleDynamicsData dynamicParams)
        {
            if (parentParticle == null)
            {
                var toParent = GetCurrentParticlePos() - this.trackTransform.position;
                this.SetCurrentParticlePos(this.trackTransform.position + toParent.normalized * boneLength);
            }

            if (enablePosQuatConstraint && posQuatConstraint.constraintType != PositionConstraintType.TARGET_FORCE)
            {
                //position constraint
                gravityParticle.currentPos = gravityParticle.oldPos = GetConstraintTargetPosition();
            }
            else
            {
                // distance constraint
                int numConstraints = constraints.Count;
                float factor = 1f / (float)numConstraints;
                for (int i = 0; i < numConstraints; ++i)
                {
                    var constraint = constraints[i];
                    var neigbor = constraints[i].otherParticle;
                    Vector3 delta = GetCurrentParticlePos() - neigbor.GetCurrentParticlePos();
                    float error = delta.magnitude;

                    float boneLength = constraints[i].constraintValue;

                    if (constraint.constraintType == ParticleConstraintType.MIN_DISTANCE && error > boneLength) return;
                    if (constraint.constraintType == ParticleConstraintType.MAX_DISTANCE && error < boneLength) return;

                    float rd1 = freedomFactor;
                    float rd2 = constraint.otherParticle.freedomFactor;
                    rd1 = 1f;
                    rd2 = 1f;
                    float weight = 1f / (float)numConstraints;
                    if (rd1 + rd2 > 0) weight *= Mathf.Pow(rd1 / (rd1 + rd2), 2);
                    //weight = 0.5f * 0.5f;

                    float stretchiness = dynamicParams.Stretchiness;
                    float stiffness = dynamicParams.Stiffness;
                    float constraintDistanceSq = (boneLength * boneLength) * (stretchiness * stretchiness);
                    float stretch = -stiffness *
                        ((2.0f * constraintDistanceSq / ((error * error) + constraintDistanceSq)) - 1.0f);

                    delta *= stretch;

                    gravityParticle.currentPos -= delta * weight;
                }
            }
        }

        public void DisablePosQuatConstraints()
        {
            this.enablePosQuatConstraint = false;
            this.enableIK = false;
            this.posQuatConstraint.constraintType = PositionConstraintType.INVERSE_KINEMATICS;
        }

        public void Constrain(PositionConstraintType constraintType, Transform targetTransform, float strength = 1)
        {
            this.enablePosQuatConstraint = true;
            this.posQuatConstraint.constraintType = constraintType;
            this.posQuatConstraint.targetTr = targetTransform;
            this.posQuatConstraint.targetForceStrength = strength;
        }

        public void Constrain(PositionConstraintType constraintType, Vector3 target, float strength = 1)
        {
            this.enablePosQuatConstraint = true;
            this.posQuatConstraint.constraintType = constraintType;
            this.posQuatConstraint.targetPos = target;
            this.posQuatConstraint.targetTr = null;
            this.posQuatConstraint.targetForceStrength = strength;
        }

        public void AlignTransforms(bool solveTwist = false, bool preventStretch = true)
        {
            var target = GetCurrentParticlePos();
            AlignTransformToTarget(target, solveTwist);
            if (!preventStretch)
                ResolveStretch();

            if (childParticle != null || preventStretch)
                gravityParticle.currentPos = GetActualPosition();
        }

        public override void Simulate(ParticleDynamicsData positionDynamics, ParticleDynamicsData rotationDynamics, float dt, float dtOld = 0)
        {
            SimulateForces(positionDynamics, dt, dtOld);
            ResolveCustomParticleCollisions();
            ApplyDistanceConstraint();
            AlignTransforms(true);
        }
        #endregion
    }
}
