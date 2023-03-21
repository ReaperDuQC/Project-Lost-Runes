using UnityEngine;

namespace AshqarApps.DynamicJoint
{

    [System.Serializable]
    public abstract class ParticleRotatable : MonoBehaviour
    {
        #region Variables
        // Source transform hierarchy
        public Transform trackTransform;
        public Transform parentTransform;
        public int particleIndex;
        public int branchID;
        public float freedomFactor = 1f;

        //mass
        public float massRadius = 0.1f;
        public float mass = 1f;
        public bool useWeightShift = false;

        // Initial position config
        public Vector3 initialLocalPosition;
        public Vector3 initialWorldPosition;

        // Initial rotation config
        public Quaternion initialLocalOrientation;
        public Vector3 initialForward;
        public Vector3 initialUp;
        public Vector3 initialRight;

        // Limits
        public bool useLimits = false;
        public bool usePositionLimits = false;
        public bool useRotationLimits = false;

        // Positional Limits
        public Vector3 minPositionLimits;
        public Vector3 maxPositionLimits;

        // Rotational limits
        public Vector2 pitchLimits;
        public Vector2 rollLimits;
        public Vector2 attitudeLimits;

        // position particle
        public ParticlePoint positionParticle;

        // orientation particles
        public ParticlePoint forwardParticle;
        public ParticlePoint upParticle;

        public ParticleFrame parentFrame;
        public ParticleFrame localFrame;

        // simulation outputs
        public Vector3 outPosition;
        public Quaternion outRotation;
        #endregion

        #region Initialization

        public virtual void Initialize(Transform trackTransform, Transform parentFrame = null, bool useWeightShift = false, float teleportationTolerance = -1f, bool simulateInParentSpace = true)
        {
            this.trackTransform = trackTransform;
            this.positionParticle = new ParticlePoint(trackTransform.position, trackTransform, parentFrame, teleportationTolerance, simulateInParentSpace: false);
            this.forwardParticle = new ParticlePoint(trackTransform.forward, trackTransform, parentFrame, simulateInParentSpace: false);
            this.upParticle = new ParticlePoint(trackTransform.up, trackTransform, parentFrame, simulateInParentSpace: false);

            //this.useWeightShift = useWeightShift;
            this.useWeightShift = false;
            if (this.useWeightShift)
            {
                forwardParticle.currentPos += trackTransform.position;
                upParticle.currentPos += trackTransform.position;
            }

            // cache initial position and orientation
            initialWorldPosition = trackTransform.position;
            initialLocalPosition = trackTransform.localPosition;
            initialLocalOrientation = trackTransform.localRotation;

            initialForward = trackTransform.forward;
            initialUp = trackTransform.up;
            initialRight = trackTransform.right;

            parentTransform = trackTransform.parent;

            if (parentTransform != null)
                this.parentFrame = new ParticleFrame(parentTransform.position, parentTransform.rotation);

            this.localFrame = new ParticleFrame(trackTransform.position, trackTransform.localRotation);

        }
        #endregion

        #region Setters
        public void SetUseLimits(bool useLimits)
        {
            this.useLimits = useLimits;
        }

        public void SetPositionLimits(Vector3 minLimits, Vector3 maxLimits)
        {
            this.minPositionLimits = minLimits;
            this.maxPositionLimits = maxLimits;
            this.useLimits = true;
        }

        public void SetRotationLimits(Vector2 pitchLimitsDegrees, Vector2 rollLimitsDegrees, Vector2 attitudeLimitsDegrees)
        {
            this.pitchLimits = new Vector2(pitchLimitsDegrees.x / 180f, pitchLimitsDegrees.y / 180f);
            this.rollLimits = new Vector2(rollLimitsDegrees.x / 180f, rollLimitsDegrees.y / 180f);
            this.attitudeLimits = new Vector2(attitudeLimitsDegrees.x / 180f, attitudeLimitsDegrees.y / 180f);
            this.useLimits = true;
        }

        public void SetParticleLimitsFrame(Vector3 forward, Vector3 up)
        {
            forwardParticle.frameVec = forward;
            upParticle.frameVec = up;
        }

        public void ReinitializeTransforms()
        {
            trackTransform.localPosition = initialLocalPosition;
            trackTransform.localRotation = initialLocalOrientation;
        }
        #endregion

        #region Simulation
        public abstract void Simulate(ParticleDynamicsData positionDynamics, ParticleDynamicsData rotationDynamics, float dt, float dtOld = 0);

        public void UpdateParticleTargets(Vector3 newPos, Vector3 newForward, Vector3 newUp)
        {
            positionParticle.SetTarget(newPos);
            forwardParticle.SetTargetPosition(newForward);
            upParticle.SetTargetPosition(newUp);
        }
        #endregion

    }
}

