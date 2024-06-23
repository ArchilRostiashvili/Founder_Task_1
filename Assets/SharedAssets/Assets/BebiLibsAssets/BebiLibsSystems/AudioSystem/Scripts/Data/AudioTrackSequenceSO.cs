using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audio System/New Audio Sequence")]
    public class AudioTrackSequenceSO : PlayableAudioTrackSO
    {
        [SerializeField] private bool _replayIfAlreadyPlaying;
        [SerializeField] private List<SequenceTrackData> _audioTrackDelayDataList = new List<SequenceTrackData>();

        private AudioTrackSO _playingAudioTrack;

        public bool ReplayEvenIfPlaying => _replayIfAlreadyPlaying;
        public string TrackID => this.ToString();

        public override AudioSystemSource Play(System.Action<string> subtitlesCallback = null)
        {
            AudioManager.PlaySequence(this, subtitlesCallback);
            return null;
        }

        public override void Stop()
        {
            AudioManager.StopSequence(this);
            foreach (var item in _audioTrackDelayDataList)
            {
                if (item != null && item.AudioTrack != null)
                {
                    item.AudioTrack.Stop();
                }
            }
        }

        public IEnumerator PlayAudioSequence(System.Action<string> subtitlesCallback = null)
        {
            for (int i = 0; i < _audioTrackDelayDataList.Count; i++)
            {
                if (_audioTrackDelayDataList[i] != null && _audioTrackDelayDataList[i].AudioTrack != null)
                {
                    _audioTrackDelayDataList[i].AudioTrack.Play();
                    _playingAudioTrack = _audioTrackDelayDataList[i].AudioTrack;
                    subtitlesCallback?.Invoke(_audioTrackDelayDataList[i].AudioTrack.Subtitle);
                    yield return new WaitForSeconds(_audioTrackDelayDataList[i].AudioTrack.AudioClip.length + _audioTrackDelayDataList[i].AdditionalDelay);
                }
            }
        }


        public void MargeSequence(List<AudioTrackSO> additionalTracksList = null)
        {
            if (additionalTracksList == null || 0 == additionalTracksList.Count) return;

            int additionalTrackIndex = 0;
            for (int i = 0; i < _audioTrackDelayDataList.Count; i++)
            {
                if (_audioTrackDelayDataList[i].IsPlaceHolder)
                {
                    if (additionalTrackIndex < additionalTracksList.Count)
                    {
                        _audioTrackDelayDataList[i].SetAudioTrack(additionalTracksList[additionalTrackIndex]);
                        additionalTrackIndex++;
                    }
                }
            }
        }

        public void ClearAudioTrackList()
        {
            _audioTrackDelayDataList.Clear();
        }

        public void AddAudioTrack(AudioTrackSO item)
        {
            _audioTrackDelayDataList.Add(new SequenceTrackData(item));
        }

        public override bool IsPlaying()
        {
            return _playingAudioTrack != null && _playingAudioTrack.IsPlaying();
        }

        public override List<AudioSystemSource> GetPlayingAudioSources()
        {
            List<AudioSystemSource> audioSystemSources = new List<AudioSystemSource>();
            if (_playingAudioTrack != null)
            {
                audioSystemSources.AddRange(_playingAudioTrack.GetPlayingAudioSources());
            }
            return audioSystemSources;
        }
    }
}
