using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace BebiAnimations.Libs.Core
{
    public abstract class AnimationAction : MonoBehaviour
    {
        public enum ActionState { IDLE, STOPPED, ACTION, DONE };

        [Range(0f, 10f)]
        [SerializeField] protected float _animLocalStartTime;
        [SerializeField] protected float _animDuration;

        public float AnimLocalStartTime => _animLocalStartTime;
        public float AnimDuration => _animDuration;
        public ActionState _actionState;
        protected bool _animOn;
        private float _animStartTime;
        private Action<AnimationAction> _actionStartEvent;
        private Action<AnimationAction> _actionEndEvent;

        public virtual void Initialize()
        {

        }

        public virtual void Reset()
        {
            _actionState = AnimationAction.ActionState.IDLE;
        }

        public void SetAnimStartTime(float startTime, Action<AnimationAction> actionStartEvent, Action<AnimationAction> actionEndEvent)
        {
            _animStartTime = startTime;
            _actionStartEvent = actionStartEvent;
            _actionEndEvent = actionEndEvent;
        }

        public void CheckAction(bool animOn, float timeLineValue)
        {
            if (CurrentActionState == AnimationAction.ActionState.DONE || CurrentActionState == AnimationAction.ActionState.ACTION)
            {
                return;
            }

            if (timeLineValue < _animStartTime)
            {
                return;
            }

            Play(animOn);
        }

        public void Play(bool animOn = true)
        {
            _animOn = animOn;
            _actionState = ActionState.ACTION;
            _actionStartEvent?.Invoke(this);

            ActionPlay();

            if (!AnimOn)
            {
                Done();
            }
        }

        public void Revert()
        {
            _actionStartEvent = null;
            _actionEndEvent = null;
            ActionRevert();
            Done();
        }

        public virtual void Stop()
        {
            _actionState = ActionState.STOPPED;
            ActionStop();
        }

        public void Done()
        {
            _actionState = ActionState.DONE;
            ActionStop();
            _actionEndEvent?.Invoke(this);
        }

        protected virtual void ActionPlay()
        {
            if (AnimOn)
            {

            }
        }

        protected virtual void ActionRevert()
        {
            if (AnimOn)
            {

            }
        }

        protected virtual void ActionStop()
        {

        }

        public ActionState CurrentActionState
        {
            get
            {
                return _actionState;
            }
        }

        public float AnimStartTime
        {
            get
            {
                return _animStartTime;
            }
        }

        public bool AnimOn
        {
            get
            {
                if (0.0f == _animDuration)
                {
                    return false;
                }
                else
                {
                    return _animOn;
                }
            }
        }



        public static void SetPoint(Transform tr, Vector3 point, Space space)
        {
            if (space == Space.Self)
            {
                tr.localPosition = point;
            }
            else
            {
                tr.position = point;
            }
        }

        public static Vector3 GetPoint(Transform tr, Space space)
        {
            if (space == Space.Self)
            {
                return tr.localPosition;
            }
            else
            {
                return tr.position;
            }
        }
    }
}


