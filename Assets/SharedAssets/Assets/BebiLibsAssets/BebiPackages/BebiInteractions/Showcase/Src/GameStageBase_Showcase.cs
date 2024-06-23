using BebiAnimations.Libs.Core;
using BebiInteractions.Libs;
using System;
using UnityEngine;

namespace Showcase
{
    public class GameStageBase_Showcase : MonoBehaviour
    {
        public string StageName;
        public Action OnCompleteEvent;
        [SerializeField] protected InteractionControllerBase _interactionController;
        [SerializeField] protected BebiAnimator _bebiAnimator;

        public static readonly string ANIM_RESET = "Reset";
        public static readonly string ANIM_SHOW = "Show";
        public static readonly string ANIM_HIDE = "Hide";
        public static readonly string ANIM_CORRECT = "Correct";
        public static readonly string ANIM_WRONG = "Wrong";
        public static readonly string ANIM_DRAGBEGIN = "DragBegin";
        public static readonly string ANIM_FINISH = "Finish";
        public static readonly string ANIM_HIGHTLIGHT = "HighLight";
        public static readonly string ANIM_HIGHTLIGHT_OFF = "Highlight_Off";

        public virtual void Init()
        {

        }

        public virtual void SetData(string[] dataArray, int correctCount)
        {
            _interactionController.Reset();
            Reset();
        }

        public virtual void Show(Action callbackDone)
        {
            _bebiAnimator.Play(ANIM_SHOW, true, () =>
            {
                _interactionController.Reset();
                callbackDone?.Invoke();
            });
        }

        public virtual void Finish(Action callbackDone)
        {
            _bebiAnimator.Play(ANIM_FINISH, true, callbackDone);
        }

        public virtual void Hide(Action callbackDone)
        {
            _bebiAnimator.Play(ANIM_HIDE, true, callbackDone);
        }

        public virtual void Reset()
        {
            _bebiAnimator.Play(ANIM_RESET, false);
        }

        public void PlayAudio(bool play = true)
        {
            if (play)
            {
                //_gameData.MainData.MainTrack?.Play();
            }
            else
            {
                //_gameData.MainData.MainTrack?.Stop();
            }
        }

        public void PlayAdditionalAudio(bool play = true)
        {
            if (play)
            {
                //_gameData.MainData.AdditionalTrack?.Play();
            }
            else
            {
                //_gameData.MainData.AdditionalTrack?.Stop();
            }
        }
    }
}
