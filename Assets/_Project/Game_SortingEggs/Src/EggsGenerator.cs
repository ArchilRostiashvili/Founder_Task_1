using System;
using System.Collections.Generic;
using FarmLife.Controllers.Drag;
using FarmLife.MiniGames.SortingOfEggs;
using UnityEngine;

namespace FarmLife
{
    public class EggsGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [SerializeField] private float _eggSpeed;

        private bool _isActivated = false;
        private bool _canChangeSpeed;
        private bool _isEggVisible;
        private float _tempSpeedTime;
        private float _actualSpeed;
        private float _targetSpeed;

        private List<DraggableEggHolder> _eggList = new List<DraggableEggHolder>();
        private Vector2 _rightScreenPoint;

        public void Activate()
        {
            _actualSpeed = _eggSpeed;
            _isActivated = true;
        }

        public bool SetCanChangeSpeed(bool canChangeSpeed)
            => _canChangeSpeed = canChangeSpeed;

        public void AddDraggableEgg(DraggableEggHolder egg)
        {
            if (_eggList.Contains(egg))
                return;

            _eggList.Add(egg);
        }

        public void OnCorrectlyDragged(DragBehavior dragBehavior)
        {
            DraggableEggHolder draggableEgg = _eggList.Find((x) => x.DraggableEgg.DragBehavior == dragBehavior);
            if (dragBehavior == null)
                return;

            _eggList.Remove(draggableEgg);
        }

        public void GetRightScreenBorder()
            => _rightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0f));


        public DraggableEgg GetActiveEgg()
        {
            if (_eggList.Count < 1)
                return null;

            Utils.Shuffle(ref _eggList);
            DraggableEggHolder egg = _eggList.Find((x) => (IsEggVisible(x)));

            if (egg == null)
                return null;

            return egg.DraggableEgg;
        }

        private bool IsEggVisible(DraggableEggHolder egg)
        {
            return egg.MovableTransform.position.x < _rightScreenPoint.x && egg.MovableTransform.position.x > (-_rightScreenPoint.x);
        }

        private void ChangeSpeed()
        {
            if (!_canChangeSpeed)
                return;

            CheckForVisibleEggs();
        }

        private void Update()
        {
            if (!_isActivated)
                return;
            MoveEggs();
        }

        private void MoveEggs()
        {
            for (int i = 0; i < _eggList.Count; i++)
            {
                DraggableEggHolder item = _eggList[i];

                if (item.MovableTransform.position.x < _endPoint.position.x)
                {
                    Vector3 position = item.transform.position;
                    position = new Vector3(_startPoint.position.x, position.y,
                        position.z);
                    item.MovableTransform.position = position;
                }

                item.MovableTransform.Translate(Vector3.left * (Time.deltaTime * _actualSpeed));
            }

            ChangeSpeed();
        }


        private void CheckForVisibleEggs()
        {
            for (int i = 0; i < _eggList.Count; i++)
            {
                if (IsEggVisible(_eggList[i]))
                {
                    if (!_isEggVisible)
                    {
                        _isEggVisible = true;
                        _tempSpeedTime = 0;
                    }

                    return;
                }
            }

            if (!_isEggVisible)
                return;

            _tempSpeedTime = 0;
            _isEggVisible = false;
            _targetSpeed = _eggSpeed;
        }
    }
}