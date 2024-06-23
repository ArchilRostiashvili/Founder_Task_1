using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    public abstract class AudioTrackBaseSO : ScriptableObject
    {
        public abstract AudioSystemSource Play(System.Action<string> subtitlesCallback = null);
        public abstract void Stop();
        public abstract bool IsPlaying();
        public abstract List<AudioSystemSource> GetPlayingAudioSources();
    }
}
