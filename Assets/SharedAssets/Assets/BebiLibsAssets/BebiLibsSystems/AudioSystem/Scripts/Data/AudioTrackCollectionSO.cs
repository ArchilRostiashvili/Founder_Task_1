using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audio System/New Audio Collection")]
    public class AudioTrackCollectionSO : PlayableAudioTrackSO
    {
        public List<AudioTrackBaseSO> AudioTracksList = new List<AudioTrackBaseSO>();
        public bool IsRandom;

        private AudioTrackBaseSO _currentSelectedTrack;

        public override AudioSystemSource Play(Action<string> subtitlesCallback = null)
        {
            if (AudioTracksList.Count == 0) return null;

            if (AudioTracksList.Contains(this))
            {
                Debug.LogWarning("You are trying to play the same audio track. This will cause an infinite cycle.");
                return null;
            }

            _currentSelectedTrack = IsRandom ? AudioTracksList.GetRandomElement() : AudioTracksList[0];
            return _currentSelectedTrack.Play(subtitlesCallback);
        }

        public override void Stop()
        {
            if (AudioTracksList.Count == 0) return;

            if (AudioTracksList.Contains(this))
            {
                Debug.LogWarning("You are trying to stop the same audio track. This will cause an infinite cycle.");
                return;
            }

            _currentSelectedTrack.Stop();
        }

        public void ClearAudioTrackList()
        {
            AudioTracksList.Clear();
        }

        public void AddAudioTrack(AudioTrackBaseSO audioTrack)
        {
            AudioTracksList.Add(audioTrack);
        }

        public override bool IsPlaying()
        {
            if (AudioTracksList.Count == 0) return false;

            if (AudioTracksList.Contains(this))
            {
                Debug.LogWarning("You are trying to check if the same audio track is playing. This will cause an infinite cycle.");
                return false;
            }

            return _currentSelectedTrack.IsPlaying();
        }

        public override List<AudioSystemSource> GetPlayingAudioSources()
        {
            if (AudioTracksList.Count == 0) return new List<AudioSystemSource>();

            if (AudioTracksList.Contains(this))
            {
                Debug.LogWarning("You are trying to get playing audio sources from the same audio track. This will cause an infinite cycle.");
                return new List<AudioSystemSource>();
            }

            return _currentSelectedTrack.GetPlayingAudioSources();
        }
    }
}
