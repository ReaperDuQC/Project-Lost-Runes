using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorGUITable;
using static LostRunes.CustomTerrain;

namespace LostRunes
{
    [CustomEditor(typeof(CustomTerrain))]
    [CanEditMultipleObjects]
    public class CustomTerrainEditor : Editor
    {
        SerializedProperty _randomHeightRange;
        SerializedProperty _heightMapScale;
        SerializedProperty _heightMapImage;

        SerializedProperty _perlinXScale;
        SerializedProperty _perlinYScale;
        SerializedProperty _perlinOffsetX;
        SerializedProperty _perlinOffsetY;
        SerializedProperty _perlinOctave;
        SerializedProperty _perlinPersistence;
        SerializedProperty _perlinHeightScale;

        GUITableState _perlinParameterTable;
        SerializedProperty _perlinParameters;

        SerializedProperty _resetTerrain;

        SerializedProperty _voronoiPeakCount;
        SerializedProperty _voronoiFallOff;
        SerializedProperty _voronoiDropOff;
        SerializedProperty _voronoiMinHeight;
        SerializedProperty _voronoiMaxHeight;
        SerializedProperty _voronoiType;


        bool _showRandom = false;
        bool _showLoadHeights = false;
        bool _showPerlin = false;
        bool _showMultiplePerlin = false;
        bool _showVoronoi = false;
        private void OnEnable()
        {
            _randomHeightRange = serializedObject.FindProperty("_randomHeightRange");
            _heightMapScale = serializedObject.FindProperty("_heightMapScale");
            _heightMapImage = serializedObject.FindProperty("_heightMapImage");

            _perlinXScale = serializedObject.FindProperty("_perlinXScale");
            _perlinYScale = serializedObject.FindProperty("_perlinYScale");
            _perlinOffsetX = serializedObject.FindProperty("_perlinOffsetX");
            _perlinOffsetY = serializedObject.FindProperty("_perlinOffsetY");
            _perlinOctave = serializedObject.FindProperty("_perlinOctave");
            _perlinPersistence = serializedObject.FindProperty("_perlinPersistence");
            _perlinHeightScale = serializedObject.FindProperty("_perlinHeightScale");

            _perlinParameterTable = new GUITableState("_perlinParameterTable");
            _perlinParameters = serializedObject.FindProperty("_perlinParameters");

            _resetTerrain = serializedObject.FindProperty("_resetTerrain");

            _voronoiPeakCount = serializedObject.FindProperty("_voronoiPeakCount");
            _voronoiFallOff = serializedObject.FindProperty("_voronoiFallOff");
            _voronoiDropOff = serializedObject.FindProperty("_voronoiDropOff");
            _voronoiMinHeight = serializedObject.FindProperty("_voronoiMinHeight");
            _voronoiMaxHeight = serializedObject.FindProperty("_voronoiMaxHeight");
            _voronoiType = serializedObject.FindProperty("_voronoiType");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomTerrain terrain = (CustomTerrain)target;

            EditorGUILayout.PropertyField(_resetTerrain);

            if (GUILayout.Button("Reset Terrain"))
            {
                terrain.ResetTerrain();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            _showRandom = EditorGUILayout.Foldout(_showRandom, "Random");

            if(_showRandom)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_randomHeightRange);

                if(GUILayout.Button("Random Heights"))
                {
                    terrain.RandomTerrain();
                }
            }

            _showLoadHeights = EditorGUILayout.Foldout(_showLoadHeights, "Load Heights");
            if (_showLoadHeights)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Load Heights From Texture", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_heightMapScale);
                EditorGUILayout.PropertyField(_heightMapImage);
                if(GUILayout.Button("Load Texture"))
                {
                    terrain.LoadTexture();
                }
            }
            _showPerlin = EditorGUILayout.Foldout(_showPerlin, "Single Perlin Noise");
            if (_showPerlin)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
                EditorGUILayout.Slider(_perlinXScale, 0 ,0.1f, new GUIContent("X Scale"));
                EditorGUILayout.Slider(_perlinYScale, 0, 0.1f, new GUIContent("Y Scale"));
                EditorGUILayout.IntSlider(_perlinOffsetX, 0, 10000, new GUIContent("X Offset"));
                EditorGUILayout.IntSlider(_perlinOffsetY, 0, 10000, new GUIContent("Y Offset"));
                EditorGUILayout.IntSlider(_perlinOctave, 1, 10, new GUIContent("Octave"));
                EditorGUILayout.Slider(_perlinPersistence, 0.1f, 10f, new GUIContent("Persistence"));
                EditorGUILayout.Slider(_perlinHeightScale, 0, 1f, new GUIContent("Height Scale"));

                if (GUILayout.Button("Perlin"))
                {
                    terrain.Perlin();
                }
            }

            _showMultiplePerlin = EditorGUILayout.Foldout(_showMultiplePerlin, "Multiple Perlin Noise");
            if (_showMultiplePerlin)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    terrain.AddPerlinParameter();   
                }
                if (GUILayout.Button("-"))
                {
                    terrain.RemovePerlinParameters();
                }
                EditorGUILayout.EndHorizontal();
                _perlinParameterTable = GUITableLayout.DrawTable(_perlinParameterTable, serializedObject.FindProperty("_perlinParameters"));
                GUILayout.Space(20);


                if (GUILayout.Button("Apply Multiple Perlin"))
                {
                    terrain.MultiplePerlinTerrain();
                }
            }

            _showVoronoi = EditorGUILayout.Foldout(_showVoronoi, "Voronoi");
            if (_showVoronoi)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.IntSlider(_voronoiPeakCount, 0, 25, new GUIContent("Peak Count"));
                EditorGUILayout.Slider(_voronoiFallOff, 0, 10f, new GUIContent("Falloff"));
                EditorGUILayout.Slider(_voronoiDropOff, 0, 10f, new GUIContent("Dropoff"));
                EditorGUILayout.Slider(_voronoiMinHeight, 0, 1f, new GUIContent("Min Height"));
                EditorGUILayout.Slider(_voronoiMaxHeight, 0, 1f, new GUIContent("Max Height"));
                EditorGUILayout.PropertyField(_voronoiType);
                if (GUILayout.Button("Voronoi"))
                {
                    terrain.Voronoi();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}