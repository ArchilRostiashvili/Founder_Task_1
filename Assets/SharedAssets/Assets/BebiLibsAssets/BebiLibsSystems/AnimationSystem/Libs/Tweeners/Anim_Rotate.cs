using BebiAnimations.Libs;
using BebiAnimations.Libs.Core;
using DG.Tweening;
using UnityEngine;

namespace BebiAnimations.Libs.Anims
{
    public class Anim_Rotate : AnimationTween
    {
        [SerializeField] private Transform _objectTR;

        [SerializeField] private bool _useRelativeRotation;
        [SerializeField] private bool _rotateEndlessly;
        [SerializeField] private Vector3 _toRotateValue = new Vector3(0,0,0);
        [SerializeField] private Vector3 _initialRotation = new Vector3(0,0,0);
        
        public override void Initialize()
        {
            if (_useInitValues)
            {
                _initialRotation = _objectTR.localRotation.eulerAngles;
            }
        }

        public void SetEndValue(Vector3 rotationValue)
        {
            _toRotateValue = rotationValue;
        }

        protected override void ActionPlay()
        {
            if (AnimOn)
            {
                if (_rotateEndlessly)
                {
                    _objectTR.DORotate(_toRotateValue, _animDuration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }
                else
                {
                    this.DOKillAnim();
                    if (_resetOnPlay)
                    {
                        _objectTR.localRotation = new Quaternion(_initialRotation.x, _initialRotation.y, _initialRotation.z, 0);
                    }

                    _tween = DoRotate().SetRelative(_useRelativeRotation).OnComplete(() =>
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
            }
            else
            {
                this.DOKillAnim();
                if (_useRelativeRotation)
                {
                    Vector3 rotTo = _initialRotation + _toRotateValue;
                    _objectTR.localRotation = new Quaternion(rotTo.x,rotTo.y, rotTo.z,0);
                }
                else
                {
                    _objectTR.localRotation = new Quaternion(_toRotateValue.x,_toRotateValue.y,_toRotateValue.z,0);
                }
            }
        }

        protected override void ActionRevert()
        {
            this.DOKillAnim();
            _objectTR.localRotation = new Quaternion(_initialRotation.x, _initialRotation.y, _initialRotation.z, 0);
        }

        protected override void ActionStop()
        {
            this.DOKillAnim();
        }

        private Tween DoRotate()
        {
            return _objectTR.DOLocalRotate(_toRotateValue, _animDuration, RotateMode.FastBeyond360);
        }
    }
}
