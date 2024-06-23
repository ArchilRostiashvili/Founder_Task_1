using BebiAnimations.Libs.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_TextColor : AnimationTween
    {
        [SerializeField] private TextMeshPro _textMesh;

        [SerializeField] private Color _targetColor;
        [SerializeField] private Color _initialColor;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialColor = _textMesh.color;
            }
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();

                if (_resetOnPlay)
                {
                    _textMesh.color = _initialColor;
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
                _textMesh.color = _targetColor;
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _textMesh.color = _initialColor;
        }

        private Tween DoColor()
        {
            return _textMesh.DOColor(_targetColor, _animDuration);
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
