using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BebiLibs
{
    [System.Serializable]
    public class ButtonPhysics : MonoBehaviour
    {
        public Collider2D _buttonCollider2D;
        public Transform TR_Content;

        public bool buttonEnabled = true;
        public bool doScale = false;

        protected int _state = 0;

        public UnityEvent onClick = new UnityEvent();

        public bool freeOfUI;

        protected Vector3 _wp;
        protected Vector2 _pointerDownPosition;
        protected Vector2 _pointerUpPosition;
        protected Vector3 _scale;


        virtual public void Awake()
        {

            if (_buttonCollider2D == null)
            {
                _buttonCollider2D = this.GetComponent<Collider2D>();
            }

            if (this.TR_Content == null)
            {
                _scale = this.transform.localScale;
                this.TR_Content = this.transform;
            }
            else
            {
                _scale = this.TR_Content.localScale;
            }
        }

        public bool ButtonEnabled
        {
            set
            {
                _state = 0;
                this.buttonEnabled = value;
                if (_buttonCollider2D != null)
                {
                    _buttonCollider2D.enabled = this.buttonEnabled;
                }
            }
        }

        public void SetCollider(Collider2D collider)
        {
            _buttonCollider2D = collider;
        }

        protected Touch _touch;

        virtual public void Update()
        {
            if (!this.buttonEnabled)
            {
                return;
            }

#if UNITY_EDITOR

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() && !this.freeOfUI)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _pointerDownPosition = Input.mousePosition;
                if (this.IsTouchColliding(Input.mousePosition))
                {
                    if (doScale)
                    {
                        this.TR_Content.DOKill();
                        this.TR_Content.DOScale(_scale * 1.1f, 0.1f);
                    }

                    _state = 1;
                    return;
                }
            }
            else
            if (Input.GetMouseButtonUp(0) && _state == 1)
            {
                _pointerUpPosition = Input.mousePosition;
                if (this.IsTouchColliding(Input.mousePosition) && Vector2.Distance(_pointerDownPosition, _pointerUpPosition) < 40f)
                {
                    if (doScale)
                    {
                        this.TR_Content.DOKill();
                        this.TR_Content.DOScale(_scale, 0.1f);
                    }

                    OnClicked();
                    _state = 0;

                    return;
                }
                else
                {
                    if (_state == 1)
                    {
                        if (doScale)
                        {
                            this.TR_Content.DOKill();
                            this.TR_Content.DOScale(_scale, 0.1f);
                        }

                        _state = 0;
                    }
                }
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId) && !this.freeOfUI)
                {
                    continue;
                }

                if (_state == 0)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (this.IsTouchColliding(touch.position))
                        {
                            _touch = touch;
                            _pointerDownPosition = touch.position;
                            _state = 1;
                            if(doScale){
                                this.TR_Content.DOScale(_scale * 1.1f, 0.1f);
                            }
                            
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
                        if(doScale){
                            this.TR_Content.DOKill();
                            this.TR_Content.DOScale(_scale, 0.1f);
                        }
                        if (this.IsTouchColliding(touch.position))
                        {
                            this.OnClicked();
                            return;
                        }
                    }
                }
            }
#endif

        }

        public virtual void OnClicked()
        {
            if (this.onClick != null)
            {
                _state = 0;
                this.onClick.Invoke();
            }
        }

        public bool IsTouchColliding(Vector2 v)
        {
            _wp = Camera.main.ScreenToWorldPoint(v);

            if (_buttonCollider2D == null)
            {
                _buttonCollider2D = this.GetComponent<Collider2D>();
                if (_buttonCollider2D != null)
                {
                    return _buttonCollider2D.OverlapPoint(_wp);
                }
                else
                {
                    Common.DebugLog("Forgont to Attach Colllider With ButtonPhysics Component");
                    return false;
                }
            }
            else
            {
                return _buttonCollider2D.OverlapPoint(_wp);
            }
        }

    }
}
