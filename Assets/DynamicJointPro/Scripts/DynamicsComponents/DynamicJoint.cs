using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    public enum DynamicJointSimulationMode
    {
        SPATIAL_RELATIONS,
        GRAVITY
    }

    public enum SpatialRelationsMode
    {
        PARENT,
        CUSTOM_POINTS,
        CUSTOM_POINTS_TRIANGULATED,
    }

    public class DynamicJoint : MonoBehaviour
    {
        #region Variables
        [HideInInspector]
        public List<DynamicJointParticle> particles;

        [Header("Source")]
        public List<Transform> sources;

        [Header("Position Dynamics")]
        public bool usePositionDynamics = true;
        public ParticleDynamicsData positionDynamics;

        [Header("Rotation Dynamics")]
        public bool useRotationDynamics = true;
        public ParticleDynamicsData rotationDynamics;

        [Header("Limits")]
        public bool useLimits = false;
        public bool useRotationLimits = true;
        public bool usePositionLimits = true;

        [Header("Position Limits")]
        public Vector3 minPositionLimits;
        public Vector3 maxPositionLimits;

        [Header("Forces")]
        public Vector3 gravity = Vector3.down;
        public float massInertia = 0;

        [Header("Simulation Mode")]
        public DynamicJointSimulationMode simulationMode = DynamicJointSimulationMode.SPATIAL_RELATIONS;

        [Header("Spatial Relations")]
        public SpatialRelationsMode targetRelativeTo = SpatialRelationsMode.PARENT;
        public Vector3 offsetEulers;
        public RigidSpatialRelations customRelativePoints;

        [HideInInspector]
        public DynamicForcesComponent externalForces;
        #endregion

        public void Setup(Transform sourceTransform, List<Transform> spatialRelationPoints, ParticleDynamicsData positionDynamics = null, ParticleDynamicsData rotationDynamics = null)
        {
            this.sources = new List<Transform>() { sourceTransform };
            this.customRelativePoints = new RigidSpatialRelations(spatialRelationPoints);
            this.positionDynamics = positionDynamics;
            this.rotationDynamics = rotationDynamics;
        }

        public void Setup(List<Transform> sourceTransforms, List<Transform> spatialRelationPoints, ParticleDynamicsData positionDynamics = null, ParticleDynamicsData rotationDynamics = null)
        {
            this.sources = sourceTransforms;
            this.customRelativePoints = new RigidSpatialRelations(spatialRelationPoints);
            this.positionDynamics = positionDynamics;
            this.rotationDynamics = rotationDynamics;
        }

        public void Initialize()
        {
            if (particles != null)
            {
                foreach (var p in particles)
                    DestroyImmediate(p);
            }

            particles = new List<DynamicJointParticle>();

            if (sources == null) sources = new List<Transform>();
            if (sources.Count == 0)
                sources.Add(this.transform);

            for (int i = 0; i < sources.Count; ++i)
            {
                Transform s = sources[i];
                var particleComps = s.GetComponents<ParticleRotatable>();
                foreach (var p in particleComps)
                    DestroyImmediate(p);
                DynamicJointParticle particle = s.gameObject.AddComponent<DynamicJointParticle>();
                particle.Initialize(s, s.parent, true, -1f, false);
                particles.Add(particle);
            }

            UpdateLimits();

            SetupSpatialRelations();

            this.externalForces = GetComponent<DynamicForcesComponent>();

            //setup parent hierarchy
            foreach (var p in particles)
            {
                var parentP = p.trackTransform;
                bool parentFound = false;
                while (parentP != null && !parentFound)
                {
                    parentP = parentP.parent;
                    foreach (var p2 in particles)
                    {
                        if (p2 != p && p2.trackTransform == parentP)
                        {
                            p.parentParticle = p2;
                            p2.childParticle = p;
                            p2.AutoSetGravityDirection();
                            parentFound = true;
                            break;
                        }
                    }
                }
            }
        }

        public void SetupSpatialRelations()
        {
            customRelativePoints.TriangulateSpatialRelations();
        }

        public void UpdateLimits()
        {
            foreach (DynamicJointParticle particle in particles)
            {
                particle.SetPositionLimits(minPositionLimits, maxPositionLimits);
            }
        }

        public void Simulate(float deltaTime)
        {
            UpdateLimits();

            for (int i = 0; i < particles.Count; ++i)
            {
                DynamicJointParticle particle = particles[i];
                particle.useLimits = this.useLimits;
                particle.usePositionLimits = this.usePositionLimits;
                particle.useRotationLimits = this.useRotationLimits;
                particle.offsetEulers = this.offsetEulers;

                if (simulationMode == DynamicJointSimulationMode.SPATIAL_RELATIONS)
                {
                    if (particle == null) continue;

                    if (targetRelativeTo == SpatialRelationsMode.PARENT || customRelativePoints == null || customRelativePoints.relativePoints.Count == 0)
                    {
                        particle.RetargetToParent(particle.trackTransform.parent);
                    }
                    else if (customRelativePoints != null && customRelativePoints.relativePoints.Count >= 3 && targetRelativeTo == SpatialRelationsMode.CUSTOM_POINTS_TRIANGULATED)
                    {
                        particle.RetargetToRelativeTriangulation(customRelativePoints.GetInitialPositions(), customRelativePoints.GetCurrentPositions(), customRelativePoints.GetTriangulation());
                    }
                    else
                    {
                        particle.RetargetToRelativePoints(customRelativePoints.GetInitialPositions(), customRelativePoints.GetCurrentPositions(), customRelativePoints.GetInitialQuats(), customRelativePoints.GetCurrentQuats());
                    }
                    particle.Simulate(deltaTime > 0 && usePositionDynamics ? positionDynamics : null, deltaTime > 0 && useRotationDynamics ? rotationDynamics : null, deltaTime);
                }
                else
                {
                    Vector3 externalForce = externalForces != null ? externalForces.ComputeExternalForces(particle, particle.trackTransform, particle.trackTransform) : Vector3.zero;
                    particle.SimulateGravity(gravity + externalForce, massInertia, deltaTime > 0 && usePositionDynamics ? positionDynamics : null, deltaTime > 0 && useRotationDynamics ? rotationDynamics : null, deltaTime);
                }

                particle.trackTransform.position = particle.outPosition;
                particle.trackTransform.rotation = particle.outRotation;

                particle.gravityCurrentPos = particle.trackTransform.TransformDirection(particle.gravityDirection) + particle.trackTransform.position;
                particle.gravitySecondaryCurrentPos = particle.trackTransform.TransformDirection(particle.gravitySecondaryDirection) + particle.trackTransform.position;

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (particles == null || particles.Count == 0)
                Initialize();
            else
            {
                foreach (var p in particles)
                    p.RefreshProperties();
            }

            this.externalForces = GetComponent<DynamicForcesComponent>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (particles == null || particles.Count == 0) return;
            Simulate(Time.deltaTime);
        }

    }
}