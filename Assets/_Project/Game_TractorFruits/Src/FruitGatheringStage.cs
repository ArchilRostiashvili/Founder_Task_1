using System;
using System.Collections;
using System.Collections.Generic;
using BebiLibs.AudioSystem;
using FarmLife.Controllers.Tap;
using FarmLife.MiniGames.Base;
using FarmLife.TractorFruits;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    public class FruitGatheringStage : MiniGameStageBase
    {
        [SerializeField] private ItemTractor _itemTractor;
        [SerializeField] private RoadGenerator _roadGenerator;
        [SerializeField] private List<ItemTree> _treeList = new List<ItemTree>();
        [SerializeField] private TapController _tapController;
        [SerializeField] private TractorFruitsData _fruitData;
        [SerializeField] private List<FruitUi> _fruitUiList = new List<FruitUi>();
        [SerializeField] private ColoredProgressBar _coloredProgressBar;

        [SerializeField] private int _correctAmount;

        private List<FruitData> _fruitDataList = new List<FruitData>();
        private FruitData _correctData;
        private int _answeredAmount;
        private int _correctFruitIndex;

        public override void Activate()
        {
            base.Activate();
        }

        public override void Init()
        {
            base.Init();
            CreateData();
            _correctData = _fruitDataList[_correctFruitIndex];

            InitTrees();
            InitUI();

            InitHelperHand();

            _tapController.SetID(_correctData.FruitID);
            _tapController.CorrectlyAnsweredEvent += OnCorrectlyAnswered;
            _coloredProgressBar.SetData(_correctAmount, Color.white, _correctData.FruitSprite);
            _coloredProgressBar.ProgressUpWithoutAnimation(.15f);
        }

        private void InitUI()
        {
            for (int i = 0; i < _fruitUiList.Count; i++)
            {
                _fruitUiList[i].SetInactive();
                _fruitUiList[i].SetCheckMarkState(false);
            }

            _fruitUiList[_correctFruitIndex].Activate();
        }

        private void InitTrees()
        {
            Utils.Shuffle(ref _treeList);

            for (int i = 0; i < _treeList.Count; i++)
            {
                _treeList[i].IsRegenerated += RegenerateData;
                _treeList[i].SetData(_fruitDataList[i]);
                _treeList[i].TapBehavior.WrongEvent += IncrementIncorrectAnswer;

                if (_correctData == _fruitDataList[i])
                    _treeList[i].HasToBeTrue = true;

                _itemTractor.SetData(_fruitDataList[i].FruitSprite, i, _fruitDataList[i].FruitID);

                _tapController.AddTapBehavior(_treeList[i].TapBehavior);
            }

            _itemTractor.SetCorrectBox();
        }

        private void CreateData()
        {
            _fruitData.CreateData(_treeList.Count);

            for (int i = 0; i < _fruitData.ActiveFruitList.Count; i++)
            {
                _fruitDataList.Add(_fruitData.ActiveFruitList[i]);
                _fruitUiList[i].SetData(_fruitData.ActiveFruitList[i].FruitSprite);
            }
        }

        public override void StartRound()
        {
            base.StartRound();
            ShowAnimation(() =>
            {
                EnableHelperTracking();
                _roadGenerator.Activate();
                _tapController.EnableTapBehaviors(true);
            });
        }

        private void OnCorrectlyAnswered(TapBehavior tapBehavior)
        {
            _answeredAmount++;
            _coloredProgressBar.ProgressUp(.085f);
            _tapController.Init();
            ItemTree tree = tapBehavior.GetComponent<ItemTree>();

            if (_answeredAmount >= _correctAmount)
            {
                tree.Finish();
                tapBehavior.EnableCollider(false);
                TreeDone();
            }
            else
            {
                _helper.Reset();
                tree.CheckForCompletion();
                _itemTractor.Correct();
            }
        }

        private void TreeDone()
        {
            _answeredAmount = 0;
            _tapController.SetIsActive(false);
            _fruitUiList[_correctFruitIndex].SetCheckMarkState(true);
            DisableHelperTracking();

            for (int i = 0; i < _treeList.Count; i++)
            {
                _treeList[i].HasToBeTrue = false;
                _treeList[i].CheckForCompletion();
            }

            _correctFruitIndex++;

            if (_correctFruitIndex >= _fruitDataList.Count)
            {
                _treeList.ForEach((x) => x.Deactivate());
                _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT, () =>
                {
                    HideAnimation(() =>
                    {
                        RoundFinishEvent?.Invoke();
                    });
                });

            }
            else
            {
                NewFruit();
            }
        }

        private void NewFruit()
        {
            _correctData = _fruitDataList[_correctFruitIndex];
            ItemTree itemTree = _treeList.Find((x) => x.TapBehavior.ID == _correctData.FruitID);
            ResetProgressBar();

            if (itemTree != null)
                itemTree.HasToBeTrue = true;

            _fruitUiList[_correctFruitIndex].Activate();

            _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT, () =>
            {
                _itemTractor.ChangeBox();
                _feelAnimator.Play("ChangeBox", () =>
                {
                    EnableHelperTracking();
                    _tapController.SetIsActive(true);
                    _tapController.SetID(_correctData.FruitID);
                });
            });
        }

        private void ResetProgressBar()
        {
            _coloredProgressBar.Reset();
            _coloredProgressBar.SetData(_correctAmount, Color.white, _correctData.FruitSprite);
            _coloredProgressBar.ProgressUpWithoutAnimation(.15f);
        }

        private void RegenerateData(ItemTree tree)
        {
            FruitData currentSprite;

            ItemTree itemTree = _treeList.Find((x) => x.TapBehavior.ID == _correctData.FruitID);

            if (tree.HasToBeTrue || itemTree == null)
            {
                currentSprite = _correctData;
            }
            else
            {
                List<FruitData> itemFruits = _fruitData.ActiveFruitList;
                Utils.Shuffle(ref itemFruits);

                currentSprite = itemFruits.Find((x) => x.FruitID != _correctData.FruitID);
            }

            tree.SetData(currentSprite);

            tree.ResetTree();
            // itemTree.SetData(data);
            tree.Show();
        }

        protected override void OnHelpNeeded()
        {
            base.OnHelpNeeded();

            ItemTree itemTree = _treeList.Find((x) => x.TapBehavior.ID == _correctData.FruitID);

            if (itemTree == null)
                return;

            if (!itemTree.HasActiveFruits)
                return;

            _helper.ShowTapHelper(itemTree.transform);
        }
    }
}