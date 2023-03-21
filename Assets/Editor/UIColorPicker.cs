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

    Sprite _backgroundSprite;
    Sprite _borderSprite;

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
        _backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", _backgroundSprite, typeof(Sprite), false);
        _borderSprite = (Sprite)EditorGUILayout.ObjectField("Border Sprite", _borderSprite, typeof(Sprite), false);

        _backgroundsCount = EditorGUILayout.IntField(_backgroundsCount);
        _bordersCount = EditorGUILayout.IntField(_bordersCount);


        if (GUILayout.Button("Get References"))
        {
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
            SetSprites();
        }
    }
    void CheckChildForImage(Transform parent)
    {
      if(parent == null) return;

        CustomButton[] buttons = parent.GetComponentsInChildren<CustomButton>();

        foreach(var b in buttons)
        {
            Image topImage = b.GetComponent<Image>();
            topImage.color = Color.white;
           _backgrounds.Add(topImage);

            Image belowImage = b.transform.GetChild(0).GetComponent<Image>();
            belowImage.color = Color.white; 
           _borders.Add(belowImage);
        }
    }
    private void SetSprites()
    {
        if (_backgroundSprite != null)
        {
            foreach (Image background in _backgrounds)
            {
                background.sprite = _backgroundSprite;
            }
        }
        if (_borderSprite != null)
        {
            foreach (Image border in _borders)
            {
                border.sprite = _borderSprite;
            }
        }
        Repaint();
    }
}
