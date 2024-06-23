using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = ("Audio System/New track settings"))]
    public class AudioTrackSettingsSO : ScriptableObject
    {
        public AudioTrackSetting audioTrackSetting;

        public bool IsLooping => audioTrackSetting.IsLooping;
        public float Volume => audioTrackSetting.Volume;
        public float TimeFade => audioTrackSetting.TimeFade;
        public float TimeDelay => audioTrackSetting.TimeDelay;
    }
}
