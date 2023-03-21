using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class DynamicForcesComponent : MonoBehaviour
    {
        [Header("External / Local Forces")]
        [Range(0, 1)]
        public float forcesStrength = 1;
        [Range(0, 1)]
        public float forcesStartWeight = 1;
        [Range(0, 1)]
        public float forcesEndWeight = 1;
        public Transform targetSpaceTransform;
        public List<DynamicLocalForce> forces;

        public Vector3 ComputeExternalForces(ParticleRotatable particle, Transform rootSpace = null, Transform rootParentSpace = null)
        {
            Vector3 finalForce = Vector3.zero;
            if (forces == null) return finalForce;

            foreach (DynamicLocalForce externalForce in forces)
            {
                float alongChainStrength = forcesStartWeight * (1 - particle.freedomFactor) + forcesEndWeight * particle.freedomFactor;
                Vector3 force = GetForceInTargetSpace(externalForce.force, externalForce.targetSpace, particle, rootSpace, rootParentSpace);
                finalForce += ComputeLocalForce(particle, force, alongChainStrength * forcesStrength * externalForce.amplitude,
                    externalForce.frequencyAlongTime, externalForce.frequencyAlongChain, externalForce.timeOffsetPerBranch,
                        externalForce.forceProgressionMode, Time.deltaTime);
            }
            return finalForce;
        }

        public Vector3 ComputeLocalForce(ParticleRotatable particle, Vector3 localForce, float amplitude, float frequencyTime, float frequencyAlongChain, float branchTimeOffset, ExternalForceMode mode, float dt)
        {
            Vector3 localTentacleForce = localForce;
            //float freedomFactor = 1f;
            float value = (frequencyTime * Time.time) + (frequencyAlongChain * particle.freedomFactor) + (branchTimeOffset * (particle.branchID + 1));
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

            return localTentacleForceAlongChain;// + boneLengthForce;
        }

        public Vector3 GetForceInTargetSpace(Vector3 force, TargetSpaceExternalForces space, ParticleRotatable particle, Transform rootSpace, Transform rootParentSpace)
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
                    if (rootSpace != null)
                    {
                        return rootSpace.TransformDirection(force);
                    }
                    break;
                case TargetSpaceExternalForces.ROOT_PARTICLE_PARENT_SPACE:
                    if (rootParentSpace != null)
                    {
                        return rootParentSpace.TransformDirection(force);
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
    }
}