using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace BebiInteractions.Libs
{
    public class DragItemBase : InteractableItemBase, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Transform TargetPointTR;
        public Vector3 InitialPosition { get; protected set; }
        public bool CenterDragPoint;

        private bool _hasDragStarted;
        private Vector2 _offset;

        public System.Action<DragItemBase> BeginDragEvent;
        public System.Action<DragItemBase> DragEvent;
        public System.Action<DragItemBase> DragEndEvent;

        public void ResetItem()
        {
            transform.position = InitialPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isEnabled) return;

            _hasDragStarted = true;
            _offset = ContentTR.position - Camera.main.ScreenToWorldPoint(eventData.position);
            BeginDragEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isEnabled || !_hasDragStarted) return;

            Vector2 position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
            
            ContentTR.position = CenterDragPoint ? position :
                position + _offset;
            
            DragEvent?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isEnabled || !_hasDragStarted) return;

            _hasDragStarted = false;
            DragEndEvent?.Invoke(this);
        }

        public void SetInitialLocation(Transform location)
        {
            InitialPosition = location.position;
        }

        public void GetBackToDefaultPosition()
        {
            transform.DOMove(InitialPosition, 0.5f);
        }
    }
}