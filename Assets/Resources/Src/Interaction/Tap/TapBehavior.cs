using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FarmLife.Controllers.Tap
{
    public class TapBehavior : MonoBehaviour, IPointerClickHandler
    {
        public Action<TapBehavior> ClickEvent;
        public Action WrongEvent;
        public Action CorrectEvent;

        [SerializeField] private Collider2D _objectCollider;
        [SerializeField] private string _objectID;
        [SerializeField] private bool _hasFeelAnimator;
        [SerializeField][HideField("_hasFeelAnimator", true)] FeelAnimator _feelAnimator;

        private bool _canInteract;

        public void SetID(string id)
          => _objectID = id;

        public string ID => _objectID;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canInteract)
                return;
            ClickEvent?.Invoke(this);
        }

        public void Correct(System.Action afterCorrectAction)
        {
            if (!_hasFeelAnimator)
                return;

            _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT, afterCorrectAction);
        }

        public void Wrong(System.Action afterWrongAction)
        {
            if (!_hasFeelAnimator)
                return;

            _feelAnimator.Play(AnimationNamesData.ANIM_WRONG, afterWrongAction);
        }

        public void EnableCollider(bool value)
        {
            _canInteract = value;
            _objectCollider.enabled = value;
        }
    }
}