using System;
using System.Collections.Generic;
using Bebi.Helpers;
using UnityEngine;

namespace FarmLife
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private CameraZoom _cameraZoom;
        [SerializeField] private PlayButton[] _playButtonsArray = new PlayButton[9];
        [SerializeField] private int[] _tutorialTimeArray;

        private float _passedTime;
        private int _activeTutorialTimeIndex;
        private bool _canTrackTime;

        public CameraZoom CameraZoom => _cameraZoom;

        private Action<string> onSceneLoadEvent;

        public void CanTrackTime(bool canTrackTime)
            => _canTrackTime = canTrackTime;

        public void Init()
        {
            foreach (PlayButton button in _playButtonsArray)
            {
                button.SetEvent(LoadLevel);
                button.Activate();
                button.FinishedHintEvent = OnFinishedHint;
            }

            _activeTutorialTimeIndex = 0;
        }

        public void SetEvent(Action<string> action)
        {
            onSceneLoadEvent = action;
        }

        public void EnableButtons(bool enable)
        {
            foreach (PlayButton button in _playButtonsArray)
            {
                button.ButtonScale.Enable(enable);
            }
        }

        internal void LoadRandomGame()
            => GetRandomVisibleButton().PressButton();

        private void LoadLevel(FarmMinigameSceneData data, Vector3 buttonPosition)
        {
            _cameraZoom.ZoomCamera(buttonPosition);

            EnableButtons(false);

            onSceneLoadEvent?.Invoke(data.SceneName);
        }

        private void Update()
        {
            if (!_canTrackTime)
                return;

            _passedTime += Time.deltaTime;

            if (_passedTime >= _tutorialTimeArray[_activeTutorialTimeIndex])
            {
                _canTrackTime = false;
                _passedTime = 0;

                if (_activeTutorialTimeIndex < _tutorialTimeArray.Length - 1)
                    _activeTutorialTimeIndex++;

                PlayTutorial();
            }
        }

        private void PlayTutorial()
            => GetRandomVisibleButton().PlayButtonTutorial();

        private PlayButton GetRandomVisibleButton()
        {
            List<PlayButton> playButtons = new List<PlayButton>();

            for (int i = 0; i < _playButtonsArray.Length; i++)
            {
                if (_playButtonsArray[i].IsVisible)
                    playButtons.Add(_playButtonsArray[i]);
            }

            if (playButtons.Count < 1)
                return null;

            return playButtons.GetRandomElement();
        }

        private void OnFinishedHint()
        {
            _canTrackTime = true;
            _passedTime = 0;
        }
    }
}