using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace BebiLibs
{
    public class SwipeExitButton : AbstractExitButton, IPointerDownHandler, IEndDragHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _parentRectTransform;
        [SerializeField] private RectTransform _swipeRectTransform;
        [SerializeField] private RectTransform _swipeTargetPoint;

        [SerializeField] private Image _swipeImage;
        [SerializeField] private Image _bodyImage;
        [SerializeField] private TMP_Text _dragToExitText;

        private Color _defaultColor;
        private bool _isInitialized = false;

        private Vector3 _startPosition;
        private Vector3 _middlePosition;
        private Vector3 _endPosition;

        private Vector2 _dragNewPosition;
        private Vector2 _dragLastPosition;
        private Vector2 _offset;
        private bool _dragStart = false;

        private void Start()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _startPosition = _swipeRectTransform.anchoredPosition;
                _endPosition = new Vector3(_swipeTargetPoint.anchoredPosition.x, _startPosition.y, _startPosition.z);
                _middlePosition = Vector2.Lerp(_startPosition, _endPosition, 0.60f);
                _defaultColor = _swipeImage.color;
            }
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            if (_isInitialized)
            {
                _swipeRectTransform.anchoredPosition = _startPosition;
                _dragStart = false;
                _swipeImage.color = _defaultColor;
                _bodyImage.color = _defaultColor;
                _dragToExitText.gameObject.SetActive(false);
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            _swipeImage.color = Color.white;
            _bodyImage.color = Color.white;
            _dragToExitText.gameObject.SetActive(true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragLastPosition = eventData.position;
            _dragStart = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragStart)
            {
                _dragNewPosition = eventData.position;
                Vector2 delta = _dragNewPosition - _dragLastPosition;
                _dragLastPosition = _dragNewPosition;

                Vector2 swipePosition = _swipeRectTransform.anchoredPosition;
                Vector2 newPos = swipePosition + delta;
                newPos.y = _startPosition.y;
                if (IsBetweenPoints(newPos, _startPosition, _endPosition))
                {
                    _swipeRectTransform.anchoredPosition = newPos;
                }
            }
        }

        bool IsBetweenPoints(Vector3 point, Vector3 start, Vector3 end)
        {
            return Vector3.Dot((end - start).normalized, (point - end).normalized) < 0f && Vector3.Dot((start - end).normalized, (point - start).normalized) < 0f;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 position = _swipeRectTransform.anchoredPosition;
            if (IsBetweenPoints(position, _middlePosition, _endPosition))
            {
                ManagerSounds.PlayEffect("fx_page16");
                _gameExitEvent?.Invoke();
                _swipeRectTransform.anchoredPosition = _startPosition;
            }
            else
            {
                _swipeRectTransform.anchoredPosition = _startPosition;
            }
            _dragStart = false;
            _swipeImage.color = _defaultColor;
            _bodyImage.color = _defaultColor;
            _dragToExitText.gameObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _swipeImage.color = _defaultColor;
            _bodyImage.color = _defaultColor;
            _dragToExitText.gameObject.SetActive(true);
        }
    }
}
