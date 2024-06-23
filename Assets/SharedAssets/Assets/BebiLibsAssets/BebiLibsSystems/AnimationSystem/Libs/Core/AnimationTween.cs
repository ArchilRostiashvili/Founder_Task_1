using BebiAnimations.Libs.Core.Settings;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Core
{
    public abstract class AnimationTween: AnimationAction
    {
        [SerializeField] protected Ease _easeType = Ease.Linear;
        [SerializeField] protected TweenSetting _tweenSetting;
        [SerializeField] protected bool _resetOnPlay = true;
        [SerializeField] protected bool _useInitValues = true;
        protected Tween _tween;

        protected void DOKillAnim()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }
    }
}


