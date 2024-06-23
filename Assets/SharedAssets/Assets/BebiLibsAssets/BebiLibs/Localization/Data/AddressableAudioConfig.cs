using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BebiLibs.Localization.Core;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization
{
    [CreateAssetMenu(fileName = "AddressableAudioConfig", menuName = "BebiLibs/Localization/AddressableAudioConfig", order = 0)]
    public class AddressableAudioConfig : ScriptableConfig<AddressableAudioConfig>
    {
        [SerializeField] private List<LocalAudioData> _localLanguagesList = new List<LocalAudioData>();
        [SerializeField] private List<AudioClip> _preloadAssetsList = new List<AudioClip>();
        [SerializeField] private List<DataSoundContainer> _nonLocalizedSounds;

        public IEnumerable<AudioClip> AssetsToPreload => _preloadAssetsList;
        public List<LocalAudioData> LocalLanguagesList => _localLanguagesList;
        public List<DataSoundContainer> NonLocalizedSounds => _nonLocalizedSounds;

        public DataSound FindNonLocalizedSound(string soundName)
        {
            foreach (var item in _nonLocalizedSounds)
            {
                DataSound dataSound = item.Find(soundName);
                if (dataSound != null)
                {
                    return dataSound;
                }
            }
            return null;
        }

        public LocalAudioData GetLocalByIdentifier(LanguageIdentifier languageIdentifier)
        {
            return _localLanguagesList.Find(x => x.Identifier == languageIdentifier);
        }

        public LocalAudioData GetAllLocalByIdentifier(LanguageIdentifier languageIdentifier)
        {
            LocalAudioData localAudioData = ScriptableObject.CreateInstance<LocalAudioData>();
            List<LocalAudioData> localList = _localLanguagesList.Where(x => x.Identifier == languageIdentifier).ToList();

            if (localList.Count == 0)
            {
                Debug.LogError($"There is no local Audio date for {languageIdentifier}");
                return localAudioData;
            }

            localAudioData.CopyDataFrom(localList[0]);
            foreach (var item in localList)
            {
                localAudioData.AudioAssetReferenceList.AddRange(item.AudioAssetReferenceList);
            }
            return localAudioData;
        }

        public override void Initialize()
        {
        }

        internal void SetAudioDataList(List<LocalAudioData> localAudioData)
        {
            _localLanguagesList = localAudioData;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(AddressableAudioConfig))]
    public class AddressableAudioConfigEditor : Editor
    {
        private AddressableAudioConfig _addressableAudioConfig;

        private void OnEnable()
        {
            _addressableAudioConfig = (AddressableAudioConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Collect All Audio Local Data"))
            {
                var assetPath = AssetDatabase.FindAssets($"t:{typeof(LocalAudioData).Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
                List<LocalAudioData> localAudioData = assetPath.Select(x => AssetDatabase.LoadAssetAtPath<LocalAudioData>(x)).ToList();

                _addressableAudioConfig.SetAudioDataList(localAudioData);
                EditorUtility.SetDirty(_addressableAudioConfig);
            }
        }
    }
#endif
}
