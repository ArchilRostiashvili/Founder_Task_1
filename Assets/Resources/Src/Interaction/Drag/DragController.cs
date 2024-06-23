using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FarmLife.Controllers.Drag
{
    public class DragController : MonoBehaviour
    {
        public Action<DragBehavior> CorrectlyDraggedEvent;
        public Action AllCorrectlyDraggedEvent;
        public Action IncorrectlyDraggedEvent;
        public Action DragStartEvent;
        public Action DragEndEvent;

        [SerializeField] private bool _removeDraggableAfterDrag;

        private List<DragBehavior> _draggableList = new List<DragBehavior>();
        private List<DragTargetBehavior> _targetList = new List<DragTargetBehavior>();
        private ICollisionBehavior _collisionBehavior;

        private int _correctlyDragged, _draggableListCount;

        private bool _dragStarted;

        public void Init()
        {
            TryGetCollisionBehaviorComponent();
            _draggableListCount = 0;
            _correctlyDragged = 0;
            _draggableList.Clear();
            _targetList.Clear();
        }

        public void Reset()
        {
            _correctlyDragged = 0;
        }

        public void SetDraggableAmount(int draggableAmount)
            => _draggableListCount = draggableAmount;

        private void TryGetCollisionBehaviorComponent()
        {
            _collisionBehavior = GetComponent<ICollisionBehavior>();
        }

        public void AddDraggable(DragBehavior dragBehavior)
        {
            if (_draggableList.Contains(dragBehavior))
                return;

            _draggableList.Add(dragBehavior);
            SubscribeToEvents(dragBehavior);
        }

        public void AddTarget(DragTargetBehavior dragTargetBehavior)
        {
            if (_targetList.Contains(dragTargetBehavior))
                return;

            _targetList.Add(dragTargetBehavior);
            _draggableListCount++;
        }

        public void EnableDraggable(bool enable, DragBehavior dragBehavior = null)
        {
            for (int i = 0; i < _draggableList.Count; i++)
            {
                if (_draggableList[i] == dragBehavior)
                    continue;

                _draggableList[i].SetCanDrag(enable);
                _draggableList[i].EnableCollider(enable);
            }
        }

        private void SubscribeToEvents(DragBehavior dragBehavior)
        {
            dragBehavior.TouchDownEvent += OnTouchDown;
            dragBehavior.DragStartEvent += OnDragStart;
            dragBehavior.DragEvent += OnDrag;
            dragBehavior.DragEndEvent += OnDragEnd;
            dragBehavior.TouchUpEvent += OnTouchUp;
        }

        private void OnTouchUp(DragBehavior dragBehavior)
        {
            if (_dragStarted)
                return;

            DragEndEvent?.Invoke();
            dragBehavior.PlayDragEnd();
            dragBehavior.EnableCollider(false);
            dragBehavior.Return(() =>
                {
                    EnableDraggable(true);
                });
        }

        private void OnDrag(DragBehavior dragBehavior)
        {
            if (_collisionBehavior == null)
                return;

            _collisionBehavior.CollisionEnter(dragBehavior, _targetList);
        }

        private void OnDragEnd(DragBehavior dragBehavior)
        {
            if (_collisionBehavior != null)
                _collisionBehavior.CollisionExit(dragBehavior, _targetList);

            _dragStarted = false;
            DragEndEvent?.Invoke();

            dragBehavior.PlayDragEnd();

            if (!dragBehavior.CheckDraggableColliderOverlap() || _collisionBehavior == null)
            {
                dragBehavior.EnableCollider(false);
                dragBehavior.Return(() =>
                    {
                        EnableDraggable(true);
                    });

                return;
            }

            if (_collisionBehavior != null)
                _collisionBehavior.CollisionFinish(dragBehavior, _targetList, AfterIncorrectlyDragged, AfterCorrectDrag);
        }

        private void AfterIncorrectlyDragged(DragBehavior behavior)
        {
            IncorrectlyDraggedEvent?.Invoke();
            EnableDraggable(true);
        }

        private void AfterCorrectDrag(DragBehavior dragBehavior, DragTargetBehavior targetBehavior)
        {
            CheckForDragFinish(dragBehavior);
            CorrectlyDraggedEvent?.Invoke(dragBehavior);
            if (_removeDraggableAfterDrag)
            {
                _draggableList.Remove(dragBehavior);
            }
        }

        private void CheckForDragFinish(DragBehavior dragBehavior)
        {
            _correctlyDragged++;
            if (_correctlyDragged >= _draggableListCount)
                AllCorrectlyDraggedEvent?.Invoke();
            else
                EnableDraggable(true, dragBehavior);
        }

        private void OnDragStart(DragBehavior dragBehavior)
        {
            _dragStarted = true;
        }

        private void OnTouchDown(DragBehavior dragBehavior)
        {
            EnableDraggable(false, dragBehavior);

            DragStartEvent?.Invoke();
            dragBehavior.PlayDragBegin();
        }
    }
}