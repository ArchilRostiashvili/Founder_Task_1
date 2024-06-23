using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BebiLibs;

public class ItemPlaceBase : MonoBehaviour
{
    public Collider2D colliderBox;
    public int state;
    public string placeID;
    public Transform TR_Point;
    public Action<string, ItemPlaceBase> CallBack;
    public string objectType;
    public ItemDrag currentItemProduct;
    protected bool _touching = false;


    virtual public void Show(bool anim)
    {

    }

    virtual public void Hide(bool anim)
    {

    }

    virtual public void Done()
    {

    }

    virtual public void Init()
    {

    }

    virtual public void Touching()
    {
        if(!_touching)
        {
            ManagerSounds.PlayEffect("fx_page17");
            _touching = true;

            this.transform.DOKill();
            this.transform.localScale = Vector3.one;
            this.transform.DOScale(1.18f, 0.1f);
        }
    }

    public virtual void TouchingOut(bool anim = true)
    {
        if(anim)
        {
            if(_touching)
            {
                _touching = false;

                this.transform.DOKill();
                this.transform.DOScale(1.0f, 0.1f);
            }
        }
        else
        {
            _touching = false;
            this.transform.DOKill();
            this.transform.localScale = Vector3.one;
        }
    }

    public virtual void Take(ItemDrag product)
    {

    }

    virtual public Vector2 HelpPoint()
    {
        if(this.TR_Point != null)
        {
            return this.TR_Point.position;
        }
        else
        {
            return this.transform.position;
        }
    }
}
