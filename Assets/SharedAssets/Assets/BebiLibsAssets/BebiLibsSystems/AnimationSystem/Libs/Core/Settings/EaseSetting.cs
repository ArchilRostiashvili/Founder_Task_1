using DG.Tweening;
using UnityEngine;

namespace BebiAnimations.Libs.Core.Settings
{
    [System.Serializable]
    public sealed class EaseSetting : TweenModifierBase
    {
        [SerializeField] private EaseType _easeType = EaseType.Default;
        [SerializeField] private Ease _tweenEasing = Ease.Linear;
        [SerializeField] private AnimationCurve _curveEasing = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float _overshoot = 1.70158f;
        [SerializeField] private float _amplitude = 1.70158f;
        [Range(-1, 1)]
        [SerializeField] internal float _period = 0;

        public override Tween ApplyModification(Tween tweenToModify)
        {
            return _easeType switch
            {
                EaseType.Default => tweenToModify.SetEase(_tweenEasing),
                EaseType.AnimationCurve => tweenToModify.SetEase(_curveEasing),
                EaseType.WithOvershoot => tweenToModify.SetEase(_tweenEasing, _overshoot),
                EaseType.WithAmplitude => tweenToModify.SetEase(_tweenEasing, _amplitude, _period),
                _ => tweenToModify.SetEase(_tweenEasing),
            };
        }
    }
}