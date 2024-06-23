using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ItemAudioSource : MonoBehaviour
    {
        public AudioSource source;
        public DataSound dataSound;

        public AUDIO_ACTION_TYPE actionType;
        public SOURCE_STATE state;
        public AUDIO_TYPE audioType;

        private System.Action _audioEndCallBack;
        private float _volume;
        private float _volumeFrom;
        private float _volumeTo;
        private float _timeValue;
        private float _timeTotal;

        public float playTime;

        public void PlayEffect(DataSound dataSound, float timeFade = 0.0f, float timeDelay = 0.0f, float volume = 1f, bool loop = false)
        {
            if (dataSound.sound == null) return;

            if (dataSound.IsLocal) dataSound.LoadAudioData();

            this.gameObject.SetActive(true);
            this.dataSound = dataSound;
            this.audioType = AUDIO_TYPE.EFFECT;
            this.state = SOURCE_STATE.IN_PROCESS;
            this.actionType = AUDIO_ACTION_TYPE.NONE;

            if (-0.0000001f < timeFade)
            {
                this.ChangeVolume(0.0f, 0.0f);
                this.ChangeVolume(volume * this.dataSound.volume, timeFade);
            }
            else if (timeFade > 0.00000001f)
            {
                this.ChangeVolume(volume * this.dataSound.volume, timeFade);
            }
            else
            {
                this.ChangeVolume(volume, 0);
            }

            this.PlaySound(loop, timeDelay);
        }

        public void PlayBackground(DataSound dataSound, float timeFade = 0.0f, float timeDelay = 0.0f, float volume = 1.0f)
        {
            if (dataSound.sound == null) return;

            if (dataSound.IsLocal) dataSound.LoadAudioData();

            this.gameObject.SetActive(true);
            this.dataSound = dataSound;
            this.audioType = AUDIO_TYPE.BACKGROUND;
            this.state = SOURCE_STATE.IN_PROCESS;
            this.actionType = AUDIO_ACTION_TYPE.NONE;

            if (0.0f < timeFade)
            {
                this.ChangeVolume(0.0f, 0.0f);
                this.ChangeVolume(volume * this.dataSound.volume, timeFade);
            }
            else
            {
                this.ChangeVolume(volume * this.dataSound.volume, timeFade);
            }

            this.PlaySound(true, timeDelay);
        }

        private void PlaySound(bool loop, float delay = 0.0f)
        {
            this.state = SOURCE_STATE.ACTIVE;

            this.source.volume = _volume * this.GlobalVolume;
            this.source.loop = loop;
            this.source.clip = this.dataSound.sound;
            playTime = this.source.clip.length + 0.5f + delay;

            if (0.0f < delay)
            {
                this.source.PlayDelayed(delay);
            }
            else
            {
                this.source.Play();
            }
        }

        private float GlobalVolume
        {
            get
            {
                if (this.audioType == AUDIO_TYPE.BACKGROUND)
                {
                    return ManagerSounds.volumeGlobalBackgrounds;
                }
                else
                {
                    return ManagerSounds.volumeGlobalEffects;
                }
            }
        }

        public void ChangeVolume(float volumeTo, float timeFade = 0.0f)
        {
            if (this.dataSound == null) return;
            if (this.state == SOURCE_STATE.PASSIVE) return;

            if (timeFade == 0.0f)
            {
                _volume = volumeTo;
                this.source.volume = _volume * this.GlobalVolume;
                this.actionType = AUDIO_ACTION_TYPE.NONE;
            }
            else
            {
                _volumeFrom = _volume;
                _volumeTo = volumeTo;
                _timeValue = timeFade;
                _timeTotal = timeFade;
                this.actionType = AUDIO_ACTION_TYPE.FADE_IN;
            }
        }

        public void UpdateVolume()
        {
            if (this.dataSound == null) return;
            if (this.source.clip != null)
            {
                this.source.volume = _volume * this.GlobalVolume;
            }
        }

        public void StopSound(float timeFade = 0.0f)
        {
            if (this.dataSound == null) return;

            if (0.0f < timeFade)
            {
                this.actionType = AUDIO_ACTION_TYPE.FADE_OUT;
                this.state = SOURCE_STATE.ACTIVE;

                _volumeFrom = _volume;
                _volumeTo = 0.0f;
                _timeTotal = timeFade;
                _timeValue = timeFade;
            }
            else
            {
                this.gameObject.SetActive(false);
                this.state = SOURCE_STATE.PASSIVE;
                this.actionType = AUDIO_ACTION_TYPE.OFF;
                this.dataSound = null;
                this.source.Stop();
                this.source.clip = null;
            }
        }

        public void SetEndCallBack(System.Action action)
        {
            _audioEndCallBack = null;
            _audioEndCallBack = action;
        }

        private void Update()
        {
            if (this.state == SOURCE_STATE.PASSIVE) return;

            if (this.state == SOURCE_STATE.ACTIVE && this.audioType == AUDIO_TYPE.EFFECT && !this.source.loop && 0.0f < playTime)
            {
                playTime -= Time.deltaTime / Time.timeScale;
                if (playTime <= 0.0f)
                {
                    playTime = -1.0f;
                    this.StopSound();
                    _audioEndCallBack?.Invoke();
                    return;
                }
            }

            if (this.actionType == AUDIO_ACTION_TYPE.FADE_OUT)
            {
                _timeValue -= Time.deltaTime / Time.timeScale;
                if (_timeValue <= 0.0f)
                {
                    _timeValue = 0.0f;
                    this.StopSound();
                    _audioEndCallBack?.Invoke();
                }
                else
                {
                    _volume = Mathf.Lerp(_volumeFrom, _volumeTo, 1.0f - (_timeValue / _timeTotal));
                    this.source.volume = _volume * this.GlobalVolume;
                }

                return;
            }
            else
            if (this.actionType == AUDIO_ACTION_TYPE.FADE_IN)
            {
                _timeValue -= Time.deltaTime / Time.timeScale;
                if (_timeValue <= 0.0f)
                {
                    _timeValue = 0.0f;
                    this.actionType = AUDIO_ACTION_TYPE.PLAYING;
                }
                _volume = Mathf.Lerp(_volumeFrom, _volumeTo, 1.0f - (_timeValue / _timeTotal));
                this.source.volume = _volume * this.GlobalVolume;

                return;
            }
        }
    }
}