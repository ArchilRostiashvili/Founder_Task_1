using System.Collections;
using System.Collections.Generic;
using FarmLife.MiniGames.Base;
using FarmLife.Controllers.Drag;
using UnityEngine;
using System;

namespace FarmLife.MiniGames.SortingOfEggs
{
    public class EggsSortingStage : MiniGameStageBase
    {
        [SerializeField] private DragController _dragController;
        [SerializeField] private List<DraggableEggHolder> _draggableEggsHolderList = new List<DraggableEggHolder>();
        [SerializeField] private List<ItemNest> _nestList = new List<ItemNest>();
        [SerializeField] private EggsGenerator _eggsGenerator;
        [SerializeField] private SortingOfEggsData _eggsData;

        private int _helperShowedAmount;

        public override void Activate()
        {
            base.Activate();
            InitHelperHand();
        }

        public override void Init()
        {
            _eggsGenerator.GetRightScreenBorder();
            base.Init();
            InitDragController();
            _nestList.ForEach((x) => x.Init());
            SetDraggableAndTargets();
            _eggsData.CreateContent(_draggableEggsHolderList, _nestList);
        }

        private void InitDragController()
        {
            _dragController.CorrectlyDraggedEvent += _eggsGenerator.OnCorrectlyDragged;
            _dragController.Init();
            _dragController.AllCorrectlyDraggedEvent = OnCorrectlyDragged;
            _dragController.EnableDraggable(false);
            _draggableEggsHolderList.ForEach((x) => _eggsGenerator.AddDraggableEgg(x));
            _dragController.DragEndEvent += EnableHelperTracking;
            _dragController.DragStartEvent += DisableHelperTracking;
            _dragController.IncorrectlyDraggedEvent += IncrementIncorrectAnswer;
        }

        public override void StartRound()
        {
            ShowAnimation(() =>
            {
                EnableHelperTracking();
                _eggsGenerator.Activate();
                _dragController.EnableDraggable(true);
            });
        }

        public override void CompleteRound()
        {
            RoundFinishEvent?.Invoke();
        }

        private void SetDraggableAndTargets()
        {
            for (int i = 0; i < _draggableEggsHolderList.Count; i++)
                _dragController.AddDraggable(_draggableEggsHolderList[i].DraggableEgg.DragBehavior);

            for (int i = 0; i < _nestList.Count; i++)
                _dragController.AddTarget(_nestList[i].DragTargetBehavior);

            _dragController.SetDraggableAmount(_draggableEggsHolderList.Count);
        }

        private void OnCorrectlyDragged()
        {
            _dragController.EnableDraggable(false);
            DisableHelperTracking();
            HideAnimation(() =>
            {
                CompleteRound();
            });
        }

        protected override void OnHelpNeeded()
        {
            base.OnHelpNeeded();
            DraggableEgg eggHolder = _eggsGenerator.GetActiveEgg();

            if (eggHolder == null)
                return;

            ItemNest itemNest = _nestList.Find((x) => x.DragTargetBehavior.ID == eggHolder.DragBehavior.ID);

            if (itemNest == null)
                return;

            _helperShowedAmount++;


            if (_helperShowedAmount >= 2)
                _eggsGenerator.SetCanChangeSpeed(true);

            _helper.ShowDragHelper(eggHolder.transform, itemNest.transform.position);
        }
    }
}