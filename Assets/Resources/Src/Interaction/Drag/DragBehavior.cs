using System;
using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FarmLife.Controllers.Drag
{
    public class DragBehavior : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action<DragBehavior> DragEvent;
        public Action<DragBehavior> DragStartEvent;
        public Action<DragBehavior> DragEndEvent;
        public Action<DragBehavior> TouchDownEvent;
        public Action<DragBehavior> TouchUpEvent;

        [SerializeField] private Collider2D _draggableCollider;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private bool _hasFeelAnimator;
        [SerializeField] private bool _isDefaultPositionLocal;
        [SerializeField][HideField("_hasFeelAnimator", true)] private FeelAnimator _feelAnimator;
        [SerializeField] private bool _hasHelperTransform;
        [SerializeField][HideField("_hasHelperTransform", true)] private Transform _helperTransform;

        [SerializeField] private string _draggableID;

        private List<Collider2D> _touchingColliderList = new List<Collider2D>();

        private Vector2 _offset;
        private Vector3 _defaultPosition;

        private bool _canDrag, _hasDragStarted, _isHighLighted, _isTouchDown;

        public Collider2D Collider => _draggableCollider;
        public List<Collider2D> TouchingColliderList => _touchingColliderList;

        public string ID => _draggableID;
        public Transform HelperTransform => _helperTransform == null ? transform : _helperTransform;
        public Vector2 DefaultPosition => _defaultPosition;

        public void Init()
        {
            _defaultPosition = _isDefaultPositionLocal ? transform.localPosition : transform.position;
        }

        public void EnableCollider(bool enable)
            => _draggableCollider.enabled = enable;

        public void SetCanDrag(bool canDrag)
            => _canDrag = canDrag;

        public void SetID(string id)
            => _draggableID = id;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_canDrag || !_isTouchDown)
                return;

            _hasDragStarted = true;
            _offset = transform.position - Camera.main.ScreenToWorldPoint(eventData.position);

            DragStartEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag || !_hasDragStarted)
                return;

            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) + _offset;

            DragEvent?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_canDrag || !_hasDragStarted)
                return;

            _canDrag = false;
            _hasDragStarted = false;
            _isTouchDown = false;

            DragEndEvent?.Invoke(this);
        }

        public void GoToPosition(Vector2 position, float time = 0.4f, Ease positionEase = Ease.Linear, Action afterMoveEvent = null)
        {
            transform.DOMove(position, time).SetEase(positionEase).OnComplete(() =>
            {
                afterMoveEvent?.Invoke();
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_canDrag || _isTouchDown)
                return;

            if (!_draggableCollider.enabled)
                return;

            TouchDownEvent?.Invoke(this);
            _isTouchDown = true;
            _touchingColliderList.Clear();
        }

        public void Wrong(Action afterWrongEvent = null)
        {
            if (_feelAnimator == null)
            {
                afterWrongEvent?.Invoke();
                return;
            }

            _feelAnimator.Play(AnimationNamesData.ANIM_WRONG, afterWrongEvent, false);
        }

        public void Correct(Action afterCorrectEvent = null)
        {
            if (_feelAnimator == null)
            {
                afterCorrectEvent?.Invoke();
                return;
            }

            _feelAnimator.Play(AnimationNamesData.ANIM_CORRECT, afterCorrectEvent, false);
        }

        public void Return(Action afterReturnEvent = null)
        {
            if (_feelAnimator == null)
            {
                afterReturnEvent?.Invoke();
                return;
            }

            _feelAnimator.Play(AnimationNamesData.ANIM_RETURN, afterReturnEvent, false);
        }

        public void Highlight(Action afterHighLight = null)
        {
            if (_feelAnimator == null)
                return;

            if (_isHighLighted)
                return;

            _isHighLighted = true;
            _feelAnimator.Play(AnimationNamesData.ANIM_HIGHLIGHT, afterHighLight, false);
        }

        public void HighlightOff(Action afterHighLight = null)
        {
            if (_feelAnimator == null)
                return;

            if (!_isHighLighted)
                return;

            _isHighLighted = false;
            _feelAnimator.Stop(AnimationNamesData.ANIM_HIGHLIGHT);
            _feelAnimator.Play(AnimationNamesData.ANIM_HIGHLIGHT_OFF, () =>
            {
                afterHighLight?.Invoke();
            }, false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_draggableCollider.enabled || !_isTouchDown || _hasDragStarted)
                return;

            _isTouchDown = false;
            TouchUpEvent?.Invoke(this);
        }

        public bool CheckDraggableColliderOverlap()
            => _draggableCollider.OverlapCollider(_contactFilter, _touchingColliderList) > 0;

        public void PlayDragBegin(Action afterDragBegin = null)
        {
            if (_feelAnimator == null)
                return;

            _feelAnimator.Play(AnimationNamesData.ANIM_DRAG_BEGIN, afterDragBegin, false);
        }

        public void PlayDragEnd(Action afterDragBegin = null)
        {
            if (_feelAnimator == null)
                return;
            _feelAnimator.Play(AnimationNamesData.ANIM_DRAG_END, afterDragBegin, false);
        }

        public void DisableHide()
        {
            if (_feelAnimator == null)
                return;

            _feelAnimator.Disable(AnimationNamesData.ANIM_HIDE);
        }
    }
}