using BebiAnimations.Libs.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_MoveTo : AnimationTween
    {
        [SerializeField] protected Transform _objectTR;
        [SerializeField] protected Space _space = Space.World;
        [SerializeField] protected Transform _fromPointTR;
        [SerializeField] protected Transform _toPointTR;
        [SerializeField] protected Vector3 _initialPoint;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                if (_fromPointTR != null)
                {
                    _initialPoint = GetPoint(_fromPointTR, _space);
                }
                else
                {
                    _initialPoint = GetPoint(_objectTR, _space);
                }
            }
        }

        public void SetFromPoint(Vector3 point)
        {
            SetPoint(_fromPointTR, point, _space);
        }

        public void SetToPoint(Vector3 point)
        {
            SetPoint(_toPointTR, point, _space);
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();
                if (_resetOnPlay)
                {
                    if (_fromPointTR != null)
                    {
                        SetPoint(_objectTR, GetPoint(_fromPointTR, _space), _space);
                    }
                    else
                    {
                        SetPoint(_objectTR, _initialPoint, _space);
                    }
                }

                _tween = DoMove().OnComplete(() =>
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
                SetPoint(_objectTR, GetPoint(_toPointTR, _space), _space);
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();

            if (_fromPointTR != null)
            {
                SetPoint(_objectTR, GetPoint(_fromPointTR, _space), _space);
            }
            else
            {
                SetPoint(_objectTR, _initialPoint, _space);
            }
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }

        protected virtual Tween DoMove()
        {
            if (_space == Space.Self)
            {
                return _objectTR.DOLocalMove(GetPoint(_toPointTR, _space), _animDuration);
            }
            else
            {
                return _objectTR.DOMove(GetPoint(_toPointTR, _space), _animDuration);
            }
        }
    }
}
