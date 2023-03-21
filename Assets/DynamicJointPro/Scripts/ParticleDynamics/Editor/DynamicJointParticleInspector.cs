using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    [CustomEditor(typeof(DynamicJointParticle))]
    [CanEditMultipleObjects]
    public class DynamicJointParticleInspector : Editor
    {
        private DynamicJointParticle particle { get { return target as DynamicJointParticle; } }

        public bool IsAuthoringConstraint = false;

        public override void OnInspectorGUI()
        {
            DynamicJointInspector.DrawParticleGUI(particle);

            if (GUILayout.Button("SELECT DYNAMIC JOINT", GUILayout.Height(40f)))
            {
                List<DynamicJoint> joints = FindObjectsOfType<DynamicJoint>().ToList();
                var chain = joints.Find(c => c.particles != null && c.particles.Contains(particle));
                if (chain != null)
                {
                    Selection.activeGameObject = chain.gameObject;
                }
            }

            if (GUI.changed) EditorUtility.SetDirty(particle);
        }

        void OnSceneGUI()
        {
            var oCol = Handles.color;
            float handleSizeScale = 0.15f;

            Handles.color = Color.yellow;
            var s = particle;
            var frame = s.trackTransform.transform;
            var gravPoint = frame.position + (frame.TransformDirection(s.gravityDirection));
            Handles.DrawLine(frame.position, gravPoint);
            var hSize = HandleUtility.GetHandleSize(frame.position) * handleSizeScale;
            if (Handles.Button(frame.position, Quaternion.identity, hSize, hSize, Handles.SphereHandleCap))
            {
            }

            hSize = HandleUtility.GetHandleSize(gravPoint) * handleSizeScale * 0.5f;
            if (Handles.Button(gravPoint, Quaternion.identity, hSize, hSize, Handles.DotHandleCap))
            {
            }

            // Moving Points
            Vector3 pointWorld = Handles.PositionHandle(gravPoint, Quaternion.identity);
            Vector3 newDir = frame.InverseTransformDirection(pointWorld - frame.position).normalized;
            if (newDir != s.gravityDirection)
            {
                if (!Application.isPlaying) Undo.RecordObject(particle, "Move Gravity Direction Point");
                s.gravityDirection = newDir;
                s.gravityCurrentPos = s.gravityOldPos = pointWorld;
                Vector3.OrthoNormalize(ref s.gravityDirection, ref s.gravitySecondaryDirection);
                s.ReEncodeGravityPosition();
                Repaint();
            }

            gravPoint = frame.position + (frame.TransformDirection(s.gravitySecondaryDirection));
            if (Handles.Button(gravPoint, Quaternion.identity, hSize, hSize, Handles.DotHandleCap))
            {
                Repaint();
            }
            Handles.DrawLine(frame.position, gravPoint);

            // Moving Points
            pointWorld = Handles.PositionHandle(gravPoint, Quaternion.identity);
            newDir = frame.InverseTransformDirection(pointWorld - frame.position).normalized;
            if (newDir != s.gravitySecondaryDirection)
            {
                if (!Application.isPlaying) Undo.RecordObject(particle, "Move Gravity Secondary Direction Point");
                s.gravitySecondaryDirection = newDir;
                Vector3.OrthoNormalize(ref s.gravityDirection, ref s.gravitySecondaryDirection);
                s.ReEncodeGravityPosition();
                Repaint();
            }


            Handles.color = oCol;
        }

    }
}
