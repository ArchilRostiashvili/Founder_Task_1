using System;
using System.Collections.Generic;
using FarmLife.Controllers.Tap;
using FarmLife.MiniGames.TractorFruit;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FarmLife.TractorFruits
{
    public class ItemTree : MonoBehaviour
    {
        public Action<ItemTree> IsRegenerated;

        public string FruitID;
        public bool HasToBeTrue;

        [SerializeField] private List<ItemFruit> _fruitList = new List<ItemFruit>();
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private TapBehavior _tapBehavior;
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private Transform _endingPoint;
        [SerializeField] private float _minimumY;
        [SerializeField] private float _maximumY;

        private int _counter;
        private bool _isActivated;

        public TapBehavior TapBehavior => _tapBehavior;
        public bool HasActiveFruits => _counter < _fruitList.Count;

        public void ResetTree()
        {
            _counter = 0;
            _tapBehavior.EnableCollider(true);
        }

        public void Correct()
        {
            if (_counter >= _fruitList.Count)
                return;

            ItemFruit fruit = _fruitList[_counter];
            fruit.Correct();

            _counter++;
        }

        public void SetData(FruitData data)
        {
            foreach (ItemFruit itemFruit in _fruitList)
            {
                itemFruit.SetData(data.FruitSprite);
            }

            _tapBehavior.SetID(data.FruitID);
            _isActivated = true;
        }

        private void Update()
        {
            if (!_isActivated) return;

            if (transform.position.x < _endingPoint.position.x)
            {
                float randY = Random.Range(_minimumY, _maximumY);

                transform.position = new Vector3(_startingPoint.position.x, randY, _startingPoint.position.z);
                IsRegenerated?.Invoke(this);
            }
        }

        public void Show()
        {
            _feelAnimator.Play(AnimationNamesData.ANIM_SHOW);
            foreach (var fruit in _fruitList)
            {
                fruit.transform.GetChild(0).DOScale(1, 0.2f);
            }
        }

        public void Finish()
        {
            for (int i = _counter; i < _fruitList.Count; i++)
                _fruitList[i].Correct();
            _feelAnimator.Play("Cleared");
        }

        public void Deactivate()
        {
            _isActivated = false;
        }

        internal void CheckForCompletion()
        {
            if (_counter >= _fruitList.Count)
            {
                _tapBehavior.EnableCollider(false);
                _feelAnimator.Play("Cleared");
            }
            else
                _tapBehavior.EnableCollider(true);
        }
    }
}