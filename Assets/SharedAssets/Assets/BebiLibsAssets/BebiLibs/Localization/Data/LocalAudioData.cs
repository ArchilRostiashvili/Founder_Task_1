using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BebiLibs.Localization.Core;
using System.Linq;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BebiLibs.Localization
{
    [CreateAssetMenu(fileName = "LocalAudioData", menuName = "BebiLibs/Localization/LocalAudioData", order = 0)]
    public class LocalAudioData : ScriptableObject
    {
        [SerializeField] private LanguageIdentifier _identifier;
        [FolderPathViewer(FolderPathType.Absolute)]
        [SerializeField] private string _folderPath;
        [SerializeField] private AssetLabelReference _assetLabelToPreload;
        [SerializeField] private bool _isEditorOnly = false;

        [Header("Data")]
        [SerializeField] private List<AudioAssetReference> _audioAssetReferenceList = new List<AudioAssetReference>();

        public LanguageIdentifier Identifier => _identifier;
        public string LanguageID => _identifier.LanguageID;
        public string AddressableGroupName => _identifier.LanguageName;

        public string FolderPath => _folderPath;
        public List<AudioAssetReference> AudioAssetReferenceList => _audioAssetReferenceList;

        public bool IsEditorOnly => _isEditorOnly;

        //Stores Runtime Data Sound Value For Individual Localized Language
        [System.NonSerialized]
        //[SerializeField]
        private List<DataSound> _runtimeSoundDataList = new List<DataSound>();

        [SerializeField] public FolderPath ModulesPath;
        private List<AsyncOperationHandle<IList<AudioClip>>> _asyncOperationHandlerList = new List<AsyncOperationHandle<IList<AudioClip>>>();


        //Get Localized Sound by it's key
        public DataSound GetDataSound(string audioKey)
        {
            for (int i = 0; i < _runtimeSoundDataList.Count; i++)
            {
                if (_runtimeSoundDataList[i].soundName == audioKey)
                {
                    return _runtimeSoundDataList[i];
                }
            }
            return null;
        }

        //Resources Loaded from addressables
        public void AddLoadedAudioClip(AsyncOperationHandle<IList<AudioClip>> asyncOperationHandle, bool isTemp)
        {
            if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded) return;

            _asyncOperationHandlerList.Add(asyncOperationHandle);

            foreach (var item in asyncOperationHandle.Result)
            {
                AudioAssetReference audioAssetReference = _audioAssetReferenceList.Find(x => x.LocalAssetName == item.name);
                if (audioAssetReference == null || audioAssetReference.AudioLoadingStatus == AudioLoadingStatus.LoadFinished) continue;
                audioAssetReference.AudioLoadingStatus = AudioLoadingStatus.LoadFinished;

                if (!_runtimeSoundDataList.Any(x => x.soundName == audioAssetReference.AssetName))
                {
                    _runtimeSoundDataList.Add(DataSound.Create(item, audioAssetReference.AssetName, 1.0f, isTemp, false));
                }
            }
        }

        public void SetAudioReferenceList(List<AudioAssetReference> audioAssets)
        {
            _audioAssetReferenceList = audioAssets;
        }

        public void CopyDataFrom(LocalAudioData localAudioData)
        {
            _identifier = localAudioData.Identifier;
            _folderPath = localAudioData.FolderPath;
            _assetLabelToPreload = localAudioData._assetLabelToPreload;
            _isEditorOnly = localAudioData.IsEditorOnly;
        }

        internal void UnloadResources()
        {
            for (int i = 0; i < _audioAssetReferenceList.Count; i++)
            {
                _audioAssetReferenceList[i].AudioLoadingStatus = AudioLoadingStatus.Unknown;
            }
            _audioAssetReferenceList.Clear();
            _runtimeSoundDataList.Clear();


            foreach (var item in _asyncOperationHandlerList)
            {
                if (item.IsValid())
                {
                    //Debug.Log("Releasing Audio Clip: " + LanguageID);
                    Addressables.Release(item);
                }
                else
                {
                    Debug.LogError("Invalid Audio Clip: " + LanguageID);
                }
            }
            _asyncOperationHandlerList.Clear();
        }
    }
}

