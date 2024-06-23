using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;

public class ItemEggHelper : MonoBehaviour
{
    public SpriteRenderer SR_Glow;
    public Animator EggBreaker;
    public Transform TR_ItemChar;
    public ItemParticleSystem SC_ParticleSystemSelected;

    public Transform TR_RoundGlow;

    public GameObject GO_EggSimple;

    public SpriteRenderer SR_EggTop;
    public SpriteRenderer SR_EggBottom;
    public SpriteRenderer SR_EggSimple;

    public Transform Point_Center;

    private void Awake()
    {
        this.Animation(0, null);
    }

    public void SetData(Sprite spEgg, Vector2 p, Sprite spChar, float scale)
    {
        this.SR_EggTop.sprite = spEgg;
        this.SR_EggBottom.sprite = spEgg;
        this.SR_EggSimple.sprite = spEgg;
        this.transform.position = p;
        this.transform.localScale = Vector3.one * scale;
        //this.TR_ItemChar.gameObject.GetComponent<SpriteRenderer>().sprite = spChar;
    }


    public void Play(Action<int> callBackFinish)
    {
        this.Animation(1, callBackFinish);
        ManagerSounds.PlayEffect("fx_correct7");
        
        
        Sequence s = DOTween.Sequence();
        s.AppendInterval(0.1f);
        s.AppendCallback(() =>
        {
            this.Animation(2, callBackFinish);
        });
        s.AppendInterval(1.2f);
        s.AppendCallback(() =>
        {
            this.transform.DOMove(this.Point_Center.position, 1.0f);
            this.transform.DOScale(2.0f, 1.0f);
        });
        s.AppendInterval(2.0f);
        s.AppendCallback(() =>
        {
            callBackFinish(1);
            this.Animation(3, callBackFinish);
        });
        s.AppendInterval(3.4f);
        s.AppendCallback(() =>
        {
            this.Animation(4, callBackFinish);
        });
        s.AppendInterval(0.2f);
        s.AppendCallback(() =>
        {
            callBackFinish(2);
        });
    }

    public void Animation(int anim, Action<int> callBackFinish)
    {
        //Debug.Log("anim = " + anim);
        if (anim == 0)
        {
            this.gameObject.SetActive(false);
            this.SR_Glow.gameObject.SetActive(false);
            this.TR_ItemChar.gameObject.SetActive(false);
            this.EggBreaker.gameObject.SetActive(false);
            this.SC_ParticleSystemSelected.Stop();
            this.GO_EggSimple.SetActive(false);
            this.TR_RoundGlow.gameObject.SetActive(false);

            this.SR_Glow.DOKill();
            this.TR_RoundGlow.DOKill();
        }
        else
        if (anim == 1)
        {
            this.gameObject.SetActive(true);
            this.GO_EggSimple.SetActive(true);
            this.SC_ParticleSystemSelected.transform.position = this.transform.position;
            this.SC_ParticleSystemSelected.Play();
        }
        else
        if (anim == 2)
        {
            this.SR_Glow.gameObject.SetActive(true);
            this.SR_Glow.color = new Color(234.0f / 255.0f, 1.0f, 0.0f, 0.5f);
            this.SR_Glow.DOFade(1.0f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        if (anim == 3)
        {
            this.GO_EggSimple.SetActive(false);
            //this.TR_ItemChar.gameObject.SetActive(true);
            this.EggBreaker.gameObject.SetActive(true);
            ManagerSounds.PlayEffect("fx_break2");
            ManagerTime.Delay(1.2f, () =>
            {
                callBackFinish(0);
                ManagerSounds.PlayEffect("fx_correct1");

                this.SR_Glow.DOKill();
                this.SR_Glow.DOFade(0.3f, 0.2f);
                this.TR_RoundGlow.GetComponent<SpriteRenderer>().color = Color.white;
                this.TR_RoundGlow.localScale = Vector2.zero;
                this.TR_RoundGlow.gameObject.SetActive(true);
                this.TR_RoundGlow.DOScale(0.5f, 0.4f).OnComplete(() =>
                {
                    this.TR_RoundGlow.DORotate(new Vector3(0.0f, 0.0f, 359.0f), 8.3f, RotateMode.WorldAxisAdd);
                });
            });
        }
        else
        if (anim == 4)
        {
            this.TR_RoundGlow.GetComponent<SpriteRenderer>().DOFade(0.0f, 0.4f);
            this.SR_Glow.DOFade(0.0f, 0.4f).OnComplete(() =>
            {
                this.Animation(0, null);
            });
        }
    }
}
