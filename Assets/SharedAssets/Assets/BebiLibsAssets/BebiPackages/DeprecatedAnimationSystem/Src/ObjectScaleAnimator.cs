using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BebiLibs/Animations/New scale animator")]
public class ObjectScaleAnimator : ObjectAnimatorBase
{
    [field: SerializeField] public Vector3 AnimationEndScaleValue;
    [field: SerializeField] public AnimationCurve ScaleAnimationCurve;

    public override void Play(Transform owner, System.Action onComplete = null)
    {
        owner.DOScale(AnimationEndScaleValue, AnimationTime).SetEase(ScaleAnimationCurve).
            OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public override void Stop(Transform owner)
    {
        owner.DOKill();
    }
}
