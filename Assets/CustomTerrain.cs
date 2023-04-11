using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static LostRunes.CustomTerrain;

namespace LostRunes
{
    [ExecuteInEditMode]
    public class CustomTerrain : MonoBehaviour
    {
        //public Vector2 RandomHeightRange { get { return _randomHeightRange; } private set { } }
        [SerializeField] Vector2 _randomHeightRange = new Vector2(0, 0.1f);

        // Texture ------------------------
        [SerializeField] Texture2D _heightMapImage;
        [SerializeField] Vector3 _heightMapScale = new Vector3(1, 1, 1);

        [SerializeField] bool _resetTerrain;

        // Perlin Noise ------------------------
        [SerializeField] float _perlinXScale = 0.01f;
        [SerializeField] float _perlinYScale = 0.01f;
        [SerializeField] int _perlinOffsetX = 0;
        [SerializeField] int _perlinOffsetY = 0;
        [SerializeField] int _perlinOctave = 3;
        [SerializeField] float _perlinPersistence = 8f;
        [SerializeField] float _perlinHeightScale = 0.09f;

        // Multiple Perlin Noise ------------------------
        [System.Serializable]
        public class PerlinParameters
        {
            public float _perlinXScale = 0.01f;
            public float _perlinYScale = 0.01f;
            public int _perlinOffsetX = 0;
            public int _perlinOffsetY = 0;
            public int _perlinOctave = 3;
            public float _perlinPersistence = 8f;
            public float _perlinHeightScale = 0.09f;
            public bool _remove = false;
        }
        [SerializeField]
        List<PerlinParameters> _perlinParameters = new List<PerlinParameters>()
        {
            new PerlinParameters()
        };
        // Voronoi ------------------------
        [SerializeField] int _voronoiPeakCount = 5;
        [SerializeField] float _voronoiFallOff = 0.2f;
        [SerializeField] float _voronoiDropOff = 0.6f;
        [SerializeField] float _voronoiMinHeight = 0f;
        [SerializeField] float _voronoiMaxHeight = 1f;
        public enum VoronoiType { Linear = 0, Power, Combined , CombinedSine}
        [SerializeField] VoronoiType _voronoiType = VoronoiType.Linear;

        [SerializeField] Terrain _terrain;
        [SerializeField] TerrainData _terrainData;

        float[,] GetHeightMap()
        {
            return !_resetTerrain ? _terrainData.GetHeights(0, 0, _terrainData.heightmapResolution, _terrainData.heightmapResolution) : new float[_terrainData.heightmapResolution, _terrainData.heightmapResolution];
        }

        public void Perlin()
        {
            float[,] heightMap = GetHeightMap();

            for (int y = 0; y < _terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < _terrainData.heightmapResolution; x++)
                {
                    heightMap[x, y] += Utils.FractalBrownianMotion((x + _perlinOffsetX) * _perlinXScale,
                                                                    (y + _perlinOffsetY) * _perlinYScale,
                                                                    _perlinOctave,
                                                                    _perlinPersistence) * _perlinHeightScale;
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        public void MultiplePerlinTerrain()
        {
            float[,] heightMap = GetHeightMap();

            for (int y = 0; y < _terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < _terrainData.heightmapResolution; x++)
                {
                    foreach (PerlinParameters p in _perlinParameters)
                    {
                        heightMap[x, y] += Utils.FractalBrownianMotion((x + p._perlinOffsetX) * p._perlinXScale,
                                                                        (y + p._perlinOffsetY) * p._perlinYScale,
                                                                        p._perlinOctave,
                                                                        p._perlinPersistence) * p._perlinHeightScale;
                    }
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        public void Voronoi()
        {
            float[,] heightMap = GetHeightMap();

            for (int i = 0; i < _voronoiPeakCount; i++)
            {
                Vector3 peak = new Vector3( UnityEngine.Random.Range(0, _terrainData.heightmapResolution),
                                            UnityEngine.Random.Range(_voronoiMinHeight, _voronoiMaxHeight),
                                            UnityEngine.Random.Range(0, _terrainData.heightmapResolution));

                heightMap[(int)peak.x, (int)peak.z] = Mathf.Max(peak.y, heightMap[(int)peak.x, (int)peak.z]);
                Vector2 peakLocation = new Vector2(peak.x, peak.z);
                float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(_terrainData.heightmapResolution, _terrainData.heightmapResolution));

                for (int y = 0; y < _terrainData.heightmapResolution; y++)
                {
                    for (int x = 0; x < _terrainData.heightmapResolution; x++)
                    {
                        if (!(x == peak.x && y == peak.z))
                        {
                            float distanceToPeak = Vector2.Distance(new Vector2(x, y), peakLocation) / maxDistance;
                            float height = 0f;

                            switch (_voronoiType)
                            {
                                case VoronoiType.Linear:
                                    height = peak.y - distanceToPeak * _voronoiFallOff; // Linear
                                    break;
                                case VoronoiType.Power:
                                    height = peak.y - MathF.Pow(distanceToPeak, _voronoiDropOff) * _voronoiFallOff; // Power
                                    break;
                                case VoronoiType.Combined:
                                    height = peak.y - distanceToPeak * _voronoiFallOff - MathF.Pow(distanceToPeak, _voronoiDropOff); // combined
                                    break;
                                case VoronoiType.CombinedSine:
                                    height = peak.y - MathF.Pow(distanceToPeak * 3, _voronoiFallOff) - Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / _voronoiDropOff; // combined Sine
                                    break;
                                default: break;
                            }
                            //float sineWave = peak.y - Mathf.Sin(distanceToPeak * 100) * 0.01f;
                            heightMap[x, y] = Mathf.Max(heightMap[x, y], height);
                        }
                    }
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        public void AddPerlinParameter()
        {
            _perlinParameters.Add(new PerlinParameters());
        }
        public void RemovePerlinParameters()
        {
            List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();

            for (int i = 0; i < _perlinParameters.Count; i++)
            {
                if (!_perlinParameters[i]._remove)
                {
                    keptPerlinParameters.Add(_perlinParameters[i]);
                }
            }
            if (keptPerlinParameters.Count == 0)
            {
                keptPerlinParameters.Add(_perlinParameters[0]);
            }

            _perlinParameters = keptPerlinParameters;
        }
        public void RandomTerrain()
        {
            float[,] heightMap = GetHeightMap();

            for (int x = 0; x < _terrainData.heightmapResolution; x++)
            {
                for (int z = 0; z < _terrainData.heightmapResolution; z++)
                {
                    heightMap[x, z] += UnityEngine.Random.Range(_randomHeightRange.x, _randomHeightRange.y);
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        public void ResetTerrain()
        {
            float[,] heightMap = new float[_terrainData.heightmapResolution, _terrainData.heightmapResolution];

            for (int x = 0; x < _terrainData.heightmapResolution; x++)
            {
                for (int z = 0; z < _terrainData.heightmapResolution; z++)
                {
                    heightMap[x, z] = 0;
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        public void LoadTexture()
        {
            float[,] heightMap = GetHeightMap();

            for (int x = 0; x < _terrainData.heightmapResolution; x++)
            {
                for (int z = 0; z < _terrainData.heightmapResolution; z++)
                {
                    heightMap[x, z] += _heightMapImage.GetPixel((int)(x * _heightMapScale.x), (int)(z * _heightMapScale.z)).grayscale * _heightMapScale.y;
                }
            }
            _terrainData.SetHeights(0, 0, heightMap);
        }
        private void Awake()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            AddTag(tagsProp, "Terrain");
            AddTag(tagsProp, "Cloud");
            AddTag(tagsProp, "Shore");

            tagManager.ApplyModifiedProperties();

            this.gameObject.tag = "Terrain";
        }
        private void OnEnable()
        {
            _terrain = GetComponent<Terrain>();
            _terrainData = Terrain.activeTerrain.terrainData;
        }
        void AddTag(SerializedProperty tagsProp, string newTag)
        {
            bool found = false;

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(newTag))
                {
                    found = true; break;
                }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
                newTagProp.stringValue = newTag;
            }
        }
    }
}