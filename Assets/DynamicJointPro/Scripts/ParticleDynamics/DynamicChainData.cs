using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    [System.Serializable]
    public enum ParticleConstraintType
    {
        FIXED_DISTANCE,
        FIXED_DISTANCE_CUSTOM,
        MIN_DISTANCE,
        MAX_DISTANCE
    }

    public enum PositionConstraintType
    {
        POSITION_CONSTRAINT,
        INVERSE_KINEMATICS,
        TARGET_FORCE,
    }

    public enum ExternalForceMode
    {
        CONSTANT = 0,
        SIN_WAVE,
        PERLIN_NOISE
    }

    public enum TargetSpaceSimple
    {
        WORLD_SPACE = 0,
        TARGET_TRANSFORM_SPACE,
    }

    public enum TargetSpaceExternalForces
    {
        WORLD_SPACE = 0,
        TARGET_TRANSFORM_SPACE,
        ROOT_PARTICLE_SPACE,
        ROOT_PARTICLE_PARENT_SPACE,
        PER_PARTICLE_SPACE
    }

    public enum ParticleInitializationMode
    {
        LAST_FRAME_CONTINUOUS = 0,
        STATIC_POSE,
        SOURCE_ANIMATION
    }

    public struct DynamicChainDescriptor
    {
        public float dt;
        public float dtOld;

        public int numNodes;

        public Vector3 gravity;
        public Vector3 wind;
        public float drag;

        public float startWeight;
        public float endWeight;
        public float springStrength;
        public float SKDamping;
        public float weightPower;
        public float surfaceFriction;
        public float jointLimitFriction;
        public float dampingRatio;
        public float rigidity;
        public float stretchiness;
        public float stiffness;
        public float followAnimationFactor;
        public float aerodynamicScale;
        public float jointLimitStrength;
    }

    [System.Serializable]
    public class ParticleFrame
    {
        public Vector3 framePosition;
        public Quaternion frameRotation;

        public ParticleFrame(Vector3 pos, Quaternion rotation)
        {
            SetFrame(pos, rotation);
        }

        public void SetFrame(Vector3 pos, Quaternion rotation)
        {
            this.framePosition = pos;
            this.frameRotation = rotation;
        }
    }

    [System.Serializable]
    public class ParticleDynamicsChainData : ParticleDynamicsData
    {
        public float startWeight = 1;
        public float endWeight = 1;
        public float surfaceFriction = 0;
        public float jointLimitFriction = 0f;

        public bool preventStretch = true;
        public float stretchiness = 1f;
        public float stiffness = 1f;

        /*
        public float weightPower = 1f;
        public float surfaceFriction = 0f;
        public float dampingRatio = 0.5f;

        public override float WeightPower { get { return weightPower; } }
        public override float DampingRatio { get { return dampingRatio; } }
        */

        public override float SurfaceFriction { get { return surfaceFriction; } }
        public override float JointLimitFriction { get { return surfaceFriction; } }
        public override float StartWeight { get { return startWeight; } }
        public override float EndWeight { get { return endWeight; } }
        public override float Stiffness { get { return preventStretch ? 1f : stiffness; } }
        public override float Stretchiness { get { return preventStretch ? 1f : stretchiness; } }
    }

    [System.Serializable]
    public class ParticleDynamicsData
    {
        public float forceStrength = 1;
        public float damping = 0.5f;
        public float springStrength = 1;
        public float drag = 0;

        public virtual float WeightPower { get { return 1f; } }
        public virtual float DampingRatio { get { return 0.5f; } }
        public virtual float SurfaceFriction { get { return 0f; } }
        public virtual float JointLimitFriction { get { return 0f; } }

        public virtual float StartWeight { get { return 1f; } }
        public virtual float EndWeight { get { return 1f; } }
        public virtual float Stiffness { get { return 0; } }
        public virtual float Stretchiness { get { return 0; } }
    }

    [System.Serializable]
    public struct DynamicLocalForce
    {
        public TargetSpaceExternalForces targetSpace;
        public ExternalForceMode forceProgressionMode;
        public Vector3 force;
        public float amplitude;
        public float frequencyAlongTime;
        public float frequencyAlongChain;
        public float timeOffsetPerBranch;
    }

    public static class UtilityExtensions
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = value;
            }
        }
    }
}