using BebiLibs.Localization.Core;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BebiLibs.AudioSystem
{
    [System.Serializable]
    public class LocalizedAudioTrackData
    {
        public static string LanguageIdentifierKey => nameof(_languageIdentifier);
        public static string AudioTrackReferenceKey => nameof(_audioTrackReference);

        [HideInInspector]
        [SerializeField] private LanguageIdentifier _languageIdentifier;
        [SerializeField] private AssetReferenceT<PlayableAudioTrackSO> _audioTrackReference;

        [NonSerialized]
        private LoadStatus _addressableLoadStatus;
        [NonSerialized]
        private AsyncOperationHandle<PlayableAudioTrackSO> _audioLoadHandler;
        [NonSerialized] private PlayableAudioTrackSO _runtimeTrack;

        public LanguageIdentifier LanguageIdentifier => _languageIdentifier;
        public AssetReferenceT<PlayableAudioTrackSO> AudioTrackReference => _audioTrackReference;
        public LoadStatus LoadStatus => _addressableLoadStatus;
        public PlayableAudioTrackSO RuntimeTrack => _runtimeTrack;


        public IEnumerator StartLoadingData()
        {
            if (_addressableLoadStatus == LoadStatus.Started)
            {
                Debug.LogWarning("Localized Track Data Loading Already Started");
                yield break;
            }

            if (_addressableLoadStatus == LoadStatus.Successfull)
            {
                Debug.LogWarning("Localized Track Data Loading Already Loaded Successfully");
                yield break;
            }

            _addressableLoadStatus = LoadStatus.Started;
            _audioLoadHandler = _audioTrackReference.LoadAssetAsync();
            yield return _audioLoadHandler;
            _addressableLoadStatus = _audioLoadHandler.Status == AsyncOperationStatus.Succeeded ? LoadStatus.Successfull : LoadStatus.Failed;
            _runtimeTrack = _audioLoadHandler.Result;
        }

        public void ReleaseAsset()
        {
            if (_addressableLoadStatus == LoadStatus.Started)
            {
                Debug.LogWarning("Loading already started");
                return;
            }

            if (_addressableLoadStatus == LoadStatus.Successfull && _audioLoadHandler.IsValid())
            {
                Addressables.Release(_audioLoadHandler);
            }

            if (_runtimeTrack != null)
            {
                Resources.UnloadAsset(_runtimeTrack);
            }

            _addressableLoadStatus = LoadStatus.None;
            _audioLoadHandler = default;
            _runtimeTrack = null;
        }
    }
}