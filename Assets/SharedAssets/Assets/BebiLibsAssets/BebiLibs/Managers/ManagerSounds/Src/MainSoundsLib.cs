//#define IS_NATIVE_ACTIVE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Reflection;
using System.IO;

namespace BebiLibs
{
    public enum AUDIO_ACTION_TYPE { NONE, PLAYING, FADE_OUT, FADE_IN, OFF };
    public enum AUDIO_TYPE { EFFECT, BACKGROUND };
    public enum SOURCE_STATE { PASSIVE, ACTIVE, IN_PROCESS };

    [System.Serializable]
    public class DataSound
    {
        public string soundName;
        public AudioClip sound;
        public float volume = 1.0f;
        private bool _isTemp;
        private bool _isLocal;

        public static DataSound Create(string soundName, float volume, bool isTemp, bool isLocal)
        {
            DataSound dataSound = new DataSound();
            dataSound.soundName = soundName;
            dataSound.volume = volume;
            dataSound._isTemp = isTemp;
            dataSound._isLocal = isLocal;
            dataSound.Init();

            return dataSound;
        }

        public static DataSound Create(AudioClip audioClip, float volume, bool isTemp, bool isLocal)
        {
            DataSound dataSound = new DataSound();
            dataSound.soundName = audioClip.name;
            dataSound.sound = audioClip;
            dataSound.volume = volume;
            dataSound._isTemp = isTemp;
            dataSound._isLocal = isLocal;

            return dataSound;
        }


        public static DataSound Create(AudioClip audioClip, string soundNameAlias, float volume, bool isTemp, bool isLocal)
        {
            DataSound dataSound = new DataSound();
            dataSound.soundName = soundNameAlias;
            dataSound.sound = audioClip;
            dataSound.volume = volume;
            dataSound._isTemp = isTemp;
            dataSound._isLocal = isLocal;

            return dataSound;
        }

        public bool IsTemp
        {
            get => _isTemp;
            set => _isTemp = value;
        }

        public bool IsLocal
        {
            get => _isLocal;
            set => _isLocal = value;
        }

        public void Init()
        {
            if (sound == null)
            {
                try
                {
                    sound = (AudioClip)Resources.Load(soundName);
                }
                catch (FileNotFoundException e)
                {
                    Common.DebugLog("error - " + e.Message);
                }
            }
            else
            {
                soundName = sound.name;
            }

            if (_isLocal)
            {
                sound.UnloadAudioData();
            }
        }

        public void LoadAudioData()
        {
            if (sound != null)
            {
                sound.LoadAudioData();
            }
        }

        public void UnloadAudioData()
        {
            if (sound != null)
            {
                sound.UnloadAudioData();
            }
        }

        public void Unload()
        {
            if (sound != null)
            {
                sound.UnloadAudioData();
                //GameObject.Destroy(sound);
                sound = null;
            }
        }
    }

    [CreateAssetMenu(fileName = "MainSoundsLib", menuName = "BebiLibs/ManagerSound/MainSoundsLib/Create")]
    public class MainSoundsLib : ScriptableObject
    {
        public DataSoundsLib dataSoundsLib;
    }
}
