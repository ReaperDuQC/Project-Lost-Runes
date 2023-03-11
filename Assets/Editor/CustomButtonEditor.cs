using LostRunes;
using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;


[CustomEditor(typeof(CustomButton))]
public class CustomButtonEditor : ButtonEditor
{
    SerializedProperty m_ButtonTextProperty;
    SerializedProperty m_ChangeScaleProperty;
    SerializedProperty m_MenuNavigationSound;
    SerializedProperty m_ReturnButton;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_ButtonTextProperty = serializedObject.FindProperty("_buttonText");
        m_ChangeScaleProperty = serializedObject.FindProperty("_changeScale");
        m_MenuNavigationSound = serializedObject.FindProperty("_menuNavigationSound");
        m_ReturnButton = serializedObject.FindProperty("_isReturnButton");
    }
    public override void OnInspectorGUI()
    {
        CustomButton customButton = (CustomButton)target;

        EditorGUILayout.PropertyField(m_ButtonTextProperty);
        EditorGUILayout.PropertyField(m_ChangeScaleProperty);
        EditorGUILayout.PropertyField(m_MenuNavigationSound);
        EditorGUILayout.PropertyField(m_ReturnButton);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}

