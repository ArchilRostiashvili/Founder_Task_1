
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BebiLibs.Localization;

namespace BebiLibs
{
    public class ManagerSounds : MonoBehaviour
    {
        public const string SOUND_ON = "sound_on";
        public const string SOUND_EFFECT_ON = "sound_effect_on";
        public const string SOUND_BACKGROUND_ON = "sound_background_on";

        private static ManagerSounds _instance;

        public List<ItemAudioSource> arrayItemAudioSources;

        public DataSoundsLib dataSoundsLib;

        public static float volumeGlobalEffects;
        public static float volumeGlobalBackgrounds;

        private static bool _soundOn;
        private static bool _soundOnEffects;
        private static bool _soundOnBackgrounds;

        public List<DataSound> _arrayDataSounds;

        public MainSoundsLib dataMainSoundsLib;

        public static Action CallBackChanged;
        [SerializeField] private bool _logInEditor;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
                InitContent();
            }
            else if (_instance != this)
            {
                ManagerSounds.AddLib(dataSoundsLib, true, false);
                GameObject.Destroy(gameObject);
                return;
            }
        }

        public void InitContent()
        {
            _soundOn = PlayerPrefs.GetInt(SOUND_ON, 1) == 1;
            _soundOnEffects = PlayerPrefs.GetInt(SOUND_EFFECT_ON, 1) == 1;
            _soundOnBackgrounds = PlayerPrefs.GetInt(SOUND_BACKGROUND_ON, 1) == 1;

            ManagerSounds.volumeGlobalEffects = _soundOnEffects ? 1.0f : 0.0f;
            ManagerSounds.volumeGlobalBackgrounds = _soundOnBackgrounds ? 1.0f : 0.0f;
            _arrayDataSounds = new List<DataSound>();

            ManagerSounds.AddLib(dataMainSoundsLib.dataSoundsLib, false, false);
            ManagerSounds.AddLib(dataSoundsLib, false, false);
        }

        public static void UnloadLocals()
        {
            if (!_instance) return;

            foreach (DataSound dataSound in _instance._arrayDataSounds)
            {
                if (dataSound.IsLocal) dataSound.UnloadAudioData();
            }
        }

        public static void AddLib(DataSoundsLib lib, bool isTemp, bool isLocal)
        {
            if (_instance == null) return;
            _instance.AddLibsLocal(lib, isTemp, isLocal);
        }

        public void AddLibsLocal(DataSoundsLib lib, bool isTemp, bool isLocal)
        {
            AddressableAudioManager.UpdateLocalizationData(lib, isTemp);
            SetDataSound(lib, isTemp);
            SetSimpleClips(lib, isTemp, isLocal);
            SetClipPacks(lib, isTemp, isLocal);
            SetClipsFromResources(lib, isTemp, isLocal);
        }

        private void SetClipsFromResources(DataSoundsLib lib, bool isTemp, bool isLocal)
        {
            if (lib.arrayClipsFromRes == null) return;

            for (int i = 0; i < lib.arrayClipsFromRes.Length; i++)
            {
                if (lib.arrayClipsFromRes[i] == null) continue;

                var dataSound = _arrayDataSounds.Find(x => x.soundName == lib.arrayClipsFromRes[i]);
                if (dataSound == null)
                {
                    _arrayDataSounds.Add(DataSound.Create(lib.arrayClipsFromRes[i], 1.0f, isTemp, isLocal));
                }
            }
        }

        private void SetClipPacks(DataSoundsLib lib, bool isTemp, bool isLocal)
        {
            if (lib.arrayAudioClipsPack == null) return;

            foreach (AudioClipPack audioPack in lib.arrayAudioClipsPack)
            {
                foreach (AudioClip audioClip in audioPack)
                {
                    if (audioClip == null) continue;

                    var dataSound = _arrayDataSounds.Find(x => x.sound == audioClip);
                    if (dataSound == null)
                    {
                        _arrayDataSounds.Add(DataSound.Create(audioClip, 1.0f, isTemp, isLocal));
                    }
                }
            }
        }

        private void SetSimpleClips(DataSoundsLib lib, bool isTemp, bool isLocal)
        {
            if (lib.arraySimpleClips == null) return;

            for (int i = 0; i < lib.arraySimpleClips.Count; i++)
            {
                if (lib.arraySimpleClips[i] == null) continue;

                DataSound dataSound = _arrayDataSounds.Find(x => x.sound == lib.arraySimpleClips[i]);
                if (dataSound == null)
                {
                    _arrayDataSounds.Add(DataSound.Create(lib.arraySimpleClips[i], 1.0f, isTemp, isLocal));
                }
            }
        }

        private void SetDataSound(DataSoundsLib lib, bool isTemp)
        {
            if (lib.arrayDataSounds == null) return;

            for (int i = 0; i < lib.arrayDataSounds.Count; i++)
            {
                if (lib.arrayDataSounds[i] == null) continue;
                lib.arrayDataSounds[i].Init();
                lib.arrayDataSounds[i].IsTemp = isTemp;

                var dataSound = _arrayDataSounds.Find(x => x.sound == lib.arrayDataSounds[i].sound);
                if (dataSound == null)
                {
                    _arrayDataSounds.Add(lib.arrayDataSounds[i]);
                }
            }
        }

        public static void UnloadTempAudio()
        {
            if (_instance == null) return;
            _instance.UnloadTempAudioLocal();
        }

        public void UnloadTempAudioLocal()
        {
            for (int i = arrayItemAudioSources.Count - 1; i >= 0; i--)
            {
                if (arrayItemAudioSources[i].dataSound != null && arrayItemAudioSources[i].dataSound.IsTemp)
                {
                    arrayItemAudioSources[i].StopSound();
                }
            }

            for (int i = _arrayDataSounds.Count - 1; i >= 0; i--)
            {
                if (_arrayDataSounds[i].IsTemp)
                {
                    _arrayDataSounds[i].Unload();
                    _arrayDataSounds.RemoveAt(i);
                }
            }
        }

        public static bool SoundsOn
        {
            get
            {
                return _soundOn;
            }
            set
            {
                PlayerPrefs.SetInt(SOUND_ON, value ? 1 : 0);
                _soundOn = value;

                ManagerSounds.SoundOnEffects = _soundOn;
                ManagerSounds.SoundOnBackgrounds = _soundOn;

                ManagerSounds.CallBackChanged?.Invoke();
            }
        }

        public static bool SoundOnEffects
        {
            get
            {
                return _soundOnEffects;
            }
            set
            {
                PlayerPrefs.SetInt(SOUND_EFFECT_ON, value ? 1 : 0);
                _soundOnEffects = value;
                ManagerSounds.volumeGlobalEffects = _soundOnEffects ? 1.0f : 0.0f;
                ManagerSounds.UpdateVolume();
            }
        }

        public static bool SoundOnBackgrounds
        {
            get
            {
                return _soundOnBackgrounds;
            }
            set
            {
                PlayerPrefs.SetInt(SOUND_BACKGROUND_ON, value ? 1 : 0);
                _soundOnBackgrounds = value;
                ManagerSounds.volumeGlobalBackgrounds = _soundOnBackgrounds ? 1.0f : 0.0f;
                ManagerSounds.UpdateVolume();
            }
        }

        public static void UpdateVolume()
        {
            if (_instance == null)
            { return; }

            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                _instance.arrayItemAudioSources[j].UpdateVolume();
            }
        }

        public static void StopSound(string soundName, float timeFade = 0.0f)
        {
            if (_instance == null) return;

            if (_instance._logInEditor && Application.isEditor)
            {
                Debug.LogWarning("Stop Sound " + soundName); ;
            }


            ItemAudioSource itemAudioSource;
            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                itemAudioSource = _instance.arrayItemAudioSources[j];
                if (itemAudioSource.dataSound != null && itemAudioSource.dataSound.soundName == soundName)
                {
                    itemAudioSource.StopSound(timeFade);
                }
            }
        }

        public static void StopAllSounds()
        {
            if (_instance == null) return;


            if (_instance._logInEditor && Application.isEditor)
            {
                Debug.LogWarning("Stop All Sound ");
            }


            _instance.StopAllSoundLocal();
        }

        private void StopAllSoundLocal()
        {
            _isStopAllSoundCalled = true;
            for (int j = 0; j < arrayItemAudioSources.Count; j++)
            {
                arrayItemAudioSources[j].StopSound(0.0f);
            }
        }

        private static ItemAudioSource GetFreeAudioSource()
        {
            if (_instance == null)
            { return null; }

            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                if (_instance.arrayItemAudioSources[j].state == SOURCE_STATE.PASSIVE)
                {
                    return _instance.arrayItemAudioSources[j];
                }
            }
            return ManagerSounds.CreateAudioSource();
        }

        private static ItemAudioSource CreateAudioSource()
        {
            if (_instance == null)
            { return null; }

            GameObject go = GameObject.Instantiate(_instance.arrayItemAudioSources[0].gameObject, _instance.transform);
            ItemAudioSource itemAudioSource = go.GetComponent<ItemAudioSource>();
            itemAudioSource.StopSound();
            _instance.arrayItemAudioSources.Add(itemAudioSource);
            return itemAudioSource;
        }

        public static bool HasLocalizedSound(string soundName)
        {
            if (_instance == null)
            {
                Debug.LogError("Instance Of Manager Sound not exit in current scene, Make sure to instantiate Manager Sound Before Calling this method");
                return false;
            }
            if (AddressableAudioManager.GetDataSound(soundName, out DataSound dataSoundLoaded))
            {
                return dataSoundLoaded != null && dataSoundLoaded.sound != null;
            }
            else
            {
                return false;
            }
        }


        private static Dictionary<string, int> _soundPlayData = new Dictionary<string, int>();


        public static ItemAudioSource PlayEffect(string soundName, float timeFade = 0.0f, float timeDelay = 0.0f, bool loop = false, System.Action action = null, float volume = 1f)
        {
            if (_instance != null && _instance._logInEditor && Application.isEditor)
            {
                Debug.Log("PlayEffect " + soundName); ;
            }

            if (string.IsNullOrEmpty(soundName))
            {
                Debug.LogError("Sound Name is Empty");
                return null;
            }

            if (_soundPlayData.ContainsKey(soundName))
            {
                if (Time.frameCount - _soundPlayData[soundName] < 2)
                {
                    if (_instance._isStopAllSoundCalled)
                    {
                        Debug.Log($"StopAllSound was called before playing sound: {soundName} twice, that was already playing, it will still play, but make sure it not interfere with other sounds");
                    }
                    else
                    {
                        Debug.Log("Sound " + soundName + " is already playing");
                        return null;
                    }
                }
                else
                {
                    _instance._isStopAllSoundCalled = false;
                    _soundPlayData[soundName] = Time.frameCount;
                }
            }
            else
            {
                _soundPlayData.Add(soundName, Time.frameCount);
            }

            DataSound dataSound;
            if (_instance == null)
            {
                Debug.LogError("Instance Of Manager Sound not exit in current scene, Make sure to instantiate Manager Sound Before Calling this method");
                return null;
            }
            else if (AddressableAudioManager.GetDataSound(soundName, out DataSound dataSoundLoaded))
            {
                dataSound = dataSoundLoaded;
            }
            else
            {
                dataSound = ManagerSounds.GetAudioClip(soundName);
            }

            if (dataSound != null)
            {
                //Debug.Log(soundName + " | " + dataSound.sound.name);
                ItemAudioSource itemAudioSource = ManagerSounds.GetFreeAudioSource();
                itemAudioSource.SetEndCallBack(action);
                itemAudioSource.PlayEffect(dataSound, timeFade, timeDelay, volume, loop);
                return itemAudioSource;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("No audio file - " + soundName);
#endif
            }
            return null;
        }

        private static ItemAudioSource PlayEffect(DataSound dataSound, float timeFade = 0.0f, float timeDelay = 0.0f, bool loop = false, System.Action action = null)
        {
            if (dataSound != null)
            {
                ItemAudioSource itemAudioSource = ManagerSounds.GetFreeAudioSource();
                itemAudioSource.SetEndCallBack(action);
                itemAudioSource.PlayEffect(dataSound, timeFade, timeDelay, 1, loop);
                return itemAudioSource;
            }
            else
            {
                Debug.Log("No data sound");
            }
            return null;
        }

        public static ItemAudioSource PlayEffect(AudioClip audioClip, float timeFade = 0.0f, float timeDelay = 0.0f, bool loop = false, System.Action action = null)
        {
            ItemAudioSource itemAudioSource = ManagerSounds.GetFreeAudioSource();
            itemAudioSource.SetEndCallBack(action);
            itemAudioSource.PlayEffect(DataSound.Create(audioClip, 1, true, true), timeFade, timeDelay, 1, loop);
            return itemAudioSource;
        }

        public static ItemAudioSource PlayBackground(string soundName, float timeFade = 0.0f, float timeDelay = 0.0f, float volume = 1.0f)
        {
            DataSound dataSound = ManagerSounds.GetAudioClip(soundName);
            if (dataSound != null)
            {
                ItemAudioSource itemAudioSource = ManagerSounds.GetPlayingAudioSource(soundName);
                if (itemAudioSource != null)
                {
                    itemAudioSource.ChangeVolume(volume, timeFade);
                }
                else
                {
                    itemAudioSource = ManagerSounds.GetFreeAudioSource();
                    itemAudioSource.PlayBackground(dataSound, timeFade, timeDelay, volume);
                }
                return itemAudioSource;
            }
            else
            {
                //Debug.Log("No audio file - " + soundName);
                return null;
            }
        }

        public static void ChangeVolume(string soundName, float volumeTo, float timeFade = 0.0f)
        {
            if (_instance == null)
            { return; }

            ItemAudioSource itemAudioSource;
            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                itemAudioSource = _instance.arrayItemAudioSources[j];
                if (itemAudioSource.dataSound != null && itemAudioSource.dataSound.soundName == soundName)
                {
                    itemAudioSource.ChangeVolume(volumeTo, timeFade);
                }
            }
        }

        public static ItemAudioSource GetPlayingAudioSource(string soundName)
        {
            if (_instance == null)
            { return null; }

            ItemAudioSource itemAudioSource;
            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                itemAudioSource = _instance.arrayItemAudioSources[j];
                if (itemAudioSource.state != SOURCE_STATE.PASSIVE && itemAudioSource.dataSound != null && itemAudioSource.dataSound.soundName == soundName)
                {
                    return itemAudioSource;
                }
            }
            return null;
        }

        public static bool IsPlaying(string soundName)
        {
            if (_instance == null) { return false; }

            ItemAudioSource itemAudioSource;
            for (int j = 0; j < _instance.arrayItemAudioSources.Count; j++)
            {
                itemAudioSource = _instance.arrayItemAudioSources[j];
                if (itemAudioSource.state != SOURCE_STATE.PASSIVE && itemAudioSource.dataSound != null && itemAudioSource.dataSound.soundName == soundName)
                {
                    return true;
                }
            }
            return false;
        }

        public static DataSound GetAudioClip(string soundName)
        {
            if (_instance == null)
            {
                Debug.LogError("Instance Of ManagerSound Is null, Add Manager Sound To Scene");
                return null;
            }
            /*
            if (ManagerLanguage.S_localVoice != null)
            {
                DataSound dataSound = ManagerLanguage.S_localVoice.GetSound(soundName);
                if (dataSound != null)
                {
                    return dataSound;
                }
            }
            */
            for (int j = 0; j < _instance._arrayDataSounds.Count; j++)
            {
                if (_instance._arrayDataSounds[j].sound != null && _instance._arrayDataSounds[j].soundName == soundName)
                {
                    return _instance._arrayDataSounds[j];
                }
            }
            return null;
        }

        //private bool _setTemp = false;
        private bool _soundOnBackgroundsTemp;
        private bool _soundOnEffectsTemp;
        private bool _isStopAllSoundCalled;

        private void OnApplicationFocus(bool active)
        {
            // if (active)
            // {
            //     if (_setTemp)
            //     {
            //         ManagerSounds.SoundOnEffects = _soundOnEffectsTemp;
            //         ManagerSounds.SoundOnBackgrounds = _soundOnBackgroundsTemp;
            //     }
            //     _setTemp = false;
            // }
            // else
            // {
            //     _setTemp = true;
            //     _soundOnBackgroundsTemp = ManagerSounds._soundOnBackgrounds;
            //     _soundOnEffectsTemp = ManagerSounds._soundOnEffects;


            //     ManagerSounds._soundOnBackgrounds = false;
            //     ManagerSounds._soundOnEffects = false;

            //     ManagerSounds.volumeGlobalBackgrounds = _soundOnBackgrounds ? 1.0f : 0.0f;
            //     ManagerSounds.volumeGlobalEffects = _soundOnEffects ? 1.0f : 0.0f;

            //     ManagerSounds.UpdateVolume();
            // }
        }


    }
}
