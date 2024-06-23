using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [System.Serializable]
    public class AudioTrackSetting
    {
        public bool IsLooping = false;

        [Range(0f, 1f)] public float Volume = 1f;
        [Range(0f, 10f)] public float TimeFade = 0;
        [Range(0f, 10f)] public float TimeDelay = 0;

        public AudioTrackSetting(bool isLooping = false, float volume = 1, float timeFade = 0, float timeDelay = 0)
        {
            IsLooping = isLooping;
            Volume = volume;
            TimeFade = timeFade;
            TimeDelay = timeDelay;
        }

        public AudioTrackSetting(AudioTrackSetting other)
        {
            IsLooping = other.IsLooping;
            Volume = other.Volume;
            TimeFade = other.TimeFade;
            TimeDelay = other.TimeDelay;
        }

        public AudioTrackSetting(AudioTrackSettingsSO other)
        {
            IsLooping = other.IsLooping;
            Volume = other.Volume;
            TimeFade = other.TimeFade;
            TimeDelay = other.TimeDelay;
        }
    }
}
