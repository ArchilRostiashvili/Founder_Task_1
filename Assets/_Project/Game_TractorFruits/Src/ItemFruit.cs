using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;

namespace FarmLife.MiniGames.TractorFruit
{
    public class ItemFruit : MonoBehaviour
    {
        [SerializeField] private ItemSpriteSizer _spriteSizer;
        [SerializeField] private FeelAnimator _feelAnimator;

        public void SetData(Sprite sprite)
            => _spriteSizer.SetSprite(sprite);

        public void Correct()
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT);
        }
    }
}