using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Core
{
    [System.Serializable]
    public class BebiAnimation : MonoBehaviour
    {
        [Header("Animation Time Parameters:")]
        
        [SerializeField] private float _animationDelay;
        //[SerializeField] private Initialization _autoInitialization = Initialization.ON_START;
        [SerializeField] private List<AnimationAction> _actionsList = new List<AnimationAction>();
        public string ActionID => gameObject.name;


        public List<AnimationAction> ActionList => _actionsList;

        private float _totalAnimationTime;
        private bool _actionsOn;
        private bool _playAnim;
        private Action _animCompleteEvent;
        private float _timeLine;

        public void Play(bool playAnim = true, System.Action animCompleteEvent = null, System.Action<AnimationAction> actionStartEvent = null, System.Action<AnimationAction> actionEndEvent = null)
        {
            _playAnim = playAnim;
            _animCompleteEvent = animCompleteEvent;
            _actionsOn = false;

            _totalAnimationTime = 0;
            _timeLine = 0.0f;
            int animCount = 0;

            foreach (var item in _actionsList)
            {
                item.Reset();
                float delayTime = _animationDelay + item.AnimLocalStartTime;
                if (_totalAnimationTime < (delayTime + item.AnimDuration))
                {
                    _totalAnimationTime = (delayTime + item.AnimDuration);
                }

                if (0 < delayTime)
                {
                    animCount++;
                    item.SetAnimStartTime(delayTime, actionStartEvent, actionEndEvent);
                }
                else
                {
                    item.SetAnimStartTime(delayTime, actionStartEvent, actionEndEvent);
                    item.Play(_playAnim);
                }
            }
            
            if (0 < animCount || _playAnim)
            {
                
                _actionsOn = true;
            }
            else
            {
                _animCompleteEvent?.Invoke();
            }
        }

        public void Revert()
        {
            Stop();

            foreach (var item in _actionsList)
            {
                item.Revert();
            }
        }

        public void EnterFrame()
        {
            if (!_actionsOn)
            {
                return;
            }

            _timeLine += Time.deltaTime;
            int count = 0;
            for (int i = 0; i < _actionsList.Count; i++)
            {
                _actionsList[i].CheckAction(_playAnim, _timeLine);
                if (_actionsList[i].CurrentActionState == AnimationAction.ActionState.DONE)
                {
                    count++;
                }
            }

            if(count == _actionsList.Count)
            {
                _actionsOn = false;
                _animCompleteEvent?.Invoke();
            }
        }

        public void Stop()
        {
            _actionsOn = false;
            foreach (var item in _actionsList)
            {
                item.Stop();
            }
        }

        private void Reset()
        {
            _actionsOn = false;
            foreach (var item in _actionsList)
            {
                item.Reset();
            }
        }
        /*
        private void Awake()
        {
            if (_autoInitialization == Initialization.ON_AWAKE)
                Initialize();
        }


        private void Start()
        {
            if (_autoInitialization == Initialization.ON_START)
                Initialize();
        }


        private void OnEnable()
        {
            if (_autoInitialization == Initialization.ON_ENABLE)
                Initialize();
        }
        */
        public void Initialize()
        {
            foreach (var item in _actionsList)
            {
                item.Initialize();
            }
        }


        public enum Initialization
        {
            NONE = 0,
            ON_AWAKE = 1,
            ON_START = 2,
            ON_ENABLE = 3
        }

        public float TotalAnimationTime
        {
            get
            {
                return _totalAnimationTime;
            }
        }
    }
}
