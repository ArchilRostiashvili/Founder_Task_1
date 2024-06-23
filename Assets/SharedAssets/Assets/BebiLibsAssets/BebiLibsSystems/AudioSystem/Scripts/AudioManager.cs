using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BebiLibs;

namespace BebiLibs.AudioSystem
{
    public class AudioManager : GenericSingletonClass<AudioManager>
    {
        private const string _SOUND_ON = "audio_manager_sound_on";
        private const string _SOUND_EFFECT_ON = "audio_manager_sound_on_effects";
        private const string _SOUND_BACKGROUND_ON = "audio_manager_sound_on_backgrounds";

        public static float VolumeGlobalEffects;
        public static float VolumeGlobalBackgrounds;
        public static Action CallBackChanged;


        private static bool _SoundOn;
        private static bool _SoundOnEffects;
        private static bool _SoundOnBackgrounds;


        private Dictionary<string, Coroutine> _runningCoroutineList = new Dictionary<string, Coroutine>();
        [SerializeField] private List<AudioSystemSource> _audioSystemSourceList;

        private bool _soundOnBackgroundsTemp;
        private bool _soundOnEffectsTemp;
        private AudioTrackSettingsSO _defaultSettings;


        protected override void OnInstanceAwake()
        {
            _dontDestroyOnLoad = true;
            _defaultSettings = Resources.Load<AudioTrackSettingsSO>("BaseSettings");
            _audioSystemSourceList = new List<AudioSystemSource>();
            AudioSystemSource audioSystemSource = CreateNewAudioSource();
            _audioSystemSourceList.Add(audioSystemSource);

            InitContent();
        }



        private static AudioSystemSource CreateNewAudioSource()
        {
            if (Instance == null) return null;
            GameObject gameObject = new GameObject("AudioSystemSource");
            gameObject.transform.parent = Instance.transform;
            var audioSystemSource = gameObject.AddComponent<AudioSystemSource>();
            return audioSystemSource;
        }

        public void InitContent()
        {
            _SoundOn = PlayerPrefs.GetInt(_SOUND_ON, 1) == 1;
            _SoundOnEffects = PlayerPrefs.GetInt(_SOUND_EFFECT_ON, 1) == 1;
            _SoundOnBackgrounds = PlayerPrefs.GetInt(_SOUND_BACKGROUND_ON, 1) == 1;

            AudioManager.VolumeGlobalEffects = _SoundOnEffects ? 1.0f : 0.0f;
            AudioManager.VolumeGlobalBackgrounds = _SoundOnBackgrounds ? 1.0f : 0.0f;
        }

        public static void ChangeState()
        {
            AudioManager.SoundsOn = !_SoundOn;
        }

        public static bool SoundsOn
        {
            get
            {
                return _SoundOn;
            }
            set
            {
                PlayerPrefs.SetInt(_SOUND_ON, value ? 1 : 0);
                _SoundOn = value;

                AudioManager.SoundOnEffects = _SoundOn;
                AudioManager.SoundOnBackgrounds = _SoundOn;

                AudioManager.CallBackChanged?.Invoke();
            }
        }

        public static bool SoundOnEffects
        {
            get
            {
                return _SoundOnEffects;
            }
            set
            {
                PlayerPrefs.SetInt(_SOUND_EFFECT_ON, value ? 1 : 0);
                _SoundOnEffects = value;
                AudioManager.VolumeGlobalEffects = _SoundOnEffects ? 1.0f : 0.0f;
                AudioManager.UpdateVolume();
            }
        }

        public static bool SoundOnBackgrounds
        {
            get
            {
                return _SoundOnBackgrounds;
            }
            set
            {
                PlayerPrefs.SetInt(_SOUND_BACKGROUND_ON, value ? 1 : 0);
                _SoundOnBackgrounds = value;
                AudioManager.VolumeGlobalBackgrounds = _SoundOnBackgrounds ? 1.0f : 0.0f;
                AudioManager.UpdateVolume();
            }
        }

        public static void UpdateVolume()
        {
            if (Instance == null) return;
            for (int j = 0; j < Instance._audioSystemSourceList.Count; j++)
            {
                Instance._audioSystemSourceList[j].UpdateVolume();
            }
        }

        public static void StopSound(string soundName, float timeFade = 0.0f)
        {
            if (Instance == null) return;
            Instance.StopPlayingSound(soundName, timeFade);
        }

        public void StopPlayingSound(string soundName, float timeFade = 0.0f)
        {
            AudioSystemSource itemAudioSource;
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                itemAudioSource = _audioSystemSourceList[j];
                if (itemAudioSource.AudioTrack != null && itemAudioSource.AudioTrack.AudioClip.name == soundName)
                {
                    itemAudioSource.StopSound(timeFade);
                }
            }
        }

        public static void StopAllSounds()
        {
            if (Instance == null) return;
            Instance.StopAllPlayingSounds();
        }

        public void StopAllPlayingSounds()
        {
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                _audioSystemSourceList[j].StopSound(0.0f);
            }
        }

        private AudioSystemSource GetFreeAudioSource()
        {
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                if (_audioSystemSourceList[j].State == AudioSystemSource.SOURCE_STATE.PASSIVE)
                {
                    return _audioSystemSourceList[j];
                }
            }
            return CreateAudioSourceLocal();
        }

        private AudioSystemSource GetAudioSourceWithTrack(AudioTrackBaseSO audioTrack)
        {
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                if (_audioSystemSourceList[j].State != AudioSystemSource.SOURCE_STATE.PASSIVE && _audioSystemSourceList[j].AudioTrack == audioTrack)
                {
                    return _audioSystemSourceList[j];
                }
            }
            return null;
        }

        private AudioSystemSource CreateAudioSourceLocal()
        {
            GameObject go = GameObject.Instantiate(_audioSystemSourceList[0].gameObject, transform);
            AudioSystemSource itemAudioSource = go.GetComponent<AudioSystemSource>();
            _audioSystemSourceList.Add(itemAudioSource);
            itemAudioSource.StopSound();
            return itemAudioSource;
        }

        public static AudioSystemSource PlayEffect(AudioTrackSO audioTrack, System.Action<string> subtitlesCallback = null)
        {
            return Instance.PlayEffectLocal(audioTrack, subtitlesCallback);
        }

        public AudioSystemSource PlayEffectLocal(AudioTrackSO audioTrack, System.Action<string> subtitlesCallback = null)
        {
            AudioTrackSO trackToPlay = audioTrack;

            if (trackToPlay != null)
            {
                if (trackToPlay.Setting == null)
                {
                    trackToPlay.SetSettings(_defaultSettings);
                }

                AudioSystemSource audioSystemSource = GetFreeAudioSource();
                audioSystemSource.SetEndCallBack(null);
                audioSystemSource.PlayEffect(trackToPlay);
                subtitlesCallback?.Invoke(trackToPlay.Subtitle);
                return audioSystemSource;
            }
            else
            {
                Debug.LogWarning("Passed AudioTrackSO is null");
            }

            return null;
        }



        public static void PlaySequence(AudioTrackSequenceSO trackSequence, System.Action<string> subtitlesCallback = null)
        {
            Instance.PlaySequenceLocal(trackSequence, subtitlesCallback);
        }

        private void PlaySequenceLocal(AudioTrackSequenceSO sequence, System.Action<string> subtitlesCallback = null)
        {
            if (sequence.ReplayEvenIfPlaying)
            {
                StopPlayingSequenceIfPlaying(sequence);
            }
            else if (_runningCoroutineList.ContainsKey(sequence.TrackID))
            {
                return;
            }

            Coroutine coroutine = StartCoroutine(PlayAudioSequence(sequence, subtitlesCallback));
            _runningCoroutineList.Add(sequence.TrackID, coroutine);
        }

        private IEnumerator PlayAudioSequence(AudioTrackSequenceSO trackSequence, System.Action<string> subtitlesCallback = null)
        {
            yield return trackSequence.PlayAudioSequence(subtitlesCallback);
            _runningCoroutineList.Remove(trackSequence.TrackID);
        }


        private void StopPlayingSequenceIfPlaying(AudioTrackSequenceSO trackSequence)
        {
            if (_runningCoroutineList.TryGetValue(trackSequence.TrackID, out Coroutine value))
            {
                StopCoroutine(value);
                _runningCoroutineList.Remove(trackSequence.TrackID);
            }
        }

        public static void StopSequence(AudioTrackSequenceSO trackSequence)
        {
            if (Instance == null) return;
            Instance.StopPlayingSequenceIfPlaying(trackSequence);
        }


        public static void StopAudio(AudioTrackBaseSO trackBase)
        {
            if (Instance == null) return;
            Instance.StopPlayingAudioTrack(trackBase);
        }

        public void StopPlayingAudioTrack(AudioTrackBaseSO audioTrack)
        {
            AudioSystemSource audioSystemSource = GetAudioSourceWithTrack(audioTrack);
            if (audioSystemSource != null)
            {
                audioSystemSource?.StopSound();
            }
        }


        public static AudioSystemSource PlayBackground(AudioTrackSO audioTrack)
        {
            if (Instance == null) return null;
            return Instance.PlayBackgroundLocal(audioTrack);
        }

        public AudioSystemSource PlayBackgroundLocal(AudioTrackSO audioTrack)
        {
            if (audioTrack != null)
            {
                AudioSystemSource itemAudioSource = AudioManager.GetPlayingAudioSource(audioTrack.AudioClip.name);
                if (itemAudioSource != null)
                {
                    itemAudioSource.ChangeVolume(audioTrack._setting.Volume, audioTrack._setting.TimeFade);
                }
                else
                {
                    itemAudioSource = GetFreeAudioSource();
                    itemAudioSource.PlayBackground(audioTrack);
                }
                return itemAudioSource;
            }
            else
            {
                return null;
            }
        }


        public static void ChangeVolume(string soundName, float volumeTo, float timeFade = 0.0f)
        {
            if (Instance == null) return;
            Instance.ChangeVolumeLocal(soundName, volumeTo, timeFade);
        }

        public void ChangeVolumeLocal(string soundName, float volumeTo, float timeFade = 0.0f)
        {
            AudioSystemSource itemAudioSource;
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                itemAudioSource = _audioSystemSourceList[j];
                if (itemAudioSource.AudioTrack != null && itemAudioSource.AudioTrack.AudioClip.name == soundName)
                {
                    itemAudioSource.ChangeVolume(volumeTo, timeFade);
                }
            }
        }


        public static AudioSystemSource GetPlayingAudioSource(string soundName)
        {
            if (Instance == null) return null;
            return Instance.GetPlayingAudioSourceLocal(soundName);
        }


        internal static AudioSystemSource GetPlayingAudioSource(AudioTrackBaseSO trackBaseSO)
        {
            if (Instance == null) return null;
            return Instance.GetAudioSourceWithTrack(trackBaseSO);
        }

        public AudioSystemSource GetPlayingAudioSourceLocal(string soundName)
        {
            AudioSystemSource itemAudioSource;
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                itemAudioSource = _audioSystemSourceList[j];
                if (itemAudioSource.State != AudioSystemSource.SOURCE_STATE.PASSIVE && itemAudioSource.AudioTrack != null && itemAudioSource.AudioTrack.AudioClip.name == soundName)
                {
                    return itemAudioSource;
                }
            }
            return null;
        }

        internal static bool IsPlaying(AudioTrackSO audioTrackSO)
        {
            if (Instance == null) return false;
            return Instance.IsPlayingLocal(audioTrackSO);
        }

        private bool IsPlayingLocal(AudioTrackSO audioTrackSO)
        {
            AudioSystemSource itemAudioSource;
            for (int j = 0; j < _audioSystemSourceList.Count; j++)
            {
                itemAudioSource = _audioSystemSourceList[j];
                if (itemAudioSource.State != AudioSystemSource.SOURCE_STATE.PASSIVE && itemAudioSource.AudioTrack == audioTrackSO)
                {
                    return true;
                }
            }
            return false;
        }
    }
}