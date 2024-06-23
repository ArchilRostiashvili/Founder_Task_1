using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BebiLibs
{
    public class ButtonPhysicsUI : MonoBehaviour
    {
        private Collider2D _buttonCollider2D;

        public Transform TR_Content;

        public bool buttonEnabled = true;

        private int _state = 0;

        public UnityEngine.UI.Button.ButtonClickedEvent onClick;

        private Vector3 _wp;
        private Vector2 _pointerDownPosition;
        private Vector2 _pointerUpPosition;
        private float _scale;
        private void Awake()
        {

            _buttonCollider2D = this.GetComponent<Collider2D>();
            _scale = this.TR_Content.localScale.y;
        }

        public bool ButtonEnabled
        {
            set
            {
                this.buttonEnabled = value;
            }
        }

        private Touch _touch;

        private void Update()
        {
            if (!this.buttonEnabled)
            {
                return;
            }

#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                _pointerDownPosition = Input.mousePosition;
                if (this.IsTouchColliding(Input.mousePosition))
                {
                    this.TR_Content.DOKill();
                    this.TR_Content.DOScale(_scale * 1.1f, 0.1f);
                    _state = 1;
                    return;
                }
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                _pointerUpPosition = Input.mousePosition;
                if (this.IsTouchColliding(Input.mousePosition) && Vector2.Distance(_pointerDownPosition, _pointerUpPosition) < 30f)
                {
                    this.TR_Content.DOKill();
                    this.TR_Content.DOScale(_scale, 0.1f);
                    OnClicked();
                    _state = 0;

                    return;
                }
                else
                {
                    if (_state == 1)
                    {
                        this.TR_Content.DOKill();
                        this.TR_Content.DOScale(_scale, 0.1f);
                        _state = 0;
                    }
                }
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                if (_state == 0)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (this.IsTouchColliding(touch.position))
                        {
                            _touch = touch;
                            _pointerDownPosition = touch.position;
                            _state = 1;
                            this.TR_Content.DOScale(_scale*1.1f, 0.1f);
                            return;
                        }
                    }
                }
                else
                if (_state == 1)
                {
                    if (touch.phase == TouchPhase.Ended && touch.fingerId == _touch.fingerId)
                    {
                        _state = 0;
                        this.TR_Content.DOKill();
                        this.TR_Content.DOScale(_scale, 0.1f);
                        if (this.IsTouchColliding(touch.position))
                        {
                            this.OnClicked();
                            return;
                        }
                    }
                }
            }
            /*
            Touch touch = Input.GetTouch(Input.touchCount - 1);
            if (_touch.phase == TouchPhase.Began && _state == 0)
            {
                
            }


            if (touch.fingerId == _touch.fingerId)
            {
                
            }

            


            if (this.IsTouchColliding(touch.position))
            {
                _touch = touch;
                this.TR_Content.DOKill();
                if (_touch.phase == TouchPhase.Began)
                {
                    _pointerDownPosition = touch.position;
                    _state = 1;
                    this.TR_Content.DOScale(_scale + 0.1f, 0.1f);
                    return;
                }

                if (_touch.phase == TouchPhase.Ended)
                {
                    _pointerUpPosition = Input.mousePosition;
                    _state = 0;
                    this.TR_Content.DOScale(_scale, 0.1f);
                    if (Vector2.Distance(_pointerDownPosition, _pointerUpPosition) < 30f)
                    {
                        this.OnClicked();
                    }
                        
                    return;
                }
            }
            else
            {
                if (_state == 1)
                {
                    this.TR_Content.DOKill();
                    this.TR_Content.DOScale(_scale, 0.1f);
                    _state = 0;
                }
            }
            */
#endif

        }

        /// <summary>
        /// Called when we click or touch this button
        /// </summary>
        public virtual void OnClicked()
        {
            if (this.onClick != null)
            {
                this.onClick.Invoke();
            }
        }

        public bool IsTouchColliding(Vector2 v)
        {
            _wp = Camera.main.ScreenToWorldPoint(v);
            if (_buttonCollider2D == Physics2D.OverlapPoint(_wp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
