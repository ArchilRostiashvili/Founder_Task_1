using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.Controllers.Tap
{
    public class TapController : MonoBehaviour
    {
        public Action<TapBehavior> CorrectlyAnsweredEvent;
        public Action<TapBehavior> IncorrectlyAnsweredEvent;

        private List<TapBehavior> _tapBehaviorList = new List<TapBehavior>();
        private string _correctID;
        private bool _shouldEnableCollider = true;
        private bool _isActive = true;

        public string CorrectID => _correctID;

        public void SetID(string correctID)
           => _correctID = correctID;

        public void Init()
        {
            _shouldEnableCollider = true;
        }

        public void SetIsActive(bool isActive)
            => _isActive = isActive;

        public void EnableTapBehaviors(bool enable)
            => _tapBehaviorList.ForEach((x) => x.EnableCollider(enable));

        public void AddTapBehavior(TapBehavior tapBehavior)
        {
            if (_tapBehaviorList.Contains(tapBehavior))
                return;

            _tapBehaviorList.Add(tapBehavior);
            SubscribeToEvents(tapBehavior);
        }

        private void SubscribeToEvents(TapBehavior tapBehavior)
        {
            tapBehavior.ClickEvent += OnTapObjectClicked;
        }

        private void OnTapObjectClicked(TapBehavior behavior)
        {
            if (!_isActive)
                return;

            if (behavior.ID == _correctID)
            {
                _shouldEnableCollider = false;

                behavior.CorrectEvent?.Invoke();
                behavior.EnableCollider(false);
                behavior.Correct(() =>
                    {
                        CorrectlyAnsweredEvent?.Invoke(behavior);
                        return;
                    });
            }
            else
            {
                behavior.WrongEvent?.Invoke();
                behavior.EnableCollider(false);
                behavior.Wrong(() =>
                    {
                        IncorrectlyAnsweredEvent?.Invoke(behavior);
                        if (_shouldEnableCollider)
                            behavior.EnableCollider(true);
                        return;
                    });
            }
        }
    }
}