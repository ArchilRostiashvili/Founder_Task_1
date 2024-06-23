using BebiAnimations.Libs.Core;
using System.Collections.Generic;
using UnityEngine;


namespace BebiAnimations.Libs.Actions
{
    [System.Serializable]
    public class Action_GOActivation : AnimationAction
    {
        [SerializeField] private List<GameObject> _gameObjectsList = new List<GameObject>();
        [SerializeField] private bool _isActive;

        protected override void ActionPlay()
        {
            foreach (var go in _gameObjectsList)
            {
                go.SetActive(_isActive);
            }
        }

        protected override void ActionRevert()
        {
            foreach (var go in _gameObjectsList)
            {
                go.SetActive(!_isActive);
            }
        }
    }
}
