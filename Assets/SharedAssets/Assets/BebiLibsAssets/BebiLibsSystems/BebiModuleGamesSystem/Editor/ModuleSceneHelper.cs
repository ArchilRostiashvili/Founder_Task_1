using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.ModulesGameSystem
{
    public class ModuleSceneHelper
    {
        private bool _hasSceneAsset;
        private List<SceneAsset> _sceneAssetList;
        private List<EditorBuildSettingsScene> _moduleSceneList = new List<EditorBuildSettingsScene>();

        public ModuleSceneHelper(List<SceneAsset> sceneAsset)
        {
            if (sceneAsset == null)
            {
                _hasSceneAsset = false;
                return;
            };

            _sceneAssetList = sceneAsset.Where(x => x != null).ToList();
            _hasSceneAsset = _sceneAssetList.Count > 0;
        }

        public void DetectIfSceneIsInBuildSettings()
        {
            _moduleSceneList = GetSceneWithName(_sceneAssetList);
        }

        public void DrawUI()
        {
            if (!_hasSceneAsset) return;


            foreach (var item in _sceneAssetList)
            {
                if (item == null)
                {
                    Debug.LogWarning("Scene Asset is null");
                    return;
                }

                string sceneName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(item));
                var edScene = _moduleSceneList.Find(x => x.path == AssetDatabase.GetAssetPath(item));
                if (edScene == null)
                {

                    EditorGUILayout.HelpBox($"Scene {sceneName} is not added in Build Settings", MessageType.Warning);
                    if (GUILayout.Button("Add Scene In Build Settings"))
                    {
                        AddScenesToBuildSettings(_sceneAssetList, true);
                    }
                    return;
                }
                else if (!edScene.enabled)
                {
                    EditorGUILayout.HelpBox($"Scene {sceneName} is in Build Settings but is not enabled", MessageType.Warning);
                    if (GUILayout.Button("Enable Scene In Build Settings"))
                    {
                        EnableScene(_sceneAssetList);
                    }
                    return;
                }
            }
        }

        public static List<EditorBuildSettingsScene> GetSceneWithName(List<SceneAsset> sceneAsset)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(2);

            EditorBuildSettingsScene[] editorScenes = EditorBuildSettings.scenes;

            foreach (var item in editorScenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(item.path);
                SceneAsset asset = sceneAsset.Find(x => x.name == sceneName);
                if (asset != null)
                {
                    scenes.Add(item);
                }
            }
            return scenes;
        }



        public static void EnableScene(List<SceneAsset> sceneAssetList)
        {
            if (sceneAssetList == null)
            {
                Debug.LogWarning("Scene Asset is not assigned");
                return;
            }

            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            foreach (var item in scenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(item.path);
                if (sceneAssetList.Find(x => x.name == sceneName) != null)
                {
                    item.enabled = true;
                }
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        public static void SetModuleSceneEnabledState(ModuleData moduleData, bool enabled)
        {
            List<SceneAsset> sceneAssets = GetModuleSceneAssets(moduleData);
            AddScenesToBuildSettings(sceneAssets, enabled);
        }

        public static void AddModuleScenesToBuildSettings(ModuleData moduleData)
        {
            List<SceneAsset> sceneAssets = GetModuleSceneAssets(moduleData);
            AddScenesToBuildSettings(sceneAssets);
        }

        public static void RemoveModuleScenesFromBuildSettings(ModuleData moduleData)
        {
            List<SceneAsset> sceneAssets = GetModuleSceneAssets(moduleData);
            RemoveScenesFromBuildSettings(sceneAssets);
        }

        public static List<SceneAsset> GetModuleSceneAssets(ModuleData moduleData)
        {
            List<SceneAsset> assets = new List<SceneAsset>();
            if (moduleData.GetSceneAsset() != null)
                assets.Add(moduleData.GetSceneAsset());

            if (moduleData.GetIntermediateSceneAsset() != null)
            {
                assets.AddRange(moduleData.GetIntermediateSceneAsset().Where(x => x != null));
            }

            foreach (var item in moduleData.ModuleGameDataList)
            {
                SceneAsset asset = item.GetSceneAsset();
                if (asset != null)
                    assets.Add(asset);
            }
            return assets;
        }

        public static void RemoveScenesFromBuildSettings(List<SceneAsset> sceneAssets)
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();
            bool isDirty = false;

            List<string> scenePath = sceneAssets.Select(x => AssetDatabase.GetAssetPath(x)).ToList();

            foreach (var editorScene in scenes)
            {
                if (!scenePath.Contains(editorScene.path))
                {
                    newScenes.Add(editorScene);
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                EditorBuildSettings.scenes = newScenes.ToArray();
            }
        }

        public static void AddScenesToBuildSettings(List<SceneAsset> sceneAssets, bool enabled = true)
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            bool isDirty = false;
            foreach (var sceneAsset in sceneAssets)
            {
                if (sceneAsset == null)
                {
                    Debug.LogWarning("Scene Asset is not assigned");
                    continue;
                }

                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                EditorBuildSettingsScene scene = scenes.Find(x => x.path == scenePath);
                if (scene != null)
                {
                    isDirty = true;
                    scene.enabled = enabled;
                }
                else if (!string.IsNullOrEmpty(scenePath))
                {
                    scenes.Add(new EditorBuildSettingsScene(scenePath, enabled));
                    isDirty = true;
                }
                else
                {
                    Debug.LogWarning("Unable To Find Scene Path");
                }
            }

            if (isDirty)
            {
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

    }
}
