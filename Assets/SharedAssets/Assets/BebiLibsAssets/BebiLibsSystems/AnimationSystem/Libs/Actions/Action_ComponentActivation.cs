using BebiAnimations.Libs.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Actions
{
    public class Action_ComponentActivation : AnimationAction
    {
        [SerializeField] private List<Behaviour> _componentsList = new List<Behaviour>();
        [SerializeField] private bool _isActive;

        protected override void ActionPlay()
        {
            foreach (var item in _componentsList)
            {
                item.enabled = _isActive;
            }
        }

        protected override void ActionRevert()
        {
            foreach (var item in _componentsList)
            {
                item.enabled = !_isActive;
            }
        }
    }
}
