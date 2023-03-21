using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class ParticleDynamicTasks
    {
        public static Vector3 UpdateParticlePosition(Vector3 currentPos, Vector3 oldPos, Vector3 targetPos, ParticleDynamicsData dynamicParams, float dt, float dtOld, out Vector3 updatedOldPos, float freedomFactor = 1, ParticleStateInfo particleState = null)
        {
            if (dt <= 0)
            {
                updatedOldPos = currentPos;
                return currentPos;
            }

            if (dtOld == 0) dtOld = dt;

            if (dynamicParams == null)
            {
                // dynamics disabled, set to rigid transform
                updatedOldPos = currentPos = targetPos;
                return currentPos;
            }

            Vector3 force = targetPos - currentPos;
            force *= dynamicParams.forceStrength;

            float weight = dynamicParams.StartWeight + (dynamicParams.EndWeight - dynamicParams.StartWeight) * freedomFactor;
            weight = Mathf.Pow(Mathf.Max(weight, 0.0f), dynamicParams.WeightPower);
            float strength = dynamicParams.springStrength * weight;

            float dragTimeScale = 0.1f;
            float drag = dynamicParams.drag;
            float dragMult = Mathf.Exp(-dt * drag / dragTimeScale);

            // Used in the integration below
            float lv = dragMult * dt / dtOld;
            float la = 0.5f * (dt + dtOld);

            Vector3 cachedParticlePos = currentPos;

            float damping = dynamicParams.damping;
            float surfaceFriction = particleState != null && particleState.isColliding ? dynamicParams.SurfaceFriction * particleState.friction : 0;
            float jointLimitFriction = particleState != null && particleState.isJointLimited ? dynamicParams.JointLimitFriction : 0;

            if (strength + damping > 0.0f)
            {
                // Full integration
                Vector3 x0 = oldPos;
                Vector3 x1 = currentPos;

                // target position animation
                Vector3 xt = currentPos;
                Vector3 xtOld = currentPos;

                Vector3 vt = dt > 0.0f ? (xt - xtOld) / dt : Vector3.zero;
                float s = Mathf.Pow(strength, 2);
                float d = surfaceFriction + jointLimitFriction + damping + (2.0f * dynamicParams.DampingRatio * strength);
                float scale = 1.0f / (1.0f + s * la * dt + d * la);

                Vector3 x2 =
                    x1 * (1.0f + lv + d * la)
                    - x0 * lv
                    + xt * (s * la * dt)
                    + vt * (d * la * dt)
                    + (force * (dt));
                x2 *= scale;
                currentPos = x2;
            }
            else
            {
                // strength is zero so we can simplify
                Vector3 x0 = oldPos;
                Vector3 x1 = currentPos;
                Vector3 x2 = x1 * (1.0f + lv) - x0 * lv
                    + (force * dt);
                currentPos = x2;
            }

            oldPos = cachedParticlePos;
            updatedOldPos = oldPos;
            return currentPos;
        }

        public static float UpdateParticleFloat(float currentValue, float targetValue, float oldValue, ParticleDynamicsData dynamicParams, float dt, out float updatedOldValue)
        {
            if (dynamicParams == null)
            {
                // dynamics disabled, set to rigid transform
                currentValue = oldValue = targetValue;
                updatedOldValue = currentValue;
                return currentValue;
            }

            float force = targetValue - currentValue;
            force *= dynamicParams.forceStrength;

            float weight = 1;
            //float damping = 0;
            float strength = dynamicParams.springStrength * weight;

            float lv = 1f;
            float la = dt;

            float cachedCurrentValue = currentValue;

            if (strength + dynamicParams.damping > 0.0f)
            {
                // Full integration
                float x0 = oldValue;
                float x1 = currentValue;

                float xt = currentValue;
                float xtOld = currentValue;

                float vt = dt > 0.0f ? (xt - xtOld) / dt : 0;
                float s = Mathf.Pow(strength, 2);
                float d = dynamicParams.damping + 2.0f * 0.5f * strength;
                float scale = 1.0f / (1.0f + s * la * dt + d * la);

                float x2 =
                    x1 * (1.0f + lv + d * la)
                    - x0 * lv
                    + xt * (s * la * dt)
                    + vt * (d * la * dt)
                    + (force * (dt));
                x2 *= scale;
                currentValue = x2;
            }
            else
            {
                // strength is zero so we can simplify
                float x0 = oldValue;
                float x1 = currentValue;
                float x2 = x1 * (1.0f + lv) - x0 * lv
                    + (force * dt);
                currentValue = x2;
            }

            updatedOldValue = cachedCurrentValue;
            return currentValue;
        }

    }
}

