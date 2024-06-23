using BebiLibs;
using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiInteractions.Libs;

namespace Showcase
{
    public class GameInteractions_Showcase : MonoBehaviour
    {
        [SerializeField] private List<GameStageBase_Showcase> _stageList;
        protected GameStageBase_Showcase _currentStage;

        private void Start()
        {
            Activate();
        }
        /*
        private float _count = 0;
        private void Update()
        {
            //return;
            if (0 <= _count)
            {
                _count += Time.deltaTime;
            }

            //Debug.Log("-- " + _count);
            if (2.0f < _count)
            {
                _count = -10.0f;
                Activate();
            }
        }
        */
        public void Activate()
        {
            for (int i = 0; i < _stageList.Count; i++)
            {
                _stageList[i].Init();
                _stageList[i].Reset();
                _stageList[i].OnCompleteEvent += () =>
                {
                    CompleteStage();
                };
            }

            StartRound();
        }

        protected void StartRound()
        {
            _currentStage = GetStageByName("Default");
            string[] array = {"a", "b", "c" };
            _currentStage.SetData(array, 3);
            _currentStage.Show(null);
        }

        protected void CompleteStage()
        {
            /*
            if (!_gameData.IsLast)
            {
                _currentStage.Hide(() =>
                {
                    StartRound();
                });
                return;
            }
            else
            {
                //base.onComplete?.Invoke();
            }
            */
        }

        protected GameStageBase_Showcase GetStageByName(string stageName)
        {
            for (int i = 0; i < _stageList.Count; i++)
            {
                if (_stageList[i].StageName == stageName)
                {
                    return _stageList[i];
                }
            }
            return _stageList[0];
        }
    } 
}
