using BebiLibs.Localization;
using BebiLibs.Localization.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BebiLibs.AudioSystem
{
    public class AudioLocalizationManager : GenericSingletonClass<AudioLocalizationManager>
    {
        private static List<WeakReference<LocalizedAudioTrackSO>> _LocalTrackList = new List<WeakReference<LocalizedAudioTrackSO>>();

        protected override void OnInstanceAwake()
        {
            _dontDestroyOnLoad = true;
            LocalizationManager.OnLanguageChangeEvent -= OnLanguageChanged;
            LocalizationManager.OnLanguageChangeEvent += OnLanguageChanged;
        }

        private void OnLanguageChanged(LanguageIdentifier obj)
        {
            List<WeakReference<LocalizedAudioTrackSO>> deletedList = new List<WeakReference<LocalizedAudioTrackSO>>();
            foreach (var weakReference in _LocalTrackList)
            {
                if (weakReference.TryGetTarget(out LocalizedAudioTrackSO trackSO))
                {
                    trackSO.ReleaseAsset();
                    StartCollectingAudioTracks(trackSO);
                }
                else
                    deletedList.Add(weakReference);
            }

            foreach (var item in deletedList)
                _LocalTrackList.Remove(item);
        }

        public static void RemoveLocalizedAudioTrack(LocalizedAudioTrackSO localizedAudioTrack)
        {
            List<WeakReference<LocalizedAudioTrackSO>> deletedList = new List<WeakReference<LocalizedAudioTrackSO>>();
            foreach (var weakReference in _LocalTrackList)
            {
                if (weakReference.TryGetTarget(out LocalizedAudioTrackSO trackSO))
                    if (trackSO == localizedAudioTrack)
                        deletedList.Add(weakReference);
            }
            foreach (var item in deletedList)
                _LocalTrackList.Remove(item);
        }

        protected override void OnInstanceDestroy()
        {

        }


        internal static void LoadLocalizationTrack(LocalizedAudioTrackSO audioTrackSO)
        {
            if (audioTrackSO == null)
            {
                Debug.LogError($"{nameof(audioTrackSO)} is null, You Most forgot to assign correct {nameof(LocalizedAudioTrackSO)}");
                return;
            }

            WeakReference<LocalizedAudioTrackSO> weakReference = new WeakReference<LocalizedAudioTrackSO>(audioTrackSO);
            _LocalTrackList.Add(weakReference);

            StartCollectingAudioTracks(audioTrackSO);
        }

        public static void StartCollectingAudioTracks(LocalizedAudioTrackSO audioTrack)
        {
            audioTrack.SetPlayDefault(LocalizationManager.IsEnglish());
            if (!audioTrack.IsAudioTrackLoaded(LocalizationManager.ActiveLanguage) && !LocalizationManager.IsEnglish())
            {
                if (Instance != null)
                {
                    Instance.StartCoroutine(audioTrack.LoadLocalizedAsset(LocalizationManager.ActiveLanguage));
                }
            }
        }

    }
}
