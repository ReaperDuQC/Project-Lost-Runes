using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/*
 * These are static methods used in the editor scripts from Infinity PBR
 */

namespace InfinityPBR
{
    [System.Serializable]
    public static class InfinityStatic
    {
#if UNITY_EDITOR
        public static string[] AllPrefabGuids => AssetDatabase.FindAssets("t:Prefab");
        public static string[] AllPrefabPaths => AllPrefabGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
#endif
        
        public static Vector3 WorldPositionOf(Transform transform, Vector3 positionOffset) => transform.TransformPoint(positionOffset);
    }
}
