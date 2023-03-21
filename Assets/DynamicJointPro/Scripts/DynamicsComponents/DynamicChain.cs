using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    public class DynamicChain : MonoBehaviour
    {
        #region Variables
        [Header("Source Transforms")]
        public List<Transform> sources;
        public string includeTag = "";
        public string excludeTag = "";

        [HideInInspector]
        public List<DynamicChainParticle> particles;

        [Header("Dynamics")]
        public ParticleInitializationMode initializationFrom = ParticleInitializationMode.LAST_FRAME_CONTINUOUS;
        public int numIterations = 3;
        public ParticleDynamicsChainData dynamicProperties;
        public Vector3 gravity = Vector3.down;
        private float dtOld = 0;

        [Header("Mass Inertia")]
        public float inertia = 0;

        [Header("Limits")]
        public bool useJointLimits = true;
        [Range(0, 1)]
        public float jointLimitStrength = 1;
        public bool solveTwistDynamics = true;
        public float maxTwistRate = 30f;

        [Header("IK")]
        public bool enableIK = false;
        public Transform IKTarget = null;
        public bool strictIK = false;
        //public bool constrainEndOrientation = false;

        [Header("Colliders")]
        public float particleRadius = 0.5f;
        public bool enableCollisions = true;
        public List<Collider> nativeColliders;
        public List<DynamicCollider> dynamicColliders;

        [Header("Collision Against Layer")]
        public bool enableCollisionAgainstLayer = false;
        public LayerMask collisionLayer = 0;

        [Header("External / Local Forces")]
        [HideInInspector]
        public DynamicForcesComponent externalForcesComponent;

        [Range(0, 1)]
        [HideInInspector]
        public float forcesStrength = 1;
        [Range(0, 1)]
        [HideInInspector]
        public float forcesStartWeight = 1;
        [Range(0, 1)]
        [HideInInspector]
        public float forcesEndWeight = 1;
        [HideInInspector]
        public Transform targetSpaceTransform;

        private int numBranches = 0;
        private int numParticles = 0;

        //public List<float> maxDistanceToRoot;

        [HideInInspector]
        public List<DynamicChainParticle> rootParticles;
        [HideInInspector]
        public List<DynamicChainParticle> endParticles;

        private int selectedParticleIndex = -1;

        #endregion

        #region Initialization
        void Start()
        {
            if (this.particles == null || this.particles.Count == 0)
                InitializeParticles();

            ResolveDynamicForcesComponent();
        }

        public void ResetParticles()
        {
            this.numBranches = 0;
            this.numParticles = 0;
            if (particles != null)
            {
                foreach (var p in particles)
                    DestroyImmediate(p);
            }

            particles = new List<DynamicChainParticle>();

            rootParticles = new List<DynamicChainParticle>();
            endParticles = new List<DynamicChainParticle>();

            if (sources == null) sources = new List<Transform>();
        }

        public void SetSelectedParticle(int index)
        {
            selectedParticleIndex = index;
        }

        public int GetSelectedParticle()
        {
            return selectedParticleIndex;
        }

        public void InitializeParticles()
        {
            ResetParticles();
            if (sources.Count == 0)
                sources.Add(this.transform);

            foreach (Transform source in sources)
            {
                numBranches++;
                AddParticlesRecursively(null, source, source, source);
                endParticles.Add(particles.Last());
            }

            foreach (DynamicChainParticle particle in particles)
            {
                particle.freedomFactor = particle.distanceToRoot / GetEndParticle(particle).distanceToRoot;
                particle.useRotationLimits = true;
                particle.usePositionLimits = false;
                particle.SetupHierarchy();
            }

            UpdateColliders();

            ResolveDynamicForcesComponent();
        }

        public DynamicChainParticle GetEndParticle(DynamicChainParticle particle)
        {
            return endParticles[particle.branchID];
        }

        public void ComputeRadii(AnimationCurve radiusCurve)
        {
            float maxDistanceToRoot = 0;
            foreach (DynamicChainParticle p in particles)
            {
                maxDistanceToRoot = Mathf.Max(maxDistanceToRoot, p.distanceToRoot);
            }
            foreach (DynamicChainParticle p in particles)
            {
                p.massRadius = particleRadius * radiusCurve.Evaluate(p.distanceToRoot / maxDistanceToRoot);
            }
        }

        public void ResolveDynamicForcesComponent()
        {
            externalForcesComponent = this.gameObject.GetComponent<DynamicForcesComponent>();
        }

        public void AddParticlesRecursively(DynamicChainParticle parent, Transform parentTr, Transform searchTr, Transform root)
        {
            int branchID = numBranches - 1;
            for (int i = 0; i < searchTr.childCount; ++i)
            {
                var child = searchTr.GetChild(i);
                bool canInclude = string.IsNullOrEmpty(includeTag) || child.CompareTag(includeTag);
                bool shouldExclude = !string.IsNullOrEmpty(excludeTag) && (child.CompareTag(excludeTag) || child.tag.Contains(excludeTag));

                if (canInclude && !shouldExclude)
                {
                    var particleComponents = parentTr.GetComponents<DynamicChainParticle>();
                    foreach (var p in particleComponents)
                        DestroyImmediate(p);

                    DynamicChainParticle particle = parentTr.gameObject.AddComponent<DynamicChainParticle>();
                    particle.Initialize(parentTr, parent != null ? parent.trackTransform : parentTr.parent, false, -1f, false);
                    Vector3 gravityDirection = child != null ? child.position - parentTr.position : Vector3.forward;
                    particle.SetupGravityParticle(gravityDirection, particles.Count > 0 ? particles[0] : null);
                    particle.branchID = branchID;
                    particle.dynamicTwistRate = this.maxTwistRate;
                    particle.particleIndex = numParticles++;
                    particle.gravityForce = gravity;
                    particle.massInertia = this.inertia;
                    particle.useGravity = true;
                    particle.dynamicTwistRate = this.maxTwistRate;
                    particle.lastRotation = parentTr.rotation;
                    particle.lastParentPosition = parentTr.position;

                    particle.EncodeInRootSpace(root);
                    particle.distanceToRoot = particle.boneLength + (parent != null ? parent.distanceToRoot : 0);
                    //maxDistanceToRoot = Mathf.Max(particle.distanceToRoot, maxDistanceToRoot);

                    particles.Add(particle);

                    if (parent == null)
                    {
                        rootParticles.Add(particle);
                    }
                    else
                    {
                        particle.parentParticle = parent;
                        parent.childParticle = particle;
                        parent.childTransform = particle.trackTransform;
                    }
                    if (child.childCount > 0)
                    {
                        AddParticlesRecursively(particle, child, child, root);
                    }
                }
                else
                {
                    AddParticlesRecursively(parent, parentTr, child, root);
                }
            }
        }
        #endregion

        #region Simulation

        public void Simulate(float deltaTime)
        {
            UpdateColliders();

            for (int i = 0; i < particles.Count; ++i)
            {
                DynamicChainParticle particle = particles[i];
                if (particle == null) continue;

                switch (initializationFrom)
                {
                    case ParticleInitializationMode.STATIC_POSE:
                        particle.ResetToInitialStaticRotation();
                        break;
                    case ParticleInitializationMode.SOURCE_ANIMATION:
                        particle.UpdateInertiaTargetsFromAnimation();
                        break;
                    case ParticleInitializationMode.LAST_FRAME_CONTINUOUS:
                    default:
                        particle.ResetToLastFrameOrientation();
                        break;
                }

                particle.trackTransform.localPosition = particle.initialLocalPosition;
                particle.jointLimitStrength = this.jointLimitStrength;
                particle.SetUseLimits(this.useJointLimits && particle.limit != null && particle.limit.enable);
                if (enableIK && strictIK) continue;
                particle.SetupForces(gravity, ComputeExternalForces(particle), inertia);
                particle.dynamicTwistRate = this.maxTwistRate;
                particle.SetDynamicColliders(dynamicColliders);

                particle.SimulateForces(deltaTime > 0 ? dynamicProperties : null, this.dtOld, deltaTime);
            }

            for (int j = 0; j < numIterations; ++j)
            {
                for (int i = 0; i < particles.Count; ++i)
                {
                    DynamicChainParticle particle = particles[i];
                    particle.ResolveDistanceConstraints(dynamicProperties);
                    if (enableCollisions)
                    {
                        particle.particleState.isColliding = false;
                        particle.ResolveCustomParticleCollisions();
                        if (enableCollisionAgainstLayer)
                            particle.ResolveLayerCollisions(collisionLayer);
                    }
                }
            }

            if (enableIK)
                StretchTowardIKTarget();

            AlignTransforms();

            ResolveIK();

            foreach (DynamicChainParticle particle in particles)
            {
                particle.lastRotation = particle.trackTransform.rotation;
                particle.lastParentPosition = particle.trackTransform.position;
            }

            this.dtOld = deltaTime;
        }

        public void AlignTransforms()
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                DynamicChainParticle particle = particles[i];
                bool solveTwist = solveTwistDynamics && !endParticles[particle.branchID].enablePosQuatConstraint;
                particle.AlignTransforms(solveTwist, dynamicProperties.preventStretch);
            }
        }

        bool simulateFlag = false;
        private void FixedUpdate()
        {
            if (particles == null || particles.Count == 0) return;
            simulateFlag = true;
        }

        void LateUpdate()
        {
            //Simulate(Time.deltaTime);
            //return;
            if (particles == null || particles.Count == 0) return;
            if (simulateFlag)
            {
                Simulate(Time.fixedDeltaTime);
                simulateFlag = false;
            }
            else
            {
                foreach (DynamicChainParticle particle in particles)
                {
                    particle.trackTransform.rotation = particle.lastRotation;
                    particle.trackTransform.position = particle.lastParentPosition;
                }
            }
        }

        #endregion

        #region Colliders

        public void UpdateColliders()
        {
            for (int i = 0; i < nativeColliders.Count; ++i)
            {
                var coll = nativeColliders[i];
                if (coll == null) continue;
                var nativeColl = coll.GetComponent<DynamicColliderNative>();
                if (nativeColl == null)
                {
                    nativeColl = coll.gameObject.AddComponent<DynamicColliderNative>();
                }
                nativeColl.nativeCollider = coll;
                dynamicColliders.Add(nativeColl);
                nativeColliders.Remove(coll);
                i--;
            }

            foreach (var c in dynamicColliders)
            {
                if (c != null)
                {
                    c.UpdateCollider();
                }
            }
        }

        #endregion

        #region External Forces
        public Vector3 ComputeExternalForces(DynamicChainParticle particle)
        {
            if (externalForcesComponent == null) return Vector3.zero;
            return externalForcesComponent.ComputeExternalForces(particle, rootParticles[particle.branchID].trackTransform, rootParticles[particle.branchID].GetParentTransform());
        }

        public Vector3 GetForceInTargetSpace(Vector3 force, TargetSpaceExternalForces space, DynamicChainParticle particle, Transform targetTransformSpace = null)
        {
            switch (space)
            {
                case TargetSpaceExternalForces.WORLD_SPACE:
                    return force;
                case TargetSpaceExternalForces.TARGET_TRANSFORM_SPACE:
                    if (targetSpaceTransform != null)
                    {
                        return targetSpaceTransform.TransformDirection(force);
                    }
                    break;
                case TargetSpaceExternalForces.ROOT_PARTICLE_SPACE:
                    if (rootParticles[particle.branchID].trackTransform != null)
                    {
                        return rootParticles[particle.branchID].trackTransform.TransformDirection(force);
                    }
                    break;
                case TargetSpaceExternalForces.ROOT_PARTICLE_PARENT_SPACE:
                    if (rootParticles[particle.branchID].GetParentTransform() != null)
                    {
                        return rootParticles[particle.branchID].GetParentTransform().TransformDirection(force);
                    }
                    break;
                case TargetSpaceExternalForces.PER_PARTICLE_SPACE:
                    if (particle.trackTransform != null)
                    {
                        return particle.trackTransform.TransformDirection(force);
                    }
                    break;
            }

            return force;
        }
        #endregion

        #region Inverse Kinematics
        public void ResolveIK()
        {
            if (enableIK)
            {
                CCD_Step();
            }
        }

        public void CCD_Step()
        {
            if (endParticles != null)
            {
                // Forward Rotations
                SolveSwingIKAlongChain(false);

                // Backward Rotations
                SolveSwingIKAlongChain(true);
            }
        }

        private void StretchTowardIKTarget()
        {
            foreach (var endParticle in endParticles)
            {
                if (!endParticle.enablePosQuatConstraint) continue;
                Vector3 targetPosition = endParticle.GetConstraintTargetPosition();

                var rootIKNode = rootParticles[endParticle.branchID];

                var node = endParticle;
                endParticle.SetCurrentParticlePos(targetPosition);

                while (node.parentParticle != null)
                {
                    var parent = node.parentParticle;
                    if (parent.enablePosQuatConstraint)
                    {
                        node = parent;
                        continue;
                    }

                    if (node == endParticle && node.posQuatConstraint.constrainRotation)
                    {
                        parent.gravityParticle.currentPos = targetPosition - ((endParticle.posQuatConstraint.targetTr.rotation * endParticle.gravityDirectionLocal).normalized * endParticle.boneLength);
                    }
                    else
                    {
                        ResolveFABRIKStretch(parent, node, node.boneLength, node.boneLength, node.boneLength);
                    }
                    //var delta = parent.GetCurrentParticlePos() - node.GetCurrentParticlePos();
                    //parent.SetCurrentParticlePos(node.GetCurrentParticlePos() + (delta.normalized * node.boneLength));

                    node = parent;
                    //if (node == rootIKNode) break;
                }
            }
        }

        private void ResolveFABRIKStretch(DynamicChainParticle targetJoint, DynamicChainParticle relativeJoint, float boneLength, float minBoneLength, float maxBoneLength)
        {
            Vector3 toRelative = targetJoint.GetCurrentParticlePos() - relativeJoint.GetCurrentParticlePos();
            if (toRelative.magnitude > maxBoneLength)
            {
                targetJoint.SetCurrentParticlePos(relativeJoint.GetCurrentParticlePos() + toRelative.normalized * maxBoneLength);
            }
            else if (toRelative.magnitude < minBoneLength)
            {
                targetJoint.SetCurrentParticlePos(relativeJoint.GetCurrentParticlePos() + toRelative.normalized * minBoneLength);
            }
        }

        private void SolveSwingIKAlongChain(bool rootToEnd = true, bool updateParticlePositions = true)
        {
            if (rootToEnd)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    DynamicChainParticle node = particles[i];

                    var endParticle = endParticles[node.branchID];
                    if (endParticle.posQuatConstraint != null &&
                        (!endParticle.enablePosQuatConstraint || endParticle.posQuatConstraint.constraintType != PositionConstraintType.INVERSE_KINEMATICS))
                        continue;
                    //if (endParticles[node.branchID].particleIndex >= node.particleIndex) continue;
                    var IKTargetTr = endParticles[node.branchID].posQuatConstraint.targetTr != null ?
                        endParticles[node.branchID].posQuatConstraint.targetTr : IKTarget;
                    if (IKTargetTr == null)
                        continue;

                    if (node.particleIndex > endParticles[node.branchID].particleIndex) continue;
                    if (node.particleIndex == endParticles[node.branchID].particleIndex && node.posQuatConstraint.constrainRotation)
                    {
                        ConstrainEndOrientation(node, IKTargetTr.rotation);
                        continue;
                    }

                    //if (node == endParticle.parentParticle || !endParticle.posQuatConstraint.constrainRotation)
                    {
                        node.AlignTransformToTarget(endParticles[node.branchID].GetActualPosition(), IKTargetTr.position);
                    }
                    /*
                    else
                    {
                        var target = IKTargetTr.position - ((endParticle.posQuatConstraint.targetTr.rotation * endParticle.gravityDirectionLocal).normalized * endParticle.boneLength);
                        node.AlignTransformToTarget(endParticles[node.branchID].parentParticle.GetActualPosition(), target);
                    }
                    */
                    //if (updateParticlePositions)
                    //    UpdateParticleError(node);
                }
            }
            else
            {
                for (int i = particles.Count - 1; i > 0; i--)
                {
                    DynamicChainParticle node = particles[i];

                    var endParticle = endParticles[node.branchID];
                    if (endParticle.posQuatConstraint != null &&
                        (!endParticle.enablePosQuatConstraint || endParticle.posQuatConstraint.constraintType != PositionConstraintType.INVERSE_KINEMATICS))
                        continue;
                    //if (endParticles[node.branchID].particleIndex >= node.particleIndex) continue;
                    var IKTargetTr = endParticles[node.branchID].posQuatConstraint.targetTr != null ?
                        endParticles[node.branchID].posQuatConstraint.targetTr : IKTarget;
                    if (IKTargetTr == null)
                        continue;

                    if (node.particleIndex > endParticles[node.branchID].particleIndex) continue;
                    if (node.particleIndex == endParticles[node.branchID].particleIndex && node.posQuatConstraint.constrainRotation)
                    {
                        ConstrainEndOrientation(node, IKTargetTr.rotation);
                        continue;
                    }

                    //if (node == endParticle.parentParticle || !endParticle.posQuatConstraint.constrainRotation)
                    {
                        node.AlignTransformToTarget(endParticles[node.branchID].GetActualPosition(), IKTargetTr.position);
                    }
                    /*
                    else
                    {
                        var target = IKTargetTr.position - ((endParticle.posQuatConstraint.targetTr.rotation * endParticle.gravityDirectionLocal).normalized * endParticle.boneLength);
                        node.AlignTransformToTarget(endParticles[node.branchID].parentParticle.GetActualPosition(), target);
                    }
                    */

                    //if (updateParticlePositions)
                    //    UpdateParticleError(node);
                }
            }

        }

        /*
        private void SolveSwingIKAlongChain(bool rootToEnd = true, bool updateParticlePositions = true)
        {
            if (rootToEnd)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    ParticleJoint node = particles[i];
                    var endParticle = endParticles[node.branchID];
                    if (endParticles[node.branchID].particleIndex <= node.particleIndex) continue;
                    if (endParticle.posQuatConstraint != null && 
                        (!endParticle.enablePosQuatConstraint || endParticle.posQuatConstraint.constraintType != PositionConstraintType.INVERSE_KINEMATICS))
                            continue;
                    var IKTargetTr = endParticles[node.branchID].posQuatConstraint.targetTr != null ?
                        endParticles[node.branchID].posQuatConstraint.targetTr : IKTarget;
                    if (IKTargetTr == null)
                        continue;
                    //if (node == endParticles[node.branchID] && constrainEndOrientation)
                   // {
                   //     if (constrainEndOrientation)
                   //         ConstrainEndOrientation(node);
                   //     break;
                        //continue;
                   // }

                    node.AlignTransformToTarget(endParticles[node.branchID].GetActualPosition(), IKTargetTr.position);
                }
            }
            else
            {
                for (int i = particles.Count - 1; i > 0; i--)
                {
                    ParticleJoint node = particles[i];
                    var endParticle = endParticles[node.branchID];
                    if (endParticle.posQuatConstraint != null &&
                        (!endParticle.enablePosQuatConstraint || endParticle.posQuatConstraint.constraintType != PositionConstraintType.INVERSE_KINEMATICS))
                            continue;
                    if (endParticles[node.branchID].particleIndex >= node.particleIndex) continue;
                    var IKTargetTr = endParticles[node.branchID].posQuatConstraint.targetTr != null ?
                        endParticles[node.branchID].posQuatConstraint.targetTr: IKTarget;
                    if (IKTargetTr == null)
                        continue;
                    //if (node == endParticles[node.branchID])
                   // {
                   //     if (constrainEndOrientation)
                   //     {
                   //         ConstrainEndOrientation(node);
                            //continue;
                   //     }
                   //     break;
                   // }
                    node.AlignTransformToTarget(endParticles[node.branchID].GetActualPosition(), IKTargetTr.position);
                }
            }

        }
        */

        public void DisablePositionConstraint(DynamicChainParticle particle)
        {
            if (particle.enablePosQuatConstraint)
            {
                particle.DisablePosQuatConstraints();
                particle.freedomFactor = particle.distanceToRoot / GetEndParticle(particle).distanceToRoot;
                float maxDistanceToRoot = GetEndParticle(particle).distanceToRoot;
                var parentP = particle.parentParticle;
                while (parentP != null)
                {
                    parentP.freedomFactor = particle.distanceToRoot / maxDistanceToRoot;
                    parentP = parentP.parentParticle;
                }
            }
        }

        public void SetParticlePositionConstraint(DynamicChainParticle particle, PositionConstraintType constraint)
        {
            particle.enablePosQuatConstraint = true;
            particle.posQuatConstraint.constraintType = constraint;

            //recompute freedom factors
            particle.freedomFactor = 0;
            var parentP = particle.parentParticle;
            float distanceToEnd = particle.boneLength;
            while (parentP != null)
            {
                distanceToEnd += parentP.boneLength;
                float fraction = distanceToEnd / (distanceToEnd + parentP.distanceToRoot);
                if (fraction > 0.5f)
                    fraction = 1f - fraction;
                parentP.freedomFactor = fraction * 2;
                parentP = parentP.parentParticle;
            }
        }

        public void SetIKEndEffector(DynamicChainParticle particle)
        {
            if (particle == endParticles[particle.branchID])
            {
                this.enableIK = true;
                if (particle.posQuatConstraint.targetTr == null)
                    particle.posQuatConstraint.targetTr = this.IKTarget;
                return;
            }

            DisablePositionConstraint(endParticles[particle.branchID]);
            var target = endParticles[particle.branchID].posQuatConstraint.targetTr;
            if (target == null)
            {
                target = this.IKTarget;
            }
            endParticles[particle.branchID] = particle;
            if (particle.posQuatConstraint.targetTr == null)
                particle.posQuatConstraint.targetTr = target;

            this.enableIK = true;
        }
        #endregion

        #region IK End orientation
        public void ConstrainEndOrientation(DynamicChainParticle particle, Quaternion rotation)
        {
            particle.trackTransform.rotation = rotation;

            if (particle.limit != null)
            {
                particle.limit.Apply(1f);
                //particle.gravityParticle.currentPos = particle.GetActualPosition();
            }
        }
        #endregion
    }
}