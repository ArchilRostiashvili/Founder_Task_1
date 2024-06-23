using System;
using System.Collections;
using System.Collections.Generic;
using FarmLife.Data.MiniGame;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FarmLife.MiniGames.Base
{
    public class MiniGameStageBase : MonoBehaviour
    {
        public Action RoundFinishEvent;

        [SerializeField] protected Transform _contentTR;
        [SerializeField] protected bool _hasAnimationChainer;
        [HideField("_hasAnimationChainer", true)][SerializeField] protected FeelAnimator _feelAnimator;
        [SerializeField] protected bool _hasHelper;
        [HideField("_hasHelper", true)][SerializeField] protected FarmHelper _helper;
        [SerializeField] protected int _incorrectAnswerAmount = 3;

        [SerializeField] protected bool _hasData;
        [HideField("_hasData", true)][SerializeField] protected MiniGameBaseData _miniGameData;

        protected int _incorrectlyAnswered;

        public virtual void Init() { }
        public virtual void StartRound() { }
        public virtual void CompleteRound() { }

        protected void InitHelperHand()
        {
            if (!_hasHelper || _helper == null)
                return;

            _helper.Init();
            _helper.SetIsActive(false);
            _helper.HelpNeededEvent = null;
            _helper.HelpNeededEvent += OnHelpNeeded;
        }

        public virtual void Activate()
        {
            if (_contentTR == null)
                return;

            _contentTR.gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            if (_contentTR == null)
                return;

            _contentTR.gameObject.SetActive(false);
        }

        protected void ShowAnimation(System.Action afterShowEvent = null)
        {
            if (!_hasAnimationChainer || _feelAnimator == null)
            {
                afterShowEvent?.Invoke();
                return;
            }
            _feelAnimator.Play(AnimationNamesData.ANIM_SHOW, afterShowEvent);
        }

        protected void HideAnimation(System.Action afterHideEvent = null)
        {
            if (!_hasAnimationChainer || _feelAnimator == null)
            {
                afterHideEvent?.Invoke();
                return;
            }

            _feelAnimator.Play(AnimationNamesData.ANIM_HIDE, afterHideEvent);
        }

        protected void PlayHintAnimation(System.Action afterHintEvent = null)
        {
            if (!_hasAnimationChainer || _feelAnimator == null)
            {
                afterHintEvent?.Invoke();
                return;
            }

            _feelAnimator.Play(AnimationNamesData.ANIM_HELP, afterHintEvent);
        }

        protected virtual void OnHelpNeeded() { }

        protected void EnableHelperTracking()
        {
            if (_helper == null || !_hasHelper)
                return;

            _helper.Reset();
            _helper.SetIsActive(true);
        }

        protected void DisableHelperTracking()
        {
            _helper.SetIsActive(false);
            _helper.HideHelper();
        }

        protected void IncrementIncorrectAnswer()
        {
            _incorrectlyAnswered++;

            if (_incorrectlyAnswered >= _incorrectAnswerAmount)
            {
                _incorrectlyAnswered = 0;
                _helper.HelpNeededEvent?.Invoke();
            }
        }
    }
}