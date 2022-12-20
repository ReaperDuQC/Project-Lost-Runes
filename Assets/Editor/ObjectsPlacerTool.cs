using UnityEngine;
using UnityEditor;

namespace LostRunes
{
    public class ObjectsPlacerTool : EditorWindow
    {
        GameObject _listOfObjects;
        Vector3 _offset;

        [MenuItem("Tools/Object Placer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ObjectsPlacerTool));
        }
        private void OnGUI()
        {
            GUILayout.Label("Place Objects From A Root", EditorStyles.boldLabel);
            _listOfObjects = EditorGUILayout.ObjectField("List Root", _listOfObjects, typeof(GameObject), true) as GameObject;
            _offset = EditorGUILayout.Vector3Field("Offset", _offset);

            if(GUILayout.Button("Place Child Objects"))
            {
                PlaceObjects();
            }

            void PlaceObjects()
            {
                if(_listOfObjects != null)
                {
                    int i = 0;

                    foreach(Transform t in _listOfObjects.transform)
                    {
                        t.transform.position = _listOfObjects.transform.position + _offset * i;
                        i++;
                    }
                }
            }
        }
    }
}