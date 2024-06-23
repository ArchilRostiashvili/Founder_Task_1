using BebiLibs;
using BebiInteractions.Libs;
using TMPro;
using UnityEngine;
using BebiAnimations.Libs.Core;
using System;
using BebiAnimations.Libs.Anims;

namespace Showcase
{
    public class VisualObject_Showcase : MonoBehaviour
    {
        [SerializeField] private InteractableItemBase _interactableItem;
        [SerializeField] protected BebiAnimator _bebiAnimator;

        [SerializeField] protected Transform _containerTR;

        protected Vector3 _toPoint;
        protected bool _usePoint;

        public virtual void SetData(string itemID)
        {
            gameObject.SetActive(true);
            if (_interactableItem != null)
            {
                _interactableItem.ItemID = itemID;
            }
        }

        public virtual void Show(Action callback = null, bool anim = true)
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_SHOW, anim, callback, (AnimationAction animationAction) =>
            {
                if (_usePoint)
                {
                    if (animationAction is Anim_MoveTo)
                    {
                        ((Anim_MoveTo)animationAction).SetToPoint(_toPoint);
                        _usePoint = false;
                    }
                }
            });
        }

        public virtual void DragBegin()
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_DRAGBEGIN);
        }

        public virtual void Highlight()
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_HIGHTLIGHT);
        }

        public virtual void Highlight_Off()
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_HIGHTLIGHT_OFF);
        }

        public virtual void Correct(Action callback = null, bool anim = true)
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_CORRECT, anim, callback, (AnimationAction animationAction) =>
            {
                if (_usePoint)
                {
                    if (animationAction is Anim_MoveTo)
                    {
                        ((Anim_MoveTo)animationAction).SetToPoint(_toPoint);
                        _usePoint = false;
                    }
                }
            });
        }

        public virtual void Wrong(Action callback = null, bool anim = true)
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_WRONG, anim, callback);
        }

        public virtual void Hide(Action callback = null, bool anim = true)
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_HIDE, anim, callback);
        }

        public virtual void Reset()
        {
            _bebiAnimator.Play(GameStageBase_Showcase.ANIM_SHOW);
        }

        public void SetExtraData(Vector3 toPoint, bool usePoint)
        {
            _toPoint = toPoint;
            _usePoint = usePoint;
        }

        public void PlayAudio(bool play = true)
        {
            if (play)
            {
               //_contentInstanceData.MainTrack.Play();
            }
            else
            {
                //_contentInstanceData.MainTrack.Stop();
            }
        }

        public void PlayAdditionalAudio(bool play = true)
        {
            if (play)
            {
                //_contentInstanceData.AdditionalTrack.Play();
            }
            else
            {
                //_contentInstanceData.AdditionalTrack.Stop();
            }
        }
    }
}
