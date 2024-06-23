using BebiLibs;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ItemDrag : MonoBehaviour
{
    public string objectID;

    public SpriteRenderer SR_Body;
    public int state;

    public Collider2D colliderProduct;
    protected Vector2 _defaultPosition;

    protected Vector3 _wp;
    protected Vector2 _dragDiff;
    public float defaultScale;
    public Action<string> CallBack;

    protected float _autoScaleTime = 0.1f;
    protected float _autoScaleValue = 1.1f;
    protected ItemPlaceBase _currentTouchingContainer;
    protected Collider2D[] _arrayTouchingColliders;
    public SortingGroup SG;
    //protected Sequence _s;
    public virtual void Init()
    {

    }

    public virtual void Show(bool anim)
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide(bool anim)
    {
        this.gameObject.SetActive(false);
    }

    public Vector2 DefaultPosition
    {
        get
        {
            return _defaultPosition;
        }
        set
        {
            _defaultPosition = value;
        }
    }


    public void SetDefaultPoint()
    {
        _defaultPosition = this.transform.position;
    }

    protected virtual void CheckConditions()
    {

    }

    protected virtual void TouchDown()
    {
        ManagerSounds.PlayEffect("fx_drag2");
    }

    protected Touch _touch;
    protected Vector3 _previousMousePos;

    public virtual void Update()
    {

#if UNITY_EDITOR
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(Input.GetMouseButtonDown(0) && this.state == 1)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(this.colliderProduct.OverlapPoint(mouseWorldPosition))
            {
                Collider2D[] overlap = Physics2D.OverlapPointAll(mouseWorldPosition);
                for (int i = 0; i < overlap.Length; i++)
                {
                    if (overlap[i].gameObject.name != this.gameObject.name)
                    {
                        return;
                    }
                }
                MiniGameBase.TouchDownProduct(this.transform.position, this);

                this.SetState(2);

                this.TouchDown();

                _previousMousePos = mouseWorldPosition;
            }
        }
        else
        if(Input.GetMouseButtonUp(0) && this.state == 2)
        {
            this.SetState(100);
            this.CheckConditions();
        }
        else if(Input.GetMouseButton(0) && this.state == 2)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.Translate((mouseWorldPosition - _previousMousePos));
            _previousMousePos = mouseWorldPosition;
            MiniGameBase.TouchMoveProduct(this.transform.position, this);
        }
#endif
        Touch touch;
        for(int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if(this.state == 1)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    if(this.colliderProduct.OverlapPoint(mouseWorldPosition))
                    {
                        _touch = touch;
                        MiniGameBase.TouchDownProduct(this.transform.position, this);


                        this.SetState(2);
                        this.TouchDown();
                        _previousMousePos = mouseWorldPosition;
                    }
                }
            }
            else
            if(this.state == 2)
            {
                if(touch.fingerId == _touch.fingerId && touch.phase != TouchPhase.Ended)
                {
                    this.transform.Translate((mouseWorldPosition - _previousMousePos));
                    _previousMousePos = mouseWorldPosition;
                    MiniGameBase.TouchMoveProduct(this.transform.position, this);
                }
                else
                if(touch.phase == TouchPhase.Ended && touch.fingerId == _touch.fingerId)
                {
                    _touch.fingerId = -1;
                    this.SetState(100);
                    this.CheckConditions();
                }
                else
                if(touch.phase == TouchPhase.Canceled && touch.fingerId == _touch.fingerId)
                {
                    _touch.fingerId = -1;
                    this.SetState(100);
                    this.CheckConditions();
                }
            }
        }
    }

    virtual public void SetState(int state)
    {
        this.state = state;
    }


    private static int _TryAgainCount = 0;

    public static void PlayTryAgain()
    {
        if(_TryAgainCount == 0)
        {
            ManagerSounds.PlayEffect("fx_tx_try_again");
        }
        _TryAgainCount++;
        if(1 < _TryAgainCount)
        {
            _TryAgainCount = 0;
        }
    }

    public void PlayTryAgainItem()
    {
        if(_TryAgainCount == 0)
        {
            ManagerSounds.PlayEffect("fx_tx_try_again");
        }
        _TryAgainCount++;
        if(1 < _TryAgainCount)
        {
            _TryAgainCount = 0;
        }
    }

    virtual public void ShakeAndBack()
    {
        Sequence s = DOTween.Sequence();
        if(s != null)
        {
            s.Kill();
            s = null;
        }
        ItemDrag.PlayTryAgain();

        s = DOTween.Sequence();
        s.AppendCallback(() =>
        {
            this.SR_Body.color = Color.red;
        });
        s.Append(this.transform.DOShakeRotation(0.22f, 50.0f, 0, 1));
        s.AppendInterval(0.1f);
        s.AppendCallback(() =>
        {
            this.SR_Body.color = Color.white;
        });
        s.Append(this.transform.DOJump(_defaultPosition, 2.0f, 1, 0.15f).SetEase(Ease.OutQuad));
        s.OnComplete(() =>
        {
            this.SetState(1);
        });
    }

    virtual public void ToBack(bool anim = true)
    {
        Sequence s = DOTween.Sequence();
        if(s != null)
        {
            s.Kill();
            s = null;
        }

        if(anim)
        {
            ItemDrag.PlayTryAgain();

            s = DOTween.Sequence();
            s.AppendCallback(() =>
            {
                this.SR_Body.color = Color.red;
            });
            s.AppendInterval(0.3f);
            s.AppendCallback(() =>
            {
                this.SR_Body.color = Color.white;
            });
            s.Append(this.transform.DOJump(_defaultPosition, 2.0f, 1, 0.15f).SetEase(Ease.OutQuad));
            s.OnComplete(() =>
            {
                this.SetState(1);
            });
        }
        else
        {
            this.SetState(0);
            this.transform.position = _defaultPosition;
            this.Hide(false);
        }
    }

    virtual public Vector2 HelpPoint()
    {
        return this.transform.position;
    }

    virtual public void SetSortingOrder(int orderId)
    {
        this.SG.sortingOrder = orderId;
    }
}
