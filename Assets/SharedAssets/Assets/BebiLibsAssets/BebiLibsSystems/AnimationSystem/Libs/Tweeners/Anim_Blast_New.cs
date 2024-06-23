using BebiAnimations.Libs.Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Blast_New : AnimationTween
    {
        [SerializeField] private Transform _objectTR;

        [SerializeField] private Space _space = Space.World;
        [SerializeField] private Transform _blastCenterPointTR;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _jumpRadius;

        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _initialPoint;

        public override void Initialize()
        {
            if (_useInitValues)
            {
                // _initialPoint = GetPoint(_objectTR, _space);

                Vector3 blastCenterPosition = GetPoint(_blastCenterPointTR, _space);
                Vector3 objectPosition = GetPoint(_objectTR, _space);
                Vector3 diffVector = blastCenterPosition - objectPosition;
                diffVector = diffVector.normalized;

                _endPoint.position -= diffVector * _jumpRadius;
            }
        }

        public void SetEndPoint(Vector3 endPoint)
        {
            _endPoint.position = endPoint;
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                this.DOKill();

                if (_resetOnPlay)
                {
                    SetPoint(_objectTR, _initialPoint.position, _space);
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
                SetPoint(_objectTR, _endPoint.position, _space);
            }
        }

        private Tween DoJump()
        {
            if (_space == Space.Self)
            {
                return _objectTR.DOLocalJump(_endPoint.position, _jumpPower, 1, _animDuration);
            }
            else
            {
                return _objectTR.DOJump(_endPoint.position, _jumpPower, 1, _animDuration);
            }
        }

        protected override void ActionRevert()
        {
            this.DOKill();
            SetPoint(_objectTR, _initialPoint.position, _space);
        }

        protected override void ActionStop()
        {
            this.DOKill();
        }
    }
}
