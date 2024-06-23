using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace CustomEditorUtilities
{
    public static class ConfigCreator
    {
        public static T LoadOrCreateConfig<T>(string configName, string directory) where T : ScriptableObject
        {
            string[] assetGuids = AssetDatabase.FindAssets($"{configName} t:{typeof(T).Name}");

            if (assetGuids.Length == 0)
            {
                return CreateAssetInstance<T>(configName, directory);
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            if (assetGuids.Length > 1)
            {
                Debug.LogWarning($"There are multiple {nameof(T)} named {configName} assets inside project, using asset at {assetPath}");
            }
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        private static T CreateAssetInstance<T>(string name, string directory) where T : ScriptableObject
        {
            T editorConfig = ScriptableObject.CreateInstance<T>();
            CreateDirectory(directory);
            string assetPath = Path.Combine(directory, $"{name}.asset");
            AssetDatabase.CreateAsset(editorConfig, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return editorConfig;
        }

        private static void CreateDirectory(string DirectoryPath)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
        }
    }
}

#endif