using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AshqarApps.DynamicJoint
{
    [CustomEditor(typeof(DynamicColliderCapsule))]
    public class DynamicColliderCapsuleInspector : Editor
    {
        private DynamicColliderCapsule collider { get { return target as DynamicColliderCapsule; } }
        List<string> boneHierarchy = new List<string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (collider.transform.parent != null)
            {
                if (GUILayout.Button("Align To Parent"))
                {
                    collider.AlignToParent();
                }
            }
        }

        private void OnSceneGUI()
        {
            Vector3 center = collider.GetPosition();
            Vector3 s1 = collider.GetPosition() + collider.transform.TransformDirection(collider.direction).normalized * collider.height / 2;
            Vector3 s2 = collider.GetPosition() - collider.transform.TransformDirection(collider.direction).normalized * collider.height / 2;
        }

        public static void DrawWireCapsule(Vector3 _pos, Vector3 _pos2, float _radius, float _height, Color _color = default)
        {
            if (_color != default) Handles.color = _color;

            var forward = _pos2 - _pos;
            var _rot = Quaternion.LookRotation(forward);
            var pointOffset = _radius / 2f;
            var length = forward.magnitude;
            var center2 = new Vector3(0f, 0, length);

            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * pointOffset, -180f, _radius);
                Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * pointOffset, -180f, _radius);
                Handles.DrawWireDisc(center2, Vector3.forward, _radius);
                Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, _radius);
                Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, _radius);

                DrawLine(_radius, 0f, length);
                DrawLine(-_radius, 0f, length);
                DrawLine(0f, _radius, length);
                DrawLine(0f, -_radius, length);
            }
        }

        private static void DrawLine(float arg1, float arg2, float forward)
        {
            Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }
    }
}