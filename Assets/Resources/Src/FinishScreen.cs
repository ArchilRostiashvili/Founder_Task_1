using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FarmLife
{
    public class FinishScreen : MonoBehaviour
    {
        [SerializeField] private FeelAnimator _feelAnimator;

        [SerializeField] private FarmMinigameSceneData _currentMiniGameData;
        [SerializeField] private List<FarmMinigameSceneData> _farmMinigameDatasList = new List<FarmMinigameSceneData>();

        [SerializeField] private bool _scaleButtons = true;
        [SerializeField] private float _idleTime = 10f;

        private bool _isButtonClicked, _isActive;

        private float _timePassed;

        public void Reset()
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_RESET);
        }

        public void Init()
        {
            List<FarmMinigameSceneData> _usedFarmMinigameDatasList = new List<FarmMinigameSceneData>();

            if (_farmMinigameDatasList.Contains(_currentMiniGameData))
                _farmMinigameDatasList.Remove(_currentMiniGameData);

            _feelAnimator.Play(AnimationNamesData.ANIM_SHOW, () =>
            {
                ButtonScaleInvoker();
                _isActive = true;
            });
        }

        private void OnButtonClick(FarmMinigameSceneData data)
        {
            if (_isButtonClicked)
                return;

            _isButtonClicked = true;

            FarmSceneLoader.LoadScene(data.GameSceneReference.SceneName, _currentMiniGameData.GameSceneReference.SceneName);
        }

        public void ButtonScaleInvoker()
        {
            if (!_scaleButtons)
                return;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            _timePassed += Time.deltaTime;
            if (_timePassed >= _idleTime)
            {
                _isActive = false;
                _timePassed = 0;
            }
        }
    }
}