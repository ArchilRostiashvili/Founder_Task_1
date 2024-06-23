using System.Collections;
using System.Collections.Generic;
using FarmLife.MiniGames.Base;
using UnityEngine;

namespace FarmLife.MiniGames
{
    public class FarmMiniGame : MonoBehaviour
    {
        [SerializeField] private FinishScreen _finishScreen;
        [SerializeField] private BalloonSystem _balloonSystem;

        [SerializeField] private List<MiniGameStageBase> _stagesList = new List<MiniGameStageBase>();

        private MiniGameStageBase _currentStage;
        private int _roundNumber = 0;

        private void Start()
        {
            Init();
            StartNewRound();
        }

        private void Init()
        {
            _roundNumber = 0;
            for (int i = 0; i < _stagesList.Count; i++)
            {
                _stagesList[i].Init();

                if (i == 0)
                    continue;

                _stagesList[i].Deactivate();
            }

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _stagesList.ForEach((x) => x.RoundFinishEvent += OnRoundFinish);
        }

        private void OnRoundFinish()
        {
            _roundNumber++;

            if (_roundNumber >= _stagesList.Count)
            {
                GameFinish();
                return;
            }

            StartNewRound();
        }

        private void StartNewRound()
        {
            if (_roundNumber >= _stagesList.Count)
            {
                return;
            }

            if (_currentStage != null)
                _currentStage.Deactivate();

            _currentStage = _stagesList[_roundNumber];

            _currentStage.Activate();
            _currentStage.StartRound();
        }

        private void GameFinish()
        {
            //if (_balloonSystem != null) _balloonSystem.Activate();
            Invoke(nameof(DisableBalloons), 4);
            _finishScreen.gameObject.SetActive(true);
            _finishScreen.Init();
        }

        private void DisableBalloons()
        {
            if (_balloonSystem != null) _balloonSystem.DisableSpawn();
        }
    }
}