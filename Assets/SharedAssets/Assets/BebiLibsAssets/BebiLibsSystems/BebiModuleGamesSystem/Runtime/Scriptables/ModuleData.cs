using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.ModulesGameSystem
{
    [CreateAssetMenu(fileName = "ModuleKindData", menuName = "BebiLibs/GameModules/ModuleKindData", order = 0)]
    public class ModuleData : ScriptableObject
    {
        [SerializeField] private string _moduleName;
        [SerializeField] private SceneReference _sharedScene;
        [SerializeField] private List<SceneReference> _requiredSceneList = new List<SceneReference>();

        [Header("Module Game List:")]
        [ObjectInspector(false)]
        [SerializeField] private List<ModuleGameData> _moduleGameDataList = new List<ModuleGameData>();

        public string ModuleName => _moduleName;
        public string SharedSceneName => _sharedScene;
        public List<string> IntermediateSceneName => _requiredSceneList.Select(x => x.ToString()).ToList();

        public List<ModuleGameData> ModuleGameDataList => _moduleGameDataList;


        public bool Contains(ModuleGameData moduleGame)
        {
            return _moduleGameDataList.Any(x => x == moduleGame);
        }

        public void Init()
        {
            for (int i = 0; i < _moduleGameDataList.Count; i++)
            {
                _moduleGameDataList[i].ModuleName = _moduleName;
            }
        }


#if UNITY_EDITOR
        public SceneAsset GetSceneAsset() => _sharedScene.GetSceneAsset();
        public List<SceneAsset> GetIntermediateSceneAsset() => _requiredSceneList.Select(x => x.GetSceneAsset()).ToList();
#endif
    }

}

