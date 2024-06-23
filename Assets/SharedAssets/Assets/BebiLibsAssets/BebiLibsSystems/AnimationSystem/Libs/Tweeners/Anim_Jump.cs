using BebiAnimations.Libs.Core;
using DG.Tweening;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Jump : Anim_MoveTo
    {
        [SerializeField] protected float _jumpPower;
        [SerializeField] protected int _numJumps;
        /*
        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKill();
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

                Tween tween = DoJump().OnComplete(() =>
                {
                    Done();
                });

                if (_tweenSetting != null)
                {
                    _tweenSetting.ApplySetting(tween);
                }
                else
                {
                    tween.SetEase(_easeType);
                }

                tween.SetId(this);
            }
            else
            {
                this.DOKill();
                SetPoint(_objectTR, GetPoint(_toPointTR, _space), _space);
            }
        }

        protected override void ActionRevert()
        {
            this.DOKill();

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
            this.DOKill();
        }
        */

        protected override Tween DoMove()
        {
            if (_space == Space.Self)
            {
                return _objectTR.DOLocalJump(_toPointTR.localPosition, _jumpPower, _numJumps, _animDuration);
            }
            else
            {
                return _objectTR.DOJump(_toPointTR.position, _jumpPower, _numJumps, _animDuration);
            }
        }
    }
}
