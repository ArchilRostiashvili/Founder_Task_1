using System;
using BebiAnimations.Libs.Core;
using BebiLibs;
using FarmLife.MiniGames;
using UnityEngine;

namespace FarmLife
{
    public class PlayButton : MonoBehaviour
    {
        public Action FinishedHintEvent;

        [SerializeField] private FarmMinigameSceneData _gameData;
        [SerializeField] private bool _hasMiniGameData;
        [SerializeField][HideField("_hasMiniGameData", true)] private MiniGameBaseData _miniGameData;
        [SerializeField] private Transform _zoomPoint;

        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private ButtonScale _buttonScale;

        private bool _isVisibleInRender;
        private bool _isPlayingAnimation;

        private Action<FarmMinigameSceneData, Vector3> buttonPressed;

        public ButtonScale ButtonScale => _buttonScale;
        public bool IsVisible => _isVisibleInRender;

        public void Activate()
        {
            if (_feelAnimator == null)
            {
                Debug.LogWarning("feel animator is null");
                return;
            }
            _feelAnimator.InitializeFeedbackPlayers();
            _feelAnimator.Play(AnimationNamesData.ANIM_SWING);
        }

        public void Stop()
        {
            if (_feelAnimator == null)
            {
                Debug.LogWarning("feel animator is null");
                return;
            }

            _feelAnimator.Stop(AnimationNamesData.ANIM_SWING);
        }

        public void SetEvent(Action<FarmMinigameSceneData, Vector3> action)
        {
            buttonPressed = action;
        }

        public void PressButton()
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_PRESS);

            if (_hasMiniGameData & _miniGameData != null)
                _gameData.MiniGameBaseData = _miniGameData;

            buttonPressed?.Invoke(_gameData, _zoomPoint.position);
        }

        public void PlayButtonTutorial()
        {
            _isPlayingAnimation = true;
            _feelAnimator.Play("Hint", () =>
            {
                _isPlayingAnimation = false;
                FinishedHintEvent?.Invoke();
            });
        }

        private void OnBecameInvisible()
        {
            _isVisibleInRender = false;

            if (_isPlayingAnimation)
            {
                _isPlayingAnimation = false;
                FinishedHintEvent?.Invoke();

                _feelAnimator.Stop("Hint");
                _feelAnimator.Play("StopHint");
            }
        }

        private void OnBecameVisible()
        {
            _isVisibleInRender = true;
        }
    }
}