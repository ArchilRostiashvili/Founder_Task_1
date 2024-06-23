#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;

namespace BebiLibs.ModulesGameSystem.ModuleEditor
{

    [CustomEditor(typeof(ModuleData), true)]
    public class ModuleContainerEditor : Editor
    {

        private ModuleData _moduleIdentifier;
        private ModuleSceneHelper _moduleSceneHelper;

        private void OnEnable()
        {
            _moduleIdentifier = (ModuleData)target;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            var assets = new List<SceneAsset> { _moduleIdentifier.GetSceneAsset() };
            assets.AddRange(_moduleIdentifier.GetIntermediateSceneAsset());
            _moduleSceneHelper = new ModuleSceneHelper(assets);
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