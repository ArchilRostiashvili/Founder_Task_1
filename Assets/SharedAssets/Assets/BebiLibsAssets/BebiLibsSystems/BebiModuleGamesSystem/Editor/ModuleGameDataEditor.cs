using BebiLibs;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;


namespace BebiLibs.ModulesGameSystem.ModuleEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ModuleGameData), true)]
    public class ModuleGameDataEditor : Editor
    {
        private ModuleGameData _moduleGameData;
        private ModuleSceneHelper _moduleSceneHelper;

        private void OnEnable()
        {
            _moduleGameData = (ModuleGameData)target;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            _moduleSceneHelper = new ModuleSceneHelper(new List<SceneAsset> { _moduleGameData.GetSceneAsset() });
            _moduleSceneHelper.DetectIfSceneIsInBuildSettings();
        }

        private void OnDisable()
        {
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
        }

        private void OnSceneListChanged()
        {
            _moduleSceneHelper.DetectIfSceneIsInBuildSettings();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _moduleSceneHelper.DrawUI();
        }
    }
}
#endif