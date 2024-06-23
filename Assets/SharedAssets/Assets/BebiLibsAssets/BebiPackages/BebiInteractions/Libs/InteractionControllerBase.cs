using BebiAnimations.Libs.Core;
using System;
using UnityEngine;

namespace BebiInteractions.Libs
{
    public enum InteractionMessage {DRAG_BEGIN, CORRECT, WRONG, FINISH, HIGHLIGHT, HIGHLIGHT_OFF, HELP};
    public class InteractionControllerBase : MonoBehaviour
    {
        protected bool _isEnabled;

        protected int _totalCorrectCount;
        protected int _correctCount;

        public Action<InteractionMessage, InteractableItemBase, InteractableItemBase> ActionEvent;

        public virtual void Reset()
        {
            Enable(true);
        }

        public virtual void Correct(InteractableItemBase interactable1, InteractableItemBase interactable2)
        {

        }

        public virtual void Wrong(InteractableItemBase interactable1, InteractableItemBase interactable2)
        {
      
        }

        public virtual void Enable(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }

        public void SetData(int totalCorrectCount)
        {
            _totalCorrectCount = totalCorrectCount;
            _correctCount = 0;
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
        }

        protected bool IsFinished()
        {
            return _totalCorrectCount <= _correctCount;
        }

        protected virtual void AddCorrect()
        {
            _correctCount++;
        }

        public virtual void GetHelp()
        {

        }
    }
}