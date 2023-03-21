using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace AshqarApps.DynamicJoint
{
    [CustomEditor(typeof(DynamicChainParticle))]
    [CanEditMultipleObjects]
    public class ParticleJointInspector : Editor
    {
        private DynamicChainParticle particle { get { return target as DynamicChainParticle; } }

        public bool IsAuthoringConstraint = false;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("SELECT PARTICLE CHAIN"))
            {
                List<DynamicChain> chains = FindObjectsOfType<DynamicChain>().ToList();
                var chain = chains.Find(c => c.particles != null && c.particles.Contains(particle));
                if (chain != null)
                {
                    chain.SetSelectedParticle(particle.particleIndex);
                    Selection.activeGameObject = chain.gameObject;
                }
            }
            if (GUI.changed) EditorUtility.SetDirty(particle);
        }

        void OnSceneGUI()
        {

        }
    }
}
