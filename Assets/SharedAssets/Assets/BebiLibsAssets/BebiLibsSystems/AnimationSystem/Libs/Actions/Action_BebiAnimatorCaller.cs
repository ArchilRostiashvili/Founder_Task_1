using BebiAnimations.Libs.Core;
using UnityEngine;

namespace BebiAnimations.Libs.Actions
{
    public class Action_BebiAnimatorCaller : AnimationAction
    {
        public enum CallerType { PLAY, REVERT, STOP }

        [SerializeField] private BebiAnimator _bebiAnimator;
        [SerializeField] private string _animationID;
        [SerializeField] private bool _playWithAnim;
        [SerializeField] private bool _noCallBack;
        [SerializeField] private CallerType _callerType;

        protected override void ActionPlay()
        {
            if (_callerType == CallerType.PLAY)
            {
                if (_noCallBack)
                {
                    _animDuration = 0.0f;
                    _animOn = false;
                    _bebiAnimator.Play(_animationID, _playWithAnim);
                }
                else
                {
                    _animDuration = 0.1f;
                    _animOn = true;
                    _bebiAnimator.Play(_animationID, _playWithAnim, () =>
                    {
                        Done();
                    });
                }
            }
            else
            if (_callerType == CallerType.REVERT)
            {

                //_animState = AnimState.ANIM_OFF;
                _bebiAnimator.Revert(_animationID);
                Done();
            }
            else
            if (_callerType == CallerType.STOP)
            {
                //_animState = AnimState.ANIM_OFF;
                _bebiAnimator.Stop(_animationID);
                Done();
            }
        }
    }
}
