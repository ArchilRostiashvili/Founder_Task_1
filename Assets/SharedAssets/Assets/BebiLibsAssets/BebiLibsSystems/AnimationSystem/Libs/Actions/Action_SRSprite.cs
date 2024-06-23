using BebiAnimations.Libs.Core;
using UnityEngine;

namespace BebiAnimations.Libs.Actions
{
    public class Action_SRSprite : AnimationAction
    {
        [SerializeField] private SpriteRenderer _mainSR;
        [SerializeField] private Sprite _targetSprite;
        private Sprite _initialSprite;

        public override void Initialize()
        {
            _initialSprite = _mainSR.sprite;
        }

        protected override void ActionPlay()
        {
            _mainSR.sprite = _targetSprite;
        }

        protected override void ActionRevert()
        {
            _mainSR.sprite = _initialSprite;
        }

        public void SetSprite(Sprite sprite)
        {
            _targetSprite = sprite;
        }
    }
}
