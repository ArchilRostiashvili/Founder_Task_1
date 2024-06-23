using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystemSource : MonoBehaviour
    {
        public enum AUDIO_ACTION_TYPE { NONE, PLAYING, FADE_OUT, FADE_IN, OFF };
        public enum AUDIO_TYPE { EFFECT, BACKGROUND };
        public enum SOURCE_STATE { PASSIVE, ACTIVE, IN_PROCESS };

        public AudioTrackSO AudioTrack { get; private set; }
        public AudioSource AudioSourceComponent { get; private set; }
        public AUDIO_ACTION_TYPE ActionType { get; private set; }
        public SOURCE_STATE State { get; private set; }
        public AUDIO_TYPE AudioType { get; private set; }
        public float PlayTime { get; private set; }

        private System.Action _audioEndCallBack;
        private float _volume;
        private float _volumeFrom;
        private float _volumeTo;
        private float _timeValue;
        private float _timeTotal;

        private void Awake()
        {
            if (AudioSourceComponent == null)
            {
                AudioSourceComponent = gameObject.AddComponent<AudioSource>();
                AudioSourceComponent.playOnAwake = false;
                AudioSourceComponent.loop = false;
                AudioSourceComponent.volume = 1;
            }
        }

        public void PlayEffect(AudioTrackSO audioTrack)
        {
            if (audioTrack.AudioClip == null) return;

            AudioTrack = audioTrack;
            AudioType = AUDIO_TYPE.EFFECT;
            gameObject.SetActive(true);
            State = SOURCE_STATE.IN_PROCESS;
            ActionType = AUDIO_ACTION_TYPE.NONE;

            if (-0.0000001f < AudioTrack.Setting.TimeFade)
            {
                ChangeVolume(0.0f, 0.0f);
                ChangeVolume(AudioTrack.Setting.Volume, AudioTrack.Setting.TimeFade);
            }
            else if (AudioTrack.Setting.TimeFade > 0.00000001f)
            {
                ChangeVolume(AudioTrack.Setting.Volume, AudioTrack.Setting.TimeFade);
            }
            else
            {
                ChangeVolume(AudioTrack.Setting.Volume, 0);
            }

            PlaySound(AudioTrack.Setting.IsLooping, AudioTrack.Setting.TimeFade);
        }

        public void PlayBackground(AudioTrackSO audioTrack)
        {
            if (audioTrack.AudioClip == null) return;

            AudioTrack = audioTrack;
            gameObject.SetActive(true);
            AudioType = AUDIO_TYPE.BACKGROUND;
            State = SOURCE_STATE.IN_PROCESS;
            ActionType = AUDIO_ACTION_TYPE.NONE;

            if (0.0f < AudioTrack.Setting.TimeFade)
            {
                ChangeVolume(0.0f, 0.0f);
                ChangeVolume(AudioTrack.Setting.Volume, AudioTrack.Setting.TimeFade);
            }
            else
            {
                ChangeVolume(AudioTrack.Setting.Volume, AudioTrack.Setting.TimeFade);
            }

            PlaySound(true, AudioTrack.Setting.TimeFade);
        }

        private void PlaySound(bool loop, float delay = 0.0f)
        {
            State = SOURCE_STATE.ACTIVE;

            AudioSourceComponent.volume = _volume * this.GlobalVolume;
            AudioSourceComponent.loop = loop;
            AudioSourceComponent.clip = AudioTrack.AudioClip;
            PlayTime = AudioSourceComponent.clip.length + 0.5f + delay;

            if (0.0f < delay)
            {
                AudioSourceComponent.PlayDelayed(delay);
            }
            else
            {
                AudioSourceComponent.Play();
            }
        }

        private float GlobalVolume
        {
            get
            {
                if (AudioType == AUDIO_TYPE.BACKGROUND)
                {
                    return AudioManager.VolumeGlobalBackgrounds;
                }
                else
                {
                    return AudioManager.VolumeGlobalEffects;
                }
            }
        }

        public void ChangeVolume(float volumeTo, float timeFade = 0.0f)
        {
            if (AudioTrack == null) return;
            if (State == SOURCE_STATE.PASSIVE) return;

            if (timeFade == 0.0f)
            {
                _volume = volumeTo;
                AudioSourceComponent.volume = _volume * GlobalVolume;
                ActionType = AUDIO_ACTION_TYPE.NONE;
            }
            else
            {
                _volumeFrom = _volume;
                _volumeTo = volumeTo;
                _timeValue = timeFade;
                _timeTotal = timeFade;
                ActionType = AUDIO_ACTION_TYPE.FADE_IN;
            }
        }

        public void UpdateVolume()
        {
            if (AudioTrack == null) return;
            if (AudioSourceComponent.clip != null)
            {
                AudioSourceComponent.volume = _volume * GlobalVolume;
            }
        }

        public void StopSound(float timeFade = 0.0f)
        {
            if (AudioTrack == null) return;

            if (0.0f < timeFade)
            {
                ActionType = AUDIO_ACTION_TYPE.FADE_OUT;
                State = SOURCE_STATE.ACTIVE;

                _volumeFrom = _volume;
                _volumeTo = 0.0f;
                _timeTotal = timeFade;
                _timeValue = timeFade;
            }
            else
            {
                gameObject.SetActive(false);
                State = SOURCE_STATE.PASSIVE;
                ActionType = AUDIO_ACTION_TYPE.OFF;
                AudioTrack = null;
                AudioSourceComponent.Stop();
                AudioSourceComponent.clip = null;
            }
        }

        public void SetEndCallBack(System.Action action)
        {
            _audioEndCallBack = null;
            _audioEndCallBack = action;
        }

        private void Update()
        {
            if (State == SOURCE_STATE.PASSIVE) return;

            if (State == SOURCE_STATE.ACTIVE && AudioType == AUDIO_TYPE.EFFECT && !AudioSourceComponent.loop && 0.0f < PlayTime)
            {
                PlayTime -= Time.deltaTime / Time.timeScale;
                if (PlayTime <= 0.0f)
                {
                    PlayTime = -1.0f;
                    StopSound();
                    _audioEndCallBack?.Invoke();
                    return;
                }
            }

            if (ActionType == AUDIO_ACTION_TYPE.FADE_OUT)
            {
                _timeValue -= Time.deltaTime / Time.timeScale;
                if (_timeValue <= 0.0f)
                {
                    _timeValue = 0.0f;
                    StopSound();
                    _audioEndCallBack?.Invoke();
                }
                else
                {
                    _volume = Mathf.Lerp(_volumeFrom, _volumeTo, 1.0f - (_timeValue / _timeTotal));
                    AudioSourceComponent.volume = _volume * GlobalVolume;
                }

                return;
            }
            else
            if (ActionType == AUDIO_ACTION_TYPE.FADE_IN)
            {
                _timeValue -= Time.deltaTime / Time.timeScale;
                if (_timeValue <= 0.0f)
                {
                    _timeValue = 0.0f;
                    ActionType = AUDIO_ACTION_TYPE.PLAYING;
                }
                _volume = Mathf.Lerp(_volumeFrom, _volumeTo, 1.0f - (_timeValue / _timeTotal));
                AudioSourceComponent.volume = _volume * GlobalVolume;

                return;
            }
        }
    }
}
