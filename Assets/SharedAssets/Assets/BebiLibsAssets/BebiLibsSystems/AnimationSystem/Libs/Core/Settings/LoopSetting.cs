using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace BebiAnimations.Libs.Core.Settings
{
    [System.Serializable]
    public class LoopSetting : TweenModifierBase
    {
        [SerializeField] private bool _enableLooping = false;
        [SerializeField] private int _loopCount = 0;
        [SerializeField] private LoopType _loopType = LoopType.Restart;

        public override Tween ApplyModification(Tween tweenToModify)
        {
            if (_enableLooping)
            {
                tweenToModify.SetLoops(_loopCount, _loopType);
            }
            return tweenToModify;
        }
    }
}
