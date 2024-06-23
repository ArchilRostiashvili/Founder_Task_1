using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Core.Settings
{

    [CreateAssetMenu(fileName = "TweenSetting", menuName = "BebiLibs/Animations/TweenSetting", order = 0)]
    public class TweenSetting : ScriptableObject
    {
        [SerializeField] private EaseSetting _easeSetting;
        [SerializeField] private LoopSetting _loopSetting;

        public EaseSetting EaseSetting => _easeSetting;
        public LoopSetting LoopSetting => _loopSetting;

        public virtual Tween ApplySetting(Tween tween)
        {
            EaseSetting.ApplyModification(tween);
            LoopSetting.ApplyModification(tween);
            return tween;
        }
    }
}
