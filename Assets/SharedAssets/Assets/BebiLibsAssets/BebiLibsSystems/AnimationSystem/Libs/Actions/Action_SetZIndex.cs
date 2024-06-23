using BebiAnimations.Libs.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace BebiAnimations.Libs.Actions
{
    [System.Serializable]
    public class Action_SetZIndex : AnimationAction
    {
        [SerializeField] private SortingGroup _mainSG;
        [SerializeField] private int _newZIndex;
        private int _initialZIndex;

        public override void Initialize()
        {
            _initialZIndex = _mainSG.sortingOrder;
        }

        protected override void ActionPlay()
        {
            _mainSG.sortingOrder = _newZIndex;
        }

        protected override void ActionRevert()
        {
            _mainSG.sortingOrder = _initialZIndex;
        }
    }
}
