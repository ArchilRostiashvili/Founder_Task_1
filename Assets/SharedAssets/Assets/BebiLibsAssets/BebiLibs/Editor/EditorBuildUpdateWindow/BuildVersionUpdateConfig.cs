using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace CustomEditorBuildWindow
{
    public class BuildVersionUpdateConfig : ScriptableObject
    {
        public List<GameBuildVersionData> BuildVersionDataList = new List<GameBuildVersionData>();
        public List<BuildVersionHandler> BuildVersionHandlers = new List<BuildVersionHandler>();

        public bool IncrementBuildVersion;
        public bool RenameBuildVersion;

        public GameBuildVersionData GetActiveBuildVersion()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            GameBuildVersionData buildVersionData = BuildVersionDataList.Find(x => x.AppBuildTarget == buildTarget);
            if (buildVersionData == null)
            {
                Debug.LogError("BuildVersionUpdatedConfig: BuildVersionData not found for BuildTarget " + buildTarget);
                return null;
            }
            return buildVersionData;
        }

        public bool IsProjectSettingBuildVersionUpdated()
        {
            if (!TryGetBuildVersionHandler(out BuildVersionHandler buildVersionHandler)) return false;

            GameBuildVersionData buildVersionData = GetActiveBuildVersion();
            return buildVersionData.VersionString == PlayerSettings.bundleVersion && buildVersionData.BuildNumber == buildVersionHandler.BuildNumberGetter();
        }

        public void SetProjectSettingBuildVersion(GameBuildVersionData buildVersionData)
        {
            if (!TryGetBuildVersionHandler(out BuildVersionHandler buildVersionHandler)) return;
            PlayerSettings.bundleVersion = buildVersionData.VersionString;
            buildVersionHandler.BuildNumberSetter(buildVersionData.BuildNumber);
        }

        private bool TryGetBuildVersionHandler(out BuildVersionHandler buildVersionHandler)
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            buildVersionHandler = BuildVersionHandlers.Find(x => x.BuildTarget == buildTarget);
            if (buildVersionHandler == null)
            {
                Debug.LogError("BuildVersionUpdatedConfig: BuildVersionHandler not found for BuildTarget " + buildTarget);
                return false;
            }
            return true;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        internal SerializedProperty GetVersionStringProperty(GameBuildVersionData versionData)
        {
            return GetPropertyByString(versionData, nameof(GameBuildVersionData.VersionString));
        }

        internal SerializedProperty GetBuildNumberProperty(GameBuildVersionData versionData)
        {
            return GetPropertyByString(versionData, nameof(GameBuildVersionData.BuildNumber));
        }

        internal SerializedProperty GetIncrementBuildVersionProperty()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(IncrementBuildVersion));
            return serializedProperty;
        }

        internal SerializedProperty GetRenameBuildVersionProperty()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(RenameBuildVersion));
            return serializedProperty;
        }

        private SerializedProperty GetPropertyByString(GameBuildVersionData versionData, string relativeProperty)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(BuildVersionDataList));

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                SerializedProperty serializedPropertyItem = serializedProperty.GetArrayElementAtIndex(i);
                string buildTargetName = nameof(GameBuildVersionData.AppBuildTarget);
                SerializedProperty property = serializedPropertyItem.FindPropertyRelative(buildTargetName);
                if (property.intValue == (int)versionData.AppBuildTarget)
                {
                    return serializedPropertyItem.FindPropertyRelative(relativeProperty);
                }
            }

            Debug.Log("Cannot Find");
            return null;
        }

    }

    public class BuildVersionHandler
    {
        public BuildTarget BuildTarget;
        public Func<int> BuildNumberGetter;
        public Action<int> BuildNumberSetter;
    }

}
