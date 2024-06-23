using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public abstract class ScriptableConfig<T> : ScriptableConfig where T : ScriptableConfig<T>
    {
        [System.NonSerialized] private static T _DefaultInstance;

        public static T DefaultInstance(string fileName = null)
        {
            if (_DefaultInstance != null)
            {
                return _DefaultInstance;
            }
            else if (!string.IsNullOrEmpty(fileName))
            {
                _DefaultInstance = GetInstanceFrom(fileName);
                return _DefaultInstance;
            }
            else
            {
                _DefaultInstance = GetInstanceFrom(typeof(T).Name);
                return _DefaultInstance;
            }
        }

        public static T GetInstanceFrom(string fileName)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                return GetInstanceOnPlay(fileName);
            }
            else
            {
                return GetEditorInstance(fileName);
            }
#else
           return GetInstanceOnPlay(fileName);
#endif
        }

        private static T GetInstanceOnPlay(string fileName)
        {
            T config = Resources.Load<T>(fileName);

            if (config == null)
            {
                Debug.LogError($"Unable to load {nameof(T)} from Resources path: \"{fileName}\"");
                return null;
            }

            if (!config._isInitialized)
            {
                config.Initialize();
            }

            return config;
        }


#if UNITY_EDITOR
        private static T GetEditorInstance(string fileName)
        {
            string[] guidList = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (var guid in guidList)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T config = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
                if (config.name == fileName)
                {
                    return config;
                }
            }
            return null;
        }
#endif

    }

    public abstract class ScriptableConfig : ScriptableObject
    {
        [System.NonSerialized] protected bool _isInitialized = false;
        public virtual void Initialize()
        {
            _isInitialized = true;
        }
    }
}
