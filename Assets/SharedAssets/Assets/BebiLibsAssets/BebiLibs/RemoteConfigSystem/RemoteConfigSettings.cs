using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.RemoteConfigSystem
{
    [CreateAssetMenu(fileName = "DataRemoteVariables", menuName = "BebiToddlers/DataRemoteVariables", order = 0)]
    public class RemoteConfigSettings : ScriptableConfig<RemoteConfigSettings>
    {
        [SerializeField] private int _timeOutInSeconds = 15;
        [SerializeField] private int _fetchIntervalInSeconds = 1;
        [SerializeField] private List<RemoteVariable> _remoteVariableList = new List<RemoteVariable>();

        public int TimeOutInSeconds => _timeOutInSeconds;
        public int FetchIntervalInSeconds => _fetchIntervalInSeconds;

        public List<RemoteVariable> RemoteVariableList => _remoteVariableList;

        public bool TryGetVariable(string variableKey, out RemoteVariable remoteVariable)
        {
            variableKey = variableKey.Replace(" ", "");
            remoteVariable = _remoteVariableList.Find(x => string.Equals(x.Key.Replace(" ", ""), variableKey, System.StringComparison.OrdinalIgnoreCase));
            return remoteVariable != null;
        }

        public void Add(RemoteVariable remoteVariable)
        {
            _remoteVariableList.Add(remoteVariable);
        }

        public void Clear()
        {
            _remoteVariableList.Clear();
        }

        public override void Initialize()
        {

        }

#if UNITY_EDITOR
        internal void CreateNewVariable(string assetFileName, System.Type type)
        {
            RemoteVariable variable = CreateVariable(assetFileName, type);
            _remoteVariableList.Add(variable);
            EditorUtility.SetDirty(this);
        }

        private RemoteVariable CreateVariable(string assetFileName, System.Type vType)
        {
            RemoteVariable remoteVariable = ScriptableObject.CreateInstance(vType) as RemoteVariable;

            string folderPath = Path.Combine("Assets/Resources/RemoteValues");
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Path.Combine("Assets/Resources"), "RemoteValues");
            }

            string newAssetPath = Path.Combine(folderPath, assetFileName + ".asset");

            if (AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(UnityEngine.Object)) == null)
            {
                AssetDatabase.CreateAsset(remoteVariable, newAssetPath);
                remoteVariable.SetVariableKey(assetFileName);
                return remoteVariable;
            }
            else
            {
                throw new System.Exception($"File at {newAssetPath} already exists");
            }
        }
#endif

    }
}
