using BebiAnimations.Libs.Core;
using DG.Tweening;
using System;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Shake : AnimationTween
    {
        [SerializeField] private Transform _objectTR;

        [SerializeField] private float _strength = 20.0f;
        [SerializeField] private int _vibration = 2;
        [SerializeField] private float _randomness = 5.0f;
        [SerializeField] private bool _fadeOut = false;
        [SerializeField] private Vector3 _initialRotation;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialRotation = _objectTR.localEulerAngles;
            }
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();
                if (_resetOnPlay)
                {
                    _objectTR.eulerAngles = _initialRotation;
                }

                _tween = DoShake().OnComplete(() =>
                {
                    Done();
                });

                if (_tweenSetting != null)
                {
                    _tweenSetting.ApplySetting(_tween);
                }
                else
                {
                    _tween.SetEase(_easeType);
                }
                _tween.SetId(this);
            }
            else
            {
                this.DOKillAnim();
                _objectTR.localScale = _initialRotation;
            }
        }

        private Tween DoShake()
        {
            return _objectTR.DOShakeRotation(_animDuration, _strength, _vibration, _randomness, _fadeOut);
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _objectTR.localEulerAngles = _initialRotation;
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }
    }
}