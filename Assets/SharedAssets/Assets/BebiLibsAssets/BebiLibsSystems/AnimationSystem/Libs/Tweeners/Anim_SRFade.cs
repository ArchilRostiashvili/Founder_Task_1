using BebiAnimations.Libs.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_SRFade : AnimationTween
    {
        [SerializeField] private SpriteRenderer _objectSR;

        [Range(0f, 1f)] [SerializeField] private float _alphaFrom;
        [Range(0f, 1f)] [SerializeField] private float _alphaTo;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                _alphaFrom = _objectSR.color.a;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            _objectSR.sprite = sprite;
        }
        
        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();
                if (_resetOnPlay)
                {
                    _objectSR.SetAlpha(_alphaFrom);
                }

                _tween = DoFade().OnComplete(() => { Done(); });

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
                _objectSR.SetAlpha(_alphaTo);
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _objectSR.SetAlpha(_alphaFrom);
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }

        private Tween DoFade()
        {
            return _objectSR.DOFade(_alphaTo, _animDuration);
        }
    }
}