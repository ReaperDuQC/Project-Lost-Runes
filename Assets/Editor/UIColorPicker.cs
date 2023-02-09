using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using LostRunes;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIColorPicker : EditorWindow
{
    Transform _UIParent;

    Color _normalBackgroundColor;
    Color _highlightBackgroundColor;

    Color _normalBorderColor;
    Color _highlightBorderColor;

    List<Image> _backgrounds;
    List<Image> _borders;

    int _backgroundsCount;
    int _bordersCount;

    [MenuItem("Tools/UI Color Picker")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UIColorPicker));
    }
    private void OnGUI()
    {
        GUILayout.Label("Tools to input Color for every UI Elements", EditorStyles.boldLabel);

        _UIParent = EditorGUILayout.ObjectField("UI Parent", _UIParent, typeof(Transform), true) as Transform;

        GUILayout.Label("Background Colors", EditorStyles.boldLabel);
        _normalBackgroundColor = EditorGUILayout.ColorField("Normal Background Color",_normalBackgroundColor);
        _highlightBackgroundColor = EditorGUILayout.ColorField("Highlight Background Color", _highlightBackgroundColor);

        GUILayout.Label("Border Colors", EditorStyles.boldLabel);
        _normalBorderColor = EditorGUILayout.ColorField("Normal Border Color", _normalBorderColor);
        _highlightBorderColor = EditorGUILayout.ColorField("Highlight Border Color", _highlightBorderColor);

        _backgroundsCount = EditorGUILayout.IntField(_backgroundsCount);
        _bordersCount = EditorGUILayout.IntField(_bordersCount);


        if (GUILayout.Button("Get References"))
        {
            if (_UIParent == null) return;

            _backgrounds.Clear();
            _backgrounds.TrimExcess();
            _borders.Clear();
            _borders.TrimExcess();

            CheckChildForImage(_UIParent);

            _backgroundsCount = _backgrounds.Count;
            _bordersCount = _borders.Count;

        }

        if (GUILayout.Button("Set Color Schemes"))
        {
            SetColors();
        }
    }
    void CheckChildForImage(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Image i = child.GetComponent<Image>();
            if (i != null)
            {
                if (i.sprite != null)
                {
                    string name = i.sprite.name;
                    if (name == "Background")
                    {
                        _backgrounds.Add(i);
                    }
                    else if (name == "Border")
                    {
                        _borders.Add(i);
                    }
                }
            }
            CheckChildForImage(child);
        }
    }
    private void SetColors()
    {
        foreach (Image background in _backgrounds)
        {
            background.color = _normalBackgroundColor;
        }

        foreach (Image border in _borders)
        {
            border.color = _normalBorderColor;
        }

        Repaint();
    }
}
