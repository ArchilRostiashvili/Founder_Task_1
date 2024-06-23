using BebiAnimations.Libs.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Swing : AnimationTween
    {
        [SerializeField] private Transform _objectTR;
        [SerializeField] private Vector3 _swingPower;
        [SerializeField] private Space _space = Space.World;

        /*
        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private float _animToTime = 0.1f;
        [SerializeField] private float _animFromTime = 0.2f;
        [SerializeField] private float _waitToTime;
        [SerializeField] private float _waitFromTime;
        */
        [SerializeField] private Vector3 _initialPoint;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialPoint = GetPoint(_objectTR, _space);
            }
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKillAnim();
                if (_resetOnPlay)
                {
                    SetPoint(_objectTR, _initialPoint, _space);
                }

                _tween = DoSwing();
            }
            else
            {
                this.DOKillAnim();
                SetPoint(_objectTR, _initialPoint, _space);
            }
        }

        public Tween DoSwing()
        {
            if (_space == Space.World)
            {
                return _objectTR.DOMove(_initialPoint + _swingPower, _animDuration).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo).SetId(this);
            }
            else
            {
                return _objectTR.DOLocalMove(_initialPoint + _swingPower, _animDuration).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo).SetId(this);
            }

            /*
            Sequence s = DOTween.Sequence();
            s.Append(_objectTR.DOMove(_toPointTR.position, _animToTime).SetEase(_easeTo));
            s.AppendInterval(_waitToTime);
            s.Append(_objectTR.DOMove(_fromPointTR.position, _animFromTime).SetEase(_easeFrom));
            s.AppendInterval(_waitFromTime);
            s.SetLoops(-1, LoopType.Restart);
            s.SetId(_objectTR);
            */
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            SetPoint(_objectTR, _initialPoint, _space);
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
            _actionState = ActionState.DONE;
        }
    }
}
