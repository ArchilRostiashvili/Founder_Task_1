using System;
using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using FarmLife.Controllers.Drag;
using FarmLife.MiniGames.Base;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    public class FruitDeliveringStage : MiniGameStageBase
    {
        [SerializeField] private List<ItemBox> _basketList = new List<ItemBox>();
        [SerializeField] private List<AnimalTarget> _animalTargetList = new List<AnimalTarget>();
        [SerializeField] private TractorFruitsData _fruitData;
        [SerializeField] private ItemTractor _itemTractor;
        [SerializeField] private DragController _dragController;

        [SerializeField] private Transform _movingObjectTR;
        [SerializeField] private Transform _mecanimStopPointTR;
        [SerializeField] private Transform _boxParentTR;
        [SerializeField] private RoadGenerator _roadGenerator;

        private int _currentAnimal;
        private bool _firstShowedHelper;

        public override void Init()
        {
            base.Init();
            _currentAnimal = 0;
            List<AnimalData> animalAnimatorList = new List<AnimalData>();

            _dragController.Init();

            for (int i = 0; i < _fruitData.AnimalList.Count; i++)
            {
                animalAnimatorList.Add(_fruitData.AnimalList[i]);
            }

            for (int i = 0; i < _animalTargetList.Count; i++)
            {
                FruitData fruitData = _fruitData.ActiveFruitList[i];
                AnimalData animalData = animalAnimatorList.GetRandomElementAndRemove();
                _animalTargetList[i].SetData(fruitData.FruitSprite, fruitData.FruitID, animalData.AnimalAnimator, animalData.ScaleUpValue);
            }

            SetDraggable();
        }

        private void SetDraggable()
        {
            for (int i = 0; i < _basketList.Count; i++)
            {
                _dragController.AddDraggable(_basketList[i].DragBehavior);
                _basketList[i].SetFinalParent(_boxParentTR);
            }

            for (int i = 0; i < _animalTargetList.Count; i++)
            {
                DragTargetBehavior dragTargetBehavior = _animalTargetList[i].DragTarget;
                _dragController.AddTarget(dragTargetBehavior);
                _animalTargetList[i].DragTarget.CorrectTakeEvent += (dragTarget) => OnCorrectTake(dragTarget, dragTargetBehavior);
            }

            _dragController.SetDraggableAmount(1);
            _dragController.IncorrectlyDraggedEvent += OnIncorrectlyDragged;
            _dragController.DragStartEvent += DisableHelperTracking;
            _dragController.DragEndEvent += EnableHelperTracking;
            _dragController.AllCorrectlyDraggedEvent += OnAllDragged;
        }

        public override void StartRound()
        {
            base.StartRound();
            _animalTargetList[_currentAnimal].transform.parent = _movingObjectTR;
            _animalTargetList[_currentAnimal].Show();
            _animalTargetList[_currentAnimal].SpawnMecanim();
            _roadGenerator.Activate();
            _roadGenerator.SetStopPoint(_mecanimStopPointTR, _animalTargetList[_currentAnimal].transform, () =>
               {
                   ShowAnimation(() =>
                   {
                       AfterShowAnimation();
                   });
               });
        }

        public override void Activate()
        {
            base.Activate();
            InitHelperHand();
            _itemTractor.Move();
        }

        private void OnIncorrectlyDragged()
        {
            EnableHelperTracking();
            IncrementIncorrectAnswer();
        }

        private void AfterShowAnimation()
        {
            EnableHelperTracking();

            if (!_firstShowedHelper)
            {
                OnHelpNeeded();
                _firstShowedHelper = true;
            }

            _dragController.EnableDraggable(true);
        }

        private void OnAllDragged()
        {
            _dragController.Reset();
            _currentAnimal++;
            DisableHelperTracking();
            if (_currentAnimal < _animalTargetList.Count)
            {
                //_feelAnimator.Play("RotateWheels");
                StartRound();
            }
            else
            {
                HideAnimation(() =>
                {
                    _roadGenerator.Activate();
                    RoundFinishEvent?.Invoke();
                });
            }
        }

        private void OnCorrectTake(DragBehavior behavior, DragTargetBehavior dragTargetBehavior)
        {
            behavior.transform.parent = dragTargetBehavior.transform;
        }

        protected override void OnHelpNeeded()
        {
            base.OnHelpNeeded();

            if (_currentAnimal >= _animalTargetList.Count)
                return;

            ItemBox itemBox = _basketList.Find((x) => x.DragBehavior.ID == _animalTargetList[_currentAnimal].DragTarget.ID);

            if (itemBox == null)
                return;

            _helper.ShowDragHelper(itemBox.transform.position, _animalTargetList[_currentAnimal].transform.position);
        }
    }
}