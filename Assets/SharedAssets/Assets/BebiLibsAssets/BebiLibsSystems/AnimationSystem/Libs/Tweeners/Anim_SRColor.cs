using BebiAnimations.Libs.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_SRColor : AnimationTween
    {
        [SerializeField] private SpriteRenderer _objectSR;
        [SerializeField] private Color _targetColor = Color.white;
        [SerializeField] private Color _initialColor = Color.white;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialColor = _objectSR.color;
            }
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();

                if (_resetOnPlay)
                {
                    _objectSR.color = _initialColor;
                }

                _tween = DoColor().OnComplete(() =>
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
                _objectSR.color = _targetColor;
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _objectSR.color = _initialColor;
        }

        private Tween DoColor()
        {
            return _objectSR.DOColor(_targetColor, _animDuration);
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }

        public void SetColor(Color color)
        {
            _targetColor = color;
        }
    }

}
