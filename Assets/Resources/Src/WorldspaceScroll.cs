using System;
using System.Collections;
using System.Collections.Generic;
using FarmLife.Data.LobbySession;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bebi.Helpers
{
    public class WorldspaceScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action IdleTimePassedEvent;

        public bool IsSwipping => _isSwipping;

        [Header("Main Content")]
        [SerializeField] private Transform _contentTR;

        [SerializeField] private Transform _startElementTR;
        [SerializeField] private Transform _endElementTR;
        [SerializeField] private bool _overstretchEnabled;

        [Header("Scroll Data")]
        [SerializeField] private float _elasticity = 0f;
        [SerializeField] private float _decelerationRate = 0.1f;
        [SerializeField] private float _idleTime = 20f;

        private Bounds _viewBound;
        private Vector2 _velocity;
        private bool _isDragging;
        private bool _isSwipping;

        private Vector2 _startLocalCursor;
        private Vector2 _contentStartPosition;
        private Vector2 _prevPosition;
        private Vector2 _calculatedOffset;
        private int _currentDragPointer;

        private float _idleTimePassed;

        private LobbySessionData _lobbySessionData;

        public void SetLobbySession(LobbySessionData lobbySessionData)
            => _lobbySessionData = lobbySessionData;

        public void GetViewBounds()
        {
            _viewBound = GetScreenBounds(false, false, false, false);
        }

        public void Activate(LobbySessionData lobbySessionData)
            => SetPosition(lobbySessionData.LastParallaxContentPosition);

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging)
                return;

            _idleTimePassed = 0f;
            _startLocalCursor = Camera.main.ScreenToWorldPoint(eventData.position);
            _contentStartPosition = _contentTR.position;
            _currentDragPointer = eventData.pointerId;
            _isDragging = true;
            _isSwipping = true;
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _currentDragPointer)
                return;

            Vector2 localCursor = Camera.main.ScreenToWorldPoint(eventData.position);

            var pointerDelta = localCursor - _startLocalCursor;
            Vector2 position = _contentStartPosition + pointerDelta;

            Vector2 offset = CalculateOffset(position - (Vector2)_contentTR.position);
            position += offset;
            if (offset.x != 0)
                position.x -= RubberDelta(offset.x, _viewBound.size.x);

            SetPosition(position);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _currentDragPointer)
                return;

            _isDragging = false;
            Invoke(nameof(ResetSwipping), 0.1f);
        }

        private void ResetSwipping()
        {
            _isSwipping = false;
        }

        private float RubberDelta(float overStretching, float viewSize)
        {
            if (!_overstretchEnabled)
            {
                overStretching = 0f;
            }

            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }

        private bool Assert(float value)
        {
            return Mathf.Abs(value) > 0.0001f;
        }

        private void Update()
        {
            CheckForIdle();
            _calculatedOffset = CalculateOffset(Vector2.zero);
            if (!_isDragging && (Assert(_velocity.x) || Assert(_calculatedOffset.x)))
            {
                Vector2 position = _contentTR.position;
                if (_calculatedOffset.x != 0)
                {
                    float speed = _velocity.x;
                    position.x = Mathf.SmoothDamp(_contentTR.position.x, _contentTR.position.x + _calculatedOffset.x, ref speed, _elasticity, Mathf.Infinity, Time.fixedDeltaTime);
                    _velocity.x = speed;
                }
                else
                {
                    _velocity.x *= Mathf.Pow(_decelerationRate, Time.fixedDeltaTime);
                    if (Mathf.Abs(_velocity.x) < 0.2)
                    {
                        _velocity.x = 0;
                    }

                    position.x += _velocity.x * Time.fixedDeltaTime;
                }

                if (Assert(_velocity.x))
                {
                    SetPosition(position);
                }
            }

            if (_isDragging)
            {
                Vector2 newVelocity = ((Vector2)_contentTR.position - _prevPosition) / Time.fixedDeltaTime;
                _velocity = Vector2.Lerp(_velocity, newVelocity, Time.fixedDeltaTime * 10);
            }

            if (_contentTR.position.x != _prevPosition.x)
            {
                _prevPosition = _contentTR.position;
            }
        }

        private void CheckForIdle()
        {
            if (_isDragging || _isSwipping)
            {
                _idleTimePassed = 0f;
                return;
            }

            _idleTimePassed += Time.deltaTime;

            if (_idleTimePassed >= _idleTime)
            {
                _idleTimePassed = 0f;
                IdleTimePassedEvent?.Invoke();
            }
        }

        private void SetPosition(Vector2 position)
        {
            position.y = _contentTR.position.y;

            if (position.x != _contentTR.position.x)
            {
                _contentTR.position = position;
                _lobbySessionData.LastParallaxContentPosition = position;
            }
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            if (_startElementTR == null || _endElementTR == null)
            {
                return Vector2.zero;
            }

            Vector2 offset = Vector2.zero;
            Vector2 min = _startElementTR.position;
            Vector2 max = _endElementTR.position;

            min.x += delta.x;
            max.x += delta.x;

            if (min.x > _viewBound.min.x)
                offset.x = _viewBound.min.x - min.x;
            else if (max.x < _viewBound.max.x)
                offset.x = _viewBound.max.x - max.x;

            return offset;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_viewBound.center, _viewBound.size);
        }


        private static Bounds GetScreenBounds(bool includeLeft, bool includeRight, bool includeTop, bool includeButton)
        {
            Camera camera = Camera.main;

            Rect safeRect = Screen.safeArea;
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

            float x = includeLeft ? safeRect.x : screenRect.x;
            float rightOffset = includeLeft ? screenRect.x : safeRect.x;
            float width = includeRight ? safeRect.width + rightOffset : screenRect.width - x;

            float y = includeButton ? safeRect.y : screenRect.y;
            float topOffset = includeButton ? screenRect.y : safeRect.y;
            float height = includeTop ? safeRect.height + topOffset : screenRect.height - y;

            Rect finalRect = new Rect(x, y, width, height);
            Vector2 safeOffset = camera.ScreenToWorldPoint(finalRect.center);
            Bounds bounds = new Bounds(safeOffset, 2 * (Vector2)camera.ScreenToWorldPoint(Screen.safeArea.max));
            return bounds;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}

