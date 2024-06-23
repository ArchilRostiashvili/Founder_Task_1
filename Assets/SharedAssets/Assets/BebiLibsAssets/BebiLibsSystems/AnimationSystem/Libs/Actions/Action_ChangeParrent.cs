using BebiAnimations.Libs.Core;
using System.Collections.Generic;
using UnityEngine;


namespace BebiAnimations.Libs.Actions
{
    [System.Serializable]
    public class Action_ChangeParrent : AnimationAction
    {
        [SerializeField] private Transform _mainTR;
        [SerializeField] private Transform _newParentTR;

        protected override void ActionPlay()
        {
            _mainTR.SetParent(_newParentTR);
        }
    }
}
