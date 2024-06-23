using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Localization.Core;
using UnityEngine.ResourceManagement;
using BebiLibs.Localization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audios System/Localized Audio Track")]
    public class LocalizedAudioTrackSO : AudioTrackBaseSO
    {
        public static string localizationTrackListKey => nameof(_localizedTrackDataList);

        [SerializeField] internal AudioTrackGroup _audioTrackGroup;

        [Header("Audio Tracks")]
        [UnityEngine.Serialization.FormerlySerializedAs("_fallbackAudioTrack")]
        [SerializeField] private PlayableAudioTrackSO _defaultAudioTrack;
        [SerializeField] private bool _useDefaultOnFailure = true;
        [SerializeField] private List<LocalizedAudioTrackData> _localizedTrackDataList = new List<LocalizedAudioTrackData>();

        private LocalizedAudioTrackData _loadedAudioTrackData;
        private bool _playOnlyDefault = false;
        private LanguageIdentifier _activeLanguage;

        public AudioTrackGroup AudioTrackGroup => _audioTrackGroup;
        public List<LocalizedAudioTrackData> LocalizedTrackDataList => _localizedTrackDataList;
        public PlayableAudioTrackSO DefaultAudioTrack => _defaultAudioTrack;


        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (EditorApplication.isPlaying)
                OnLocalizedAssetEnable();

#else
            OnLocalizedAssetEnable();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
            AudioLocalizationManager.RemoveLocalizedAudioTrack(this);
            ReleaseAsset();
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredPlayMode)
            {
                OnLocalizedAssetEnable();
            }
        }
#endif

        private void OnLocalizedAssetEnable()
        {
            AudioLocalizationManager.LoadLocalizationTrack(this);
        }

        internal void ReleaseAsset()
        {
            if (_loadedAudioTrackData != null)
            {
                _loadedAudioTrackData.ReleaseAsset();
                _loadedAudioTrackData = null;
            }
        }

        internal bool IsAudioTrackLoaded(LanguageIdentifier activeLanguage)
        {
            return _loadedAudioTrackData != null && _loadedAudioTrackData.LoadStatus == LoadStatus.Successfull && _loadedAudioTrackData.LanguageIdentifier == activeLanguage;
        }


        internal IEnumerator LoadLocalizedAsset(LanguageIdentifier activeLanguage)
        {
            _playOnlyDefault = false;
            _activeLanguage = activeLanguage;
            LocalizedAudioTrackData trackToLoadFromMemory = GetLocalizedAudioTrackData(activeLanguage);
            if (trackToLoadFromMemory == null)
            {
                Debug.LogWarning($"Unable to find localized track data for {activeLanguage.LanguageName} language inside {name} track", this);
                yield break;
            }

            yield return trackToLoadFromMemory.StartLoadingData();

            if (trackToLoadFromMemory.LoadStatus == LoadStatus.Successfull)
            {
                _loadedAudioTrackData = trackToLoadFromMemory;
            }
        }

        public override AudioSystemSource Play(Action<string> subtitlesCallback = null)
        {

            PlayableAudioTrackSO audioTrackToPlay = GetPlayableAudioTrack("play");
            if (audioTrackToPlay == null)
                return null;

            return audioTrackToPlay.Play(subtitlesCallback);
        }

        public override void Stop()
        {
            PlayableAudioTrackSO audioTrackToPlay = GetPlayableAudioTrack("stop");
            if (audioTrackToPlay == null)
                return;
            audioTrackToPlay.Stop();
        }

        public override bool IsPlaying()
        {
            PlayableAudioTrackSO audioTrackToPlay = GetPlayableAudioTrack("check if is playing");
            if (audioTrackToPlay == null)
                return false;
            return audioTrackToPlay.IsPlaying();
        }

        public override List<AudioSystemSource> GetPlayingAudioSources()
        {
            PlayableAudioTrackSO audioTrackToPlay = GetPlayableAudioTrack("get playing audio sources");
            if (audioTrackToPlay == null)
                return null;
            return audioTrackToPlay.GetPlayingAudioSources();
        }


        public PlayableAudioTrackSO GetPlayableAudioTrack(string forAction)
        {
            PlayableAudioTrackSO audioTrackToPlay = null;
            string errorText;
            if (_playOnlyDefault)
            {
                audioTrackToPlay = _defaultAudioTrack;
                errorText = _defaultAudioTrack == null ? $"Default audio track is not set" : null;
            }
            else if (_loadedAudioTrackData != null && _loadedAudioTrackData.LoadStatus == LoadStatus.Successfull)
            {
                audioTrackToPlay = _loadedAudioTrackData.RuntimeTrack;
                errorText = _loadedAudioTrackData.RuntimeTrack == null ? $"Runtime audio track is not set for {_activeLanguage.LanguageName} Language" : null;
            }
            else if (_useDefaultOnFailure)
            {
                audioTrackToPlay = _defaultAudioTrack;
                errorText = _defaultAudioTrack == null ? $" Runtime audio track loading failed for {_activeLanguage.LanguageName} Language and Default audio track is not set" : null;
            }
            else
            {
                errorText = $"Runtime audio track loading failed for {_activeLanguage.LanguageName} Language";
            }

            if (audioTrackToPlay == null)
            {
                Debug.LogWarning($"Unable to {forAction} localized track {name} because {errorText}", this);
                return null;
            }

            return audioTrackToPlay;
        }

        internal void SetPlayDefault(bool value)
        {
            _playOnlyDefault = value;
        }

        public bool TryGetLocalizedAudioData(LanguageIdentifier languageIdentifier, out LocalizedAudioTrackData localizedAudioTrackData)
        {
            localizedAudioTrackData = GetLocalizedAudioTrackData(languageIdentifier);
            return localizedAudioTrackData != null;
        }

        public LocalizedAudioTrackData GetLocalizedAudioTrackData(LanguageIdentifier languageIdentifier)
        {
            return _localizedTrackDataList.Find(x => x.LanguageIdentifier == languageIdentifier);
        }

        public LocalizedAudioTrackData GetLocalizedAudioTrackData(string languageName)
        {
            return _localizedTrackDataList.Find(x => x.LanguageIdentifier.LanguageName == languageName);
        }

        public void SetDefaultAudioTrack(PlayableAudioTrackSO audioTrack)
        {
            _defaultAudioTrack = audioTrack;
        }

        public void SetAudioTrackGroup(AudioTrackGroup audioTrackGroup)
        {
            _audioTrackGroup = audioTrackGroup;
        }

        public void RemoveLocalizedAudioTrackData(LocalizedAudioTrackData track)
        {
            _localizedTrackDataList.Remove(track);
        }
    }
}