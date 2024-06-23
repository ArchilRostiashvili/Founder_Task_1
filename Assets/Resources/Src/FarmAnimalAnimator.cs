using System;
using UnityEngine;

namespace Bebi.FarmLife
{
    public class FarmAnimalAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _hasShadow;
        [SerializeField][HideField("_hasShadow", true)] GameObject _shadowRenderer;
        public Animator FarmAnimator => _animator;

        private readonly int _setHappyHash_Trigger = Animator.StringToHash("SetHappy");
        private readonly int _setSadHash_Trigger = Animator.StringToHash("SetSad");
        private readonly int _setExcitedHash_Trigger = Animator.StringToHash("SetExcited");
        private readonly int _setActionHash_Trigger = Animator.StringToHash("SetAction");

        public void SetHappy() => _animator.SetTrigger(_setHappyHash_Trigger);
        public void SetSad() => _animator.SetTrigger(_setSadHash_Trigger);
        public void SetExcited() => _animator.SetTrigger(_setExcitedHash_Trigger);
        public void SetAction() => _animator.SetTrigger(_setActionHash_Trigger);

        internal void EnableShadow(bool enableShadow)
        {
            if (_hasShadow && _shadowRenderer != null)
                _shadowRenderer.SetActive(enableShadow);
        }
    }
}


