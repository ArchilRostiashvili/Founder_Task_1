using BebiLibs.AudioSystem;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will allow you to play AudioTrack.")]
    [FeedbackPath("Audio/Audio Track")]
    public class MMF_AudioTrack : MMF_Feedbacks
    {
        [MMFInspectorGroup("Audiotrack", true, 28, true)]
        [SerializeField] private AudioTrackBaseSO _audioTrack;

        [SerializeField] private bool _playAudio;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || (_audioTrack == null))
            {
                return;
            }

            if (_playAudio)
            {
                _audioTrack.Play();
            }
            else
            {
                _audioTrack.Stop();
            }
        }

        public void SetAudio(AudioTrackBaseSO audioTrackBaseSo)
        {
            _audioTrack = audioTrackBaseSo;
        }
    }
}

