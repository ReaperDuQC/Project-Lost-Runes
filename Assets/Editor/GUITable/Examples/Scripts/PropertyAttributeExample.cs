using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using EditorGUITable;
#endif

public class PropertyAttributeExample : MonoBehaviour {
	
	[System.Serializable]
	public class SimpleObject
	{
		public string stringProperty;
		public float floatProperty;
		public GameObject objectProperty;
		public Vector2 v2Property;
	}

	public List<SimpleObject> simpleObjectsDefaultDisplay;

	[Table]
	public List<SimpleObject> simpleObjectsUsingTableAttribute;

	[ReorderableTable]
	public List<SimpleObject> simpleObjectsUsingReorderableTableAttribute;

	void OnGUI ()
	{
		GUILayout.Label ("Select the PropertyAttribute scene object to visualize the table in the inspector");
	}

}
