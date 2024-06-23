using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;

public class ItemProduct_Egg : MonoBehaviour
{
    public int state;

    public SpriteRenderer SR_Body;
    public SpriteRenderer SR_Frame;
    //public SpriteRenderer SR_Shadow;

    public Transform TR_Content;
    public ParticleElement SC_Smoke;

    private Transform _mainParent;
    private Vector2 _defaultPosition;
    private float _defaultScale;
    //public ItemParticleSystem Particle_Splash;

    private void Awake()
    {
        _mainParent = this.transform.parent;
        _defaultPosition = this.transform.localPosition;
        _defaultScale = this.transform.localScale.x;
        this.gameObject.SetActive(false);
    }

    public void SetData(Sprite sp)
    {
        this.TR_Content.gameObject.SetActive(true);
        this.SC_Smoke.Stop();
        this.gameObject.SetActive(false);
        this.SR_Body.sprite = sp;
        this.SetState(0);
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        this.transform.position = _defaultPosition;

        this.SR_Body.sortingOrder = 5;
        this.SR_Frame.sortingOrder = 6;
        //this.SR_Shadow.sortingOrder = 4;
        //this.SR_Shadow.gameObject.SetActive(true);
    }

    public void Open()
    {
        this.SetState(3);
        this.gameObject.SetActive(false);
    }

    public void Correct()
    {
        this.SR_Body.sortingOrder = 15;
        this.SR_Frame.sortingOrder = 16;
        //this.SR_Shadow.sortingOrder = 14;
    }

    public void MoveIn()
    {
        this.gameObject.SetActive(true);

        //this.transform.
        this.SetState(1);

        this.transform.DOScale(_defaultScale, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            //ManagerSounds.PlayEffect("fx_show2");
            //ManagerSounds.PlayEffect("fx_take1");
            this.SetState(2);
        }); 
    }

    public void MoveOut()
    {
        //this.gameObject.SetActive(true);

        if (this.state != 3)
        {
            this.TR_Content.gameObject.SetActive(false);
            this.SC_Smoke.Play();
        }

        //this.SetState(1);
        /*
        this.transform.DOMoveY(-10.0f, 0.3f).OnComplete(() =>
        {
            this.SetState(0);
        });
        */

        /*
        this.transform.DOScale(0.0f, 0.2f).OnComplete(() =>
        {
            this.SetState(0);
        });
        */
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void SetState(int state)
    {
        this.state = state;
        if (this.state == 0)
        {
            this.transform.DOKill();
            this.transform.eulerAngles = Vector3.zero;
        }
        else
        if (this.state == 1)
        {
            this.transform.DOKill();
            this.transform.eulerAngles = Vector3.zero;
        }
        else
        if (this.state == 2)
        {
            this.transform.DOKill();

            float angleStart = 3.0f;
            float v = UnityEngine.Random.Range(0.5f, angleStart);
            float side = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            float timeStart = 0.6f;
            timeStart = ((v / angleStart) * timeStart) * 0.5f;

            this.transform.eulerAngles = Vector3.zero;
            this.transform.DORotate(new Vector3(0.0f, 0.0f, angleStart * side), timeStart).OnComplete(() =>
            {
                this.transform.DORotate(new Vector3(0.0f, 0.0f, angleStart * side * (-1)), 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            });
        }
        else
        if (this.state == 3)
        {
            this.transform.DOKill();
            this.transform.eulerAngles = Vector3.zero;
        }
    }

    public void Shake()
    {
        Sequence s = DOTween.Sequence();
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
        s.OnComplete(() =>
        {
            this.SetState(2);
        });
    }
}
