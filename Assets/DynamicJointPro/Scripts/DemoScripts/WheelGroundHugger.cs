using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    public class WheelGroundHugger : MonoBehaviour
    {
        #region Variables
        [Header("Wheel Definitions")]
        public float wheelRadius = 0.35f;
        public List<Transform> frontWheels;
        public List<Transform> rearWheels;
        public LayerMask collisionLayer = ~0;

        private List<ParticlePoint> wheelParticles;
        private List<Transform> wheels;

        public bool enableMotion = true;
        [Header("Motor")]
        public float acceleration = 2f;
        public float maxSpeed = 5;
        private float speed = 0;

        [Header("Steer")]
        public float steerAngle = 5;

        private RigidSpatialRelations spatialRelations;
        private Vector3 initialWorldPos = Vector3.zero;
        private Vector3 initialForward = Vector3.zero;
        private Vector3 initialUp = Vector3.zero;
        private List<Vector3> initialLocalWheelsPositions;
        #endregion

        #region Motor & Steer
        public void AccelerateStep()
        {
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        public void SteerStep()
        {
            transform.Rotate(transform.up, steerAngle * Time.deltaTime);
        }

        public void DriveStep()
        {
            if (enableMotion)
            {
                AccelerateStep();
                SteerStep();
            }
        }
        #endregion

        #region Ground Alignment
        public void GroundHuggingStep()
        {
            Physics.queriesHitBackfaces = true;
            for (int i = 0; i < wheels.Count; ++i)
            {
                Transform t = wheels[i];
                RaycastHit hitInfo;
                Vector3 upVec = transform.up;

                Ray ray = new Ray(t.position + (upVec * 20f), -upVec);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, collisionLayer))
                {
                    t.position = hitInfo.point + (Vector3.up * wheelRadius);
                }
            }
        }

        public void UpdateSpatialRelations()
        {
            Vector3 nPos = spatialRelations.GetPointInSpatialRelationSpace(initialWorldPos);
            Vector3 nForward = spatialRelations.GetPointInSpatialRelationSpace(initialWorldPos + initialForward) - nPos;
            Vector3 nUp = spatialRelations.GetPointInSpatialRelationSpace(initialWorldPos + initialUp) - nPos;

            Debug.DrawLine(nPos, nPos + nForward.normalized * 2);
            Debug.DrawLine(nPos, nPos + nUp.normalized * 2);

            Quaternion finalQ = Quaternion.LookRotation(nForward.normalized, nUp.normalized);

            Vector3 fPos = transform.position;
            fPos.y = nPos.y;

            transform.position = fPos;
            transform.rotation = finalQ;


            for (int i = 0; i < wheels.Count; ++i)
            {
                Transform wheel = wheels[i];
                wheel.localPosition = initialLocalWheelsPositions[i];
            }
        }
        #endregion

        #region Initialization
        public void Initialize()
        {
            wheels = new List<Transform>();
            wheels.AddRange(rearWheels);
            wheels.AddRange(frontWheels);

            spatialRelations = new RigidSpatialRelations(wheels);

            initialWorldPos = transform.position;
            initialForward = transform.forward;
            initialUp = transform.up;

            initialLocalWheelsPositions = wheels.Select(w => w.localPosition).ToList();
        }
        #endregion

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            DriveStep();

            GroundHuggingStep();

            UpdateSpatialRelations();

            GroundHuggingStep();
        }
    }
}
