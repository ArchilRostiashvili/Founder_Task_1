using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audio System/New Audio Skip")]
    public class AudioTrackSkipSO : PlayableAudioTrackSO
    {
        public AudioTrackBaseSO AudioTrack;
        public int SkipValue;
        private int _index = -1;

        public override List<AudioSystemSource> GetPlayingAudioSources()
        {
            if (object.ReferenceEquals(AudioTrack, this))
            {
                Debug.LogWarning("You are trying to get playing audio sources from the same audio track. This will cause an infinite cycle.");
                return new List<AudioSystemSource>();
            }

            return AudioTrack.GetPlayingAudioSources();
        }

        public override bool IsPlaying()
        {
            if (object.ReferenceEquals(AudioTrack, this))
            {
                Debug.LogWarning("You are trying to check if the same audio track is playing. This will cause an infinite cycle.");
                return false;
            }

            return AudioTrack.IsPlaying();
        }

        public override AudioSystemSource Play(System.Action<string> subtitlesCallback = null)
        {
            if (object.ReferenceEquals(AudioTrack, this))
            {
                Debug.LogWarning("You are trying to play the same audio track. This will cause an infinite cycle.");
                return null;
            }

            _index++;
            if (_index % SkipValue == 0)
            {
                return AudioTrack.Play(subtitlesCallback);
            }


            return null;
        }

        public override void Stop()
        {
            if (object.ReferenceEquals(AudioTrack, this))
            {
                Debug.LogWarning("You are trying to stop the same audio track. This will cause an infinite cycle.");
                return;
            }

            AudioTrack.Stop();
        }
    }
}
