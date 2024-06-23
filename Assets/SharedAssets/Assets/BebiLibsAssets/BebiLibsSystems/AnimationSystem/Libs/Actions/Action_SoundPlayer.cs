using UnityEngine;
using BebiLibs.AudioSystem;
using BebiAnimations.Libs.Core;

namespace BebiAnimations.Libs.Actions
{
    [System.Serializable]
    public class Action_SoundPlayer : AnimationAction
    {
        [SerializeField] private AudioTrackBaseSO _audioTrack;
        [SerializeField] private bool _playSound = true;

        protected override void ActionPlay()
        {
            if (_playSound)
            {
                _audioTrack.Play();
            }
            else
            {
                _audioTrack.Stop();
            }
        }

        protected override void ActionStop()
        {
            // _audioTrack.Stop();
        }
    }
}
