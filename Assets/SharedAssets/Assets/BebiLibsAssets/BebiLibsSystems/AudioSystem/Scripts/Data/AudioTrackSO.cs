using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audio System/New Audio Track")]
    public class AudioTrackSO : PlayableAudioTrackSO
    {
        [FormerlySerializedAs("Subtitle")]
        [SerializeField] private string _subtitle;
        public string Subtitle => _subtitle;

        [FormerlySerializedAs("AudioClip")]
        [SerializeField] private AudioClip _audioClip;

        [ObjectInspector]
        [FormerlySerializedAs("Settings")]
        [SerializeField] internal AudioTrackSettingsSO _setting;
        [SerializeField] internal bool _unloadAudioClipOnDisable = true;

        public AudioClip AudioClip => _audioClip;
        public AudioTrackSettingsSO Setting => _setting;

        private void OnDisable()
        {
            if (_unloadAudioClipOnDisable && _audioClip != null)
            {
                Resources.UnloadAsset(_audioClip);
            }
        }

        public override AudioSystemSource Play(System.Action<string> subtitlesCallback = null)
        {
            return AudioManager.PlayEffect(this, subtitlesCallback);
        }

        public override void Stop()
        {
            AudioManager.StopAudio(this);
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
        }

        public void SetSettings(AudioTrackSettingsSO audioTrackSettingsSO)
        {
            _setting = audioTrackSettingsSO;
        }

        public override bool IsPlaying()
        {
            return AudioManager.IsPlaying(this);
        }

        public override List<AudioSystemSource> GetPlayingAudioSources()
        {
            return new List<AudioSystemSource>() { AudioManager.GetPlayingAudioSource(this) };
        }

        public bool IsLooping
        {
            get
            {
                LogSettingState();
                return _setting != null && _setting.IsLooping;
            }
        }

        public float Volume
        {
            get
            {
                LogSettingState();
                return _setting == null ? 1.0f : _setting.Volume;
            }
        }

        public float TimeFade
        {
            get
            {
                LogSettingState();
                return _setting == null ? 0.0f : _setting.TimeFade;
            }
        }

        public float TimeDelay
        {
            get
            {
                LogSettingState();
                return _setting == null ? 0.0f : _setting.TimeDelay;
            }
        }

        private void LogSettingState()
        {
            if (_setting == null)
            {
                Debug.LogWarning("AudioTrackSO: " + name + " has no settings assigned.");
            }
        }
    }
}
