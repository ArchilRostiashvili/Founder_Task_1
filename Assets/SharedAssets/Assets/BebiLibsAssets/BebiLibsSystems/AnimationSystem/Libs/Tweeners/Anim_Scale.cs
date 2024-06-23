using BebiAnimations.Libs.Core;
using DG.Tweening;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Scale : AnimationTween
    {
        [SerializeField] private Transform _objectTR;

        [SerializeField] private bool _useRelativeScale;
        [SerializeField] private Vector3 _toScaleValue = Vector3.one;
        [SerializeField] private Vector3 _initialScale = Vector3.one;


        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialScale = _objectTR.localScale;
            }
        }

        public void SetEndValue(Vector3 scaleValue)
        {
            _toScaleValue = scaleValue;
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();
                if (_resetOnPlay)
                {
                    _objectTR.localScale = _initialScale;
                }

                _tween = DoScale().SetRelative(_useRelativeScale).OnComplete(() =>
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
                if (_useRelativeScale)
                {
                    _objectTR.localScale = _initialScale + _toScaleValue;
                }
                else
                {
                    _objectTR.localScale = _toScaleValue;
                }
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _objectTR.localScale = _initialScale;
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }

        private Tween DoScale()
        {
            return _objectTR.DOScale(_toScaleValue, _animDuration);
        }
    }
}
