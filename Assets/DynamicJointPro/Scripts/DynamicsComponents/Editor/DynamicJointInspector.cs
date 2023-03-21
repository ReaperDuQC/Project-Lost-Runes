using UnityEngine;
using UnityEditor;

namespace AshqarApps.DynamicJoint
{
    [CustomEditor(typeof(DynamicJoint))]
    [CanEditMultipleObjects]
    public class DynamicJointInspector : Editor
    {
        private DynamicJoint node { get { return target as DynamicJoint; } }

        private DynamicJointParticle selectedParticle = null;

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10f);
            if (selectedParticle != null)
            {
                DrawParticleGUI(selectedParticle);
                if (GUILayout.Button("DYNAMIC JOINT SETTINGS", GUILayout.Height(40f)))
                {
                    selectedParticle = null;
                }
            }
            else
            {
                GUILayout.Space(10f);
                if (GUILayout.Button("PreProcess"))
                {
                    node.Initialize();
                }

                GUILayout.Space(10f);

                base.DrawDefaultInspector();
                if (GUI.changed) EditorUtility.SetDirty(node);

            }
        }

        void OnSceneGUI()
        {
            var oCol = Handles.color;
            float handleSizeScale = 0.15f;
            if (node.particles != null && node.particles.Count > 0)
            {
                foreach (var s in node.particles)
                {
                    Handles.color = s == selectedParticle ? Color.green : Color.yellow;

                    var frame = s.trackTransform.transform;
                    var gravPoint = frame.position + (frame.TransformDirection(s.gravityDirection));
                    Handles.DrawLine(frame.position, gravPoint);
                    var hSize = HandleUtility.GetHandleSize(frame.position) * handleSizeScale;
                    if (Handles.Button(frame.position, Quaternion.identity, hSize, hSize, Handles.SphereHandleCap))
                    {
                        selectedParticle = s;
                        Repaint();
                    }

                    hSize = HandleUtility.GetHandleSize(gravPoint) * handleSizeScale * 0.5f;
                    if (Handles.Button(gravPoint, Quaternion.identity, hSize, hSize, Handles.DotHandleCap))
                    {
                        selectedParticle = s;
                        Repaint();
                    }

                    if (selectedParticle == s)
                    {
                        // Moving Points
                        Vector3 pointWorld = Handles.PositionHandle(gravPoint, Quaternion.identity);
                        Vector3 newDir = frame.InverseTransformDirection(pointWorld - frame.position).normalized;
                        if (newDir != s.gravityDirection)
                        {
                            if (!Application.isPlaying) Undo.RecordObject(node, "Move Gravity Direction Point");
                            s.gravityDirection = newDir;
                            s.gravityCurrentPos = s.gravityOldPos = pointWorld;
                            Vector3.OrthoNormalize(ref s.gravityDirection, ref s.gravitySecondaryDirection);
                            s.ReEncodeGravityPosition();
                            Repaint();
                        }

                        gravPoint = frame.position + (frame.TransformDirection(s.gravitySecondaryDirection));
                        if (Handles.Button(gravPoint, Quaternion.identity, hSize, hSize, Handles.DotHandleCap))
                        {
                            selectedParticle = s;
                            Repaint();
                        }
                        Handles.DrawLine(frame.position, gravPoint);

                        // Moving Points
                        pointWorld = Handles.PositionHandle(gravPoint, Quaternion.identity);
                        newDir = frame.InverseTransformDirection(pointWorld - frame.position).normalized;
                        if (newDir != s.gravitySecondaryDirection)
                        {
                            if (!Application.isPlaying) Undo.RecordObject(node, "Move Gravity Secondary Direction Point");
                            s.gravitySecondaryDirection = newDir;
                            Vector3.OrthoNormalize(ref s.gravityDirection, ref s.gravitySecondaryDirection);
                            s.ReEncodeGravityPosition();
                            Repaint();
                        }

                    }
                }
            }

            Handles.color = oCol;
        }

        public static void DrawParticleGUI(DynamicJointParticle particle)
        {
            GUILayout.Space(10f);
            GUILayout.Label("Particle '" + particle.trackTransform.name + "' settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (particle != null)
            {
                if (Selection.activeGameObject != particle.trackTransform.gameObject && GUILayout.Button("GO TO GAMEOBJECT"))
                {
                    Selection.activeGameObject = particle.trackTransform.gameObject;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                var oDir = particle.gravityDirection;
                var oSDir = particle.gravitySecondaryDirection;
                particle.gravityDirection = EditorGUILayout.Vector3Field("Center of Mass Direction", particle.gravityDirection);
                particle.gravitySecondaryDirection = EditorGUILayout.Vector3Field("Center of Mass Secondary Direction", particle.gravitySecondaryDirection);
                if (oDir != particle.gravityDirection || oSDir != particle.gravitySecondaryDirection)
                {
                    particle.ReEncodeGravityPosition();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("FORWARD"))
                {
                    particle.gravityDirection = Vector3.forward;
                    particle.gravitySecondaryDirection = new Vector3(particle.gravityDirection.y, particle.gravityDirection.z, particle.gravityDirection.x);
                }
                if (GUILayout.Button("UP"))
                {
                    particle.gravityDirection = Vector3.up;
                    particle.gravitySecondaryDirection = new Vector3(particle.gravityDirection.y, particle.gravityDirection.z, particle.gravityDirection.x);
                }
                if (GUILayout.Button("RIGHT"))
                {
                    particle.gravityDirection = Vector3.right;
                    particle.gravitySecondaryDirection = new Vector3(particle.gravityDirection.y, particle.gravityDirection.z, particle.gravityDirection.x);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
        }
    }
}