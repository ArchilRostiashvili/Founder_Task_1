using BebiLibs;
using BebiLibs.Analytics.GameEventLogger;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.ModulesGameSystem
{
    [CreateAssetMenu(fileName = "ModuleGameData", menuName = "BebiLibs/GameModules/ModuleGameData")]
    public class ModuleGameData : ScriptableObject
    {
        [Header("Naming:")]
        [SerializeField] private string _moduleName;
        [SerializeField] private string _displayName;

        [Header("Scene Details:")]
        [SerializeField] private SceneReference _sceneReference;

        [Space]
        [SerializeField] private bool _isGameLocked;
        [SerializeField] private bool _isComingSoon;
        [SerializeField] private bool _playAudioOnGameStart = false;

        public string ModuleName
        {
            get => _moduleName; set => _moduleName = value;
        }

        public string SceneName => _sceneReference;
        public string DisplayName => _displayName;

        public string AnalyticsID => _moduleName + "_" + _displayName;

        public bool IsGameLocked { get => _isGameLocked; set => _isGameLocked = value; }
        public bool IsComingSoon { get => _isComingSoon; set => _isComingSoon = value; }
        public bool PlayAudioOnGameStart { get => _playAudioOnGameStart; set => _playAudioOnGameStart = value; }

        public virtual bool GetExtraAnalyticsParameters(out List<IGameParameter> gameParameters)
        {
            gameParameters = null;
            return false;
        }


#if UNITY_EDITOR
        public SceneAsset GetSceneAsset() => _sceneReference.GetSceneAsset();
#endif
    }
}
