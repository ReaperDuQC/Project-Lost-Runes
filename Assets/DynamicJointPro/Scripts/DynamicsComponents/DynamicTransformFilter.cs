using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicTransformFilter : MonoBehaviour
    {
        #region Variables
        private DynamicJointParticle particle;

        [Header("Target")]
        public Transform target;

        [Header("Toggle Dynamics")]
        public bool enableDynamics = true;

        [Header("Position Dynamics")]
        public bool usePositionDynamics = true;
        public ParticleDynamicsData positionDynamics;

        [Header("Rotation Dynamics")]
        public bool useRotationDynamics = true;
        public ParticleDynamicsData rotationDynamics;

        [Header("Limits")]
        public bool useLimits = false;
        public bool usePositionLimits = true;
        public bool useRotationLimits = true;

        [Header("Position Limits")]
        public Vector3 minPositionLimits;
        public Vector3 maxPositionLimits;
        #endregion

        public void Initialize()
        {
            if (particle != null)
                DestroyImmediate(particle);

            particle = this.gameObject.AddComponent<DynamicJointParticle>();
            //particle = new ParticleRigidSpring();
            particle.Initialize(transform, transform.parent, false, -1f, false);
            UpdateLimits();
        }

        public void UpdateLimits()
        {
            if (particle != null)
            {
                particle.SetPositionLimits(minPositionLimits, maxPositionLimits);
                particle.useLimits = this.useLimits;
            }

            particle.usePositionLimits = this.usePositionLimits;
            particle.useRotationLimits = this.useRotationLimits;
        }

        public void Simulate(float deltaTime)
        {
            UpdateLimits();
            particle.SetParticleLimitsFrame(target.forward, target.up);
            particle.UpdateParticleTargets(target.position, target.forward, target.up);

            particle.Simulate(deltaTime > 0 && enableDynamics && usePositionDynamics ? positionDynamics : null,
                deltaTime > 0 && enableDynamics && useRotationDynamics ? rotationDynamics : null, deltaTime);

            particle.trackTransform.position = particle.outPosition;
            particle.trackTransform.rotation = particle.outRotation;
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (particle == null) return;
            Simulate(Time.deltaTime);
        }
    }
}
