using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using UnityEngine;

namespace FarmLife
{
    public class QuestionBubble : MonoBehaviour
    {
        [SerializeField] private FeelAnimator _feelAnimator;

        [SerializeField] private ItemSpriteSizer _itemSpriteSizer;

        public void SetData(Sprite sprite)
        {
            _itemSpriteSizer.SetSprite(sprite);
        }

        public void Show(System.Action afterShowEvent = null)
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_SHOW, afterShowEvent);
        }

        public void Hide(System.Action afterHideEvent = null)
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_HIDE, afterHideEvent);
        }
    }
}