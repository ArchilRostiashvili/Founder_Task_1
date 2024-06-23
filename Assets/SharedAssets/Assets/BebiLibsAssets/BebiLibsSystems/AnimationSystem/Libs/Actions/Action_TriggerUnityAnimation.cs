using BebiAnimations.Libs.Core;
using UnityEngine;

namespace BebiAnimations.Libs.Actions
{
    public class Action_TriggerUnityAnimation : AnimationAction
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _triggerID;

        protected override void ActionPlay()
        {
            _animator.SetTrigger(_triggerID);
        }
    }
}
