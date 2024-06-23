using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.Controllers.Drag
{
    public class DragTargetBehavior : MonoBehaviour
    {
        public Action<DragBehavior> CorrectTakeEvent;

        [SerializeField] private Collider2D _draggableCollider;
        [SerializeField] private string _draggableID;
        [SerializeField] private bool _hasFeelAnimator;

        [HideField("_hasFeelAnimator", true)][SerializeField] private bool _playWrongAnimation;
        [HideField("_hasFeelAnimator", true)][SerializeField] private bool _playCorrectOnTake;
        [HideField("_hasFeelAnimator", true)][SerializeField] private FeelAnimator _feelAnimator;

        private bool _isDone;
        private bool _isHighLighted;

        public Collider2D Collider => _draggableCollider;
        public string ID => _draggableID;
        public bool IsDone => _isDone;

        public FeelAnimator FeelAnimator => _feelAnimator;

        public void SetID(string id)
            => _draggableID = id;

        public void Take(DragBehavior dragBehavior, Action<DragBehavior, DragTargetBehavior> afterTakeEvent = null)
        {
            _isDone = true;
            dragBehavior.Correct(() =>
            {
                CorrectTakeEvent?.Invoke(dragBehavior);
                afterTakeEvent?.Invoke(dragBehavior, this);
            });

            if (_playCorrectOnTake)
                PlayCorrect();
        }

        public void Highlight(Action afterHighLight = null)
        {
            if (_feelAnimator == null)
                return;

            if (_isHighLighted)
                return;

            _isHighLighted = true;

            _feelAnimator.Play(AnimationNamesData.ANIM_HIGHLIGHT, afterHighLight);
        }

        public void HighlightOff(Action afterHighLight = null)
        {
            if (_feelAnimator == null)
                return;

            if (!_isHighLighted)
                return;

            _isHighLighted = false;

            _feelAnimator.Stop(AnimationNamesData.ANIM_HIGHLIGHT);
            _feelAnimator.Play(AnimationNamesData.ANIM_HIGHLIGHT_OFF, () =>
            {
                afterHighLight?.Invoke();
            });
        }

        public void PlayCorrect()
        {
            if (_feelAnimator == null)
                return;
            _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT, () =>
            {
            });
        }

        public void PlayWrong()
        {
            if (_feelAnimator == null || !_playWrongAnimation)
                return;
            _feelAnimator.Play(AnimationNamesData.ANIM_WRONG, () =>
            {
            });
        }
    }
}