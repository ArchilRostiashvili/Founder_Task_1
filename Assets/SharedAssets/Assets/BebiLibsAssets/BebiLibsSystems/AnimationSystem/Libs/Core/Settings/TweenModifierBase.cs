using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Core.Settings
{
    [System.Serializable]
    public abstract class TweenModifierBase
    {
        public abstract Tween ApplyModification(Tween tweenToModify);
    }
}
