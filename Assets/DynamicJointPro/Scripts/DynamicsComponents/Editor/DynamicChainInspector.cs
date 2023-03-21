using UnityEngine;
using UnityEditor;

namespace AshqarApps.DynamicJoint
{
    [CustomEditor(typeof(DynamicChain))]
    [CanEditMultipleObjects]
    public class DynamicChainInspector : Editor
    {
        private DynamicChain chain { get { return target as DynamicChain; } }

        public bool IsAuthoringConstraint = false;

        public override void OnInspectorGUI()
        {
            if (chain.particles == null || chain.particles.Count <= selectedParticle)
                selectedParticle = -1;

            if (chain.GetSelectedParticle() >= 0)
            {
                selectedParticle = chain.GetSelectedParticle();
                SceneView.RepaintAll();
                chain.SetSelectedParticle(-1);
            }

            if (selectedParticle >= 0)
            {
                DrawParticleUI(chain.particles[selectedParticle]);
            }
            else
            {
                if (!Application.isPlaying)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Chain Processing", EditorStyles.boldLabel);
                    if (GUILayout.Button("Process Chain"))
                    {
                        chain.InitializeParticles();
                        chain.ComputeRadii(this.particleRadiusCurve);
                    }

                    GUILayout.Space(10);
                }

                base.OnInspectorGUI();

                GUILayout.Space(10);
                GUILayout.Label("Particle Radius Computer", EditorStyles.boldLabel);
                //chain.particleRadius = EditorGUILayout.FloatField("Base Radius", chain.particleRadius);
                particleRadiusCurve = EditorGUILayout.CurveField("Radius Curve", particleRadiusCurve);
                if (GUILayout.Button("Compute Particle Radii"))
                {
                    chain.ComputeRadii(this.particleRadiusCurve);
                }

            }

            if (GUI.changed) EditorUtility.SetDirty(chain);
        }

        private AnimationCurve particleRadiusCurve = AnimationCurve.Constant(0, 1, 1);

        private void DrawDistanceConstraintGUI(ParticleRelativeConstraint constraint)
        {
            if (selectedParticle >= 0 && constraint != null)
            {
                DynamicChainParticle selectedNode = chain.particles[selectedParticle];

                DynamicChainParticle neighbor = constraint.otherParticle;
                if (constraint.constraintType != ParticleConstraintType.FIXED_DISTANCE)
                {
                    int selectedConstraintType = (int)constraint.constraintType - 1;
                    string[] constraintOptions = new string[]
                    {
                    "FIXED_DISTANCE", "MIN_DISTANCE", "MAX_DISTANCE",
                    };

                    selectedConstraintType = EditorGUILayout.Popup("Constraint Type", selectedConstraintType, constraintOptions);
                    constraint.constraintType = (ParticleConstraintType)((int)selectedConstraintType + 1);
                    constraint.constraintValue = EditorGUILayout.FloatField("Value", constraint.constraintValue);

                    var neighborConstraint = neighbor.constraints.Find(n => n.otherParticle == selectedNode);
                    if (neighborConstraint != null && (neighborConstraint.constraintType != constraint.constraintType
                        || neighborConstraint.constraintValue != constraint.constraintValue))
                    {
                        neighborConstraint.constraintType = constraint.constraintType;
                        neighborConstraint.constraintValue = constraint.constraintValue;
                        //chain.UpdateDistanceConstraintsHierarchy();
                    }

                    if (GUILayout.Button("Delete Constraint"))
                    {
                        IsAuthoringConstraint = false;
                        selectedNode.RemoveDistanceConstraint(constraint);
                        if (neighborConstraint != null)
                            neighbor.RemoveDistanceConstraint(neighborConstraint);

                        //chain.UpdateDistanceConstraintsHierarchy();

                        selectedConstraint = -1;
                    }
                }
            }
        }

        public void DrawParticleUI(DynamicChainParticle particle)
        {
            GUILayout.Label("Particle " + particle.particleIndex.ToString() + " settings", EditorStyles.boldLabel);
            GUILayout.Label("Transform: " + particle.trackTransform.name);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("DESELECT", GUILayout.Height(30f)))
            {
                selectedParticle = -1;
                selectedConstraint = -1;
                IsAuthoringConstraint = false;
            }
            if (particle.trackTransform != null)
            {
                if (GUILayout.Button("GO TO GAMEOBJECT", GUILayout.Height(30f)))
                {
                    Selection.activeGameObject = particle.trackTransform.gameObject;
                }
            }
            GUILayout.EndHorizontal();

            {
                GUILayout.Space(15f);


                if (selectedConstraint != -1)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Modify Constraint", EditorStyles.boldLabel);
                    DrawDistanceConstraintGUI(particle.constraints[selectedConstraint]);
                    if (GUI.changed) EditorUtility.SetDirty(chain);
                    return;
                }

                bool IKState = particle.enableIK;
                bool positionConstraintState = particle.enablePosQuatConstraint;
                var posQuatConstraint = particle.posQuatConstraint;
                var ConstraintTypeState = posQuatConstraint.constraintType;

                DrawConstraintsUI(particle);
                particle.enableIK = particle.enablePosQuatConstraint && posQuatConstraint.constraintType == PositionConstraintType.INVERSE_KINEMATICS;

                if (posQuatConstraint.constraintType == PositionConstraintType.TARGET_FORCE)
                {
                    posQuatConstraint.targetForceStrength = EditorGUILayout.FloatField("Target Force Strength", posQuatConstraint.targetForceStrength);
                }

                if (particle.enablePosQuatConstraint)
                {
                    posQuatConstraint.targetPos = EditorGUILayout.Vector3Field("Target Position", posQuatConstraint.targetPos);
                    posQuatConstraint.targetTr = EditorGUILayout.ObjectField("Target Transform", posQuatConstraint.targetTr, typeof(Transform), true) as Transform;
                    if (posQuatConstraint.constraintType == PositionConstraintType.INVERSE_KINEMATICS)
                        posQuatConstraint.constrainRotation = EditorGUILayout.Toggle("Constrain Rotation", posQuatConstraint.constrainRotation);
                }

                GUILayout.Space(10);
                particle.massRadius = EditorGUILayout.FloatField("Particle Radius", particle.massRadius);

                if (particle.enableIK != IKState || particle.enablePosQuatConstraint != positionConstraintState || posQuatConstraint.constraintType != ConstraintTypeState)
                {
                    if ((particle.enablePosQuatConstraint != positionConstraintState || posQuatConstraint.constraintType != ConstraintTypeState)
                        && posQuatConstraint.constraintType == PositionConstraintType.INVERSE_KINEMATICS)
                    {
                        chain.SetIKEndEffector(particle);
                    }
                }

                GUILayout.Space(10);
                GUILayout.Label("Custom Distance Constraints", EditorStyles.boldLabel);
                if (!IsAuthoringConstraint)
                {
                    GUILayout.Space(10);
                    if (GUILayout.Button("Author New Distance Constraint"))
                    {
                        IsAuthoringConstraint = true;
                    }
                }
                else
                {
                    GUILayout.Label("Select a particle to constrain this with.");
                }

                int counter = 1;
                for (int i = 0; i < particle.constraints.Count; ++i)
                {
                    DynamicChainParticle neighbor = particle.constraints[i].otherParticle;
                    if (particle.constraints[i].constraintType != ParticleConstraintType.FIXED_DISTANCE)
                    {
                        GUILayout.Space(10);
                        GUILayout.Label("Constraint " + counter.ToString() + ":", EditorStyles.boldLabel);
                        DrawDistanceConstraintGUI(particle.constraints[i]);
                        counter++;
                    }
                }
            }

            GUILayout.Space(15f);

            if (GUILayout.Button("FULL-CHAIN SETTINGS", GUILayout.Height(40f)))
            {
                selectedParticle = -1;
                selectedConstraint = -1;
                IsAuthoringConstraint = false;
            }
        }

        public void DrawConstraintsUI(DynamicChainParticle node)
        {
            var oldColor = GUI.backgroundColor;
            GUILayout.Label("Target Constraint", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (!node.enablePosQuatConstraint) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("NONE", GUILayout.Height(25f)))
            {
                chain.DisablePositionConstraint(node);
            }
            GUI.backgroundColor = oldColor;
            if (node.enablePosQuatConstraint && node.posQuatConstraint.constraintType == PositionConstraintType.POSITION_CONSTRAINT) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("POSITION CONTRAINT", GUILayout.Height(25f)))
            {
                chain.SetParticlePositionConstraint(node, PositionConstraintType.POSITION_CONSTRAINT);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = oldColor;
            if (node.enablePosQuatConstraint && node.posQuatConstraint.constraintType == PositionConstraintType.INVERSE_KINEMATICS) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("INVERSE KINEMATICS", GUILayout.Height(25f)))
            {
                chain.SetParticlePositionConstraint(node, PositionConstraintType.INVERSE_KINEMATICS);
            }
            GUI.backgroundColor = oldColor;
            if (node.enablePosQuatConstraint && node.posQuatConstraint.constraintType == PositionConstraintType.TARGET_FORCE) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("TARGET FORCE", GUILayout.Height(25f)))
            {
                chain.SetParticlePositionConstraint(node, PositionConstraintType.TARGET_FORCE);
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = oldColor;
        }

        public int selectedParticle = -1;

        public bool SphereButton(Vector3 position, Quaternion direction, float size, float pickSize)
        {
            return Handles.Button(position, direction, size, pickSize, Handles.SphereHandleCap);
        }

        public bool DotButton(Vector3 position, Quaternion direction, float size, float pickSize)
        {
            return Handles.Button(position, direction, size, pickSize, Handles.DotHandleCap);
        }

        private int selectedConstraint = -1;

        void OnSceneGUI()
        {
            if (chain.particles != null)
            {
                for (int i = 0; i < chain.particles.Count; ++i)
                {
                    Handles.color = (i == selectedParticle) ? Color.green : Color.red;

                    DynamicChainParticle node = chain.particles[i];
                    if (node != null)
                    {
                        for (int j = 0; j < node.constraints.Count; ++j)
                        {
                            DynamicChainParticle neighbor = node.constraints[j].otherParticle;
                            if (node.constraints[j].constraintType != ParticleConstraintType.FIXED_DISTANCE)
                            {
                                Handles.color = (i == selectedParticle && j == selectedConstraint) ? Color.green : Color.red;
                                Vector3 nPos = !Application.isPlaying ? node.GetActualPosition() : node.GetCurrentParticlePos();
                                Vector3 neighborPos = !Application.isPlaying ? neighbor.GetActualPosition() : neighbor.GetCurrentParticlePos();
                                Vector3 center = (nPos + neighborPos) / 2;
                                Vector3 c1 = center + (neighborPos - center).normalized * node.constraints[j].constraintValue / 2;
                                Vector3 c2 = center - (neighborPos - nPos).normalized * node.constraints[j].constraintValue / 2;
                                Handles.DrawLine(center, c1);
                                Handles.DrawDottedLine(c1, neighborPos, 3f);
                                Handles.DrawLine(center, c2);
                                Handles.DrawDottedLine(c2, nPos, 3f);

                                if (DotButton(center, Quaternion.identity, HandleUtility.GetHandleSize(center) * 0.1f, HandleUtility.GetHandleSize(center) * 0.1f * 0.35f))
                                {
                                    selectedParticle = node.particleIndex;
                                    selectedConstraint = j;
                                    Repaint();
                                }
                            }
                        }

                        Handles.color = (i == selectedParticle) ? Color.green : Color.yellow;
                        if (i != selectedParticle)
                        {
                            if (node.enablePosQuatConstraint && node.posQuatConstraint.constraintType == PositionConstraintType.INVERSE_KINEMATICS)
                            {
                                Handles.color = Color.blue;
                            }
                            else if (node.enablePosQuatConstraint && node.posQuatConstraint.constraintType == PositionConstraintType.POSITION_CONSTRAINT)
                            {
                                Handles.color = Color.cyan;
                            }
                        }

                        Vector3 nodePos = !Application.isPlaying ? node.GetActualPosition() : node.GetCurrentParticlePos();

                        float oRadius = node.massRadius;
                        node.massRadius = Handles.RadiusHandle(node.trackTransform.rotation, nodePos, node.massRadius);
                        if (oRadius != node.massRadius)
                            Repaint();

                        float buttonSize = Mathf.Min(HandleUtility.GetHandleSize(nodePos) * 0.2f, node.massRadius);
                        if (SphereButton(nodePos, Quaternion.identity, buttonSize, buttonSize))
                        {
                            if (IsAuthoringConstraint && selectedParticle >= 0 && selectedParticle != i)
                            {
                                DynamicChainParticle selectedNode = chain.particles[selectedParticle];
                                Vector3 nPos = !Application.isPlaying ? node.GetActualPosition() : node.GetCurrentParticlePos();
                                Vector3 neighborPos = !Application.isPlaying ? selectedNode.GetActualPosition() : selectedNode.GetCurrentParticlePos();
                                float distance = Vector3.Distance(nPos, neighborPos);
                                selectedNode.AddDistanceConstraint(node, distance, ParticleConstraintType.FIXED_DISTANCE_CUSTOM);
                                node.AddDistanceConstraint(selectedNode, distance, ParticleConstraintType.FIXED_DISTANCE_CUSTOM);
                                IsAuthoringConstraint = false;
                            }
                            else
                            {
                                selectedParticle = i;
                                selectedConstraint = -1;
                            }
                            Repaint();
                            break;
                        }
                    }
                }
            }
        }
    }
}