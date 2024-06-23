using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace DependencyResolverInitializer
{

    [InitializeOnLoad]
    public class InitializeDependencyResolver
    {
        static AddRequest _addRequest;
        static DependencyManagerInitializedConfig _config;

        static InitializeDependencyResolver()
        {
            string[] assetPath = AssetDatabase.FindAssets($"t:{typeof(DependencyManagerInitializedConfig).Name}");

            if (assetPath.Length == 0) { return; }

            _config = AssetDatabase.LoadAssetAtPath<DependencyManagerInitializedConfig>(AssetDatabase.GUIDToAssetPath(assetPath[0]));

            if (_config.IsInitialized) { return; }
            Debug.Log("InitializeDependencyResolver");

            string dependencyAssetPath = AssetDatabase.GetAssetPath(_config.DependencyManagerAsset);
            string dependencyPackagePath = "file:../" + dependencyAssetPath;
            _addRequest = Client.Add(dependencyPackagePath);
            EditorApplication.update += CheckSearch;
        }

        static void CheckSearch()
        {
            if (_addRequest.IsCompleted)
            {
                if (_addRequest.Status != StatusCode.Success)
                {
                    Debug.Log(_addRequest.Error.message);
                }
                else
                {
                    _config.IsInitialized = true;
                }

                EditorApplication.update -= CheckSearch;
            }
        }
    }
}
