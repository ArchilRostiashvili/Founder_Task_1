﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressIndicator : MonoBehaviour
{
    public IndicatorElement[] arrayElements;
    private int _index;
    private int _count;

    public int Count
    {
        get
        {
            return _count;
        }
    }

    public bool Win()
    {
        if (_count == _index)
        {
            return true;
        }

        return false;
    }

    public int AddedCount
    {
        get
        {
            return _index;
        }
    }

    public void SetTotalCount(int count, bool anim)
    {
        this.gameObject.SetActive(true);
        _count = count;
        if (anim)
        {
            for (int i = 0; i < this.arrayElements.Length; i++)
            {
                this.arrayElements[i].gameObject.SetActive(false);
                this.arrayElements[i].Activate(false);
                this.arrayElements[i].transform.localScale = Vector3.zero;
            }
            this.StartCoroutine(this.DelayShow());
        }
        else
        {
            for (int i = 0; i < this.arrayElements.Length; i++)
            {
                if (i < count)
                {
                    this.arrayElements[i].transform.localScale = Vector3.one;
                    this.arrayElements[i].gameObject.SetActive(true);
                    this.arrayElements[i].Activate(false);
                }
                else
                {
                    this.arrayElements[i].gameObject.SetActive(false);
                }
            }
        }

        _index = 0;
    }

    private IEnumerator DelayShow()
    {
        for (int i = 0; i < this.arrayElements.Length; i++)
        {
            if (i < _count)
            {
                this.arrayElements[i].gameObject.SetActive(true);
                this.arrayElements[i].transform.DOScale(1.0f, 0.09f);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void AddPoint()
    {
        if (this.arrayElements.Length <= _index)
        {
            return;
        }

        Sequence s = DOTween.Sequence();
        s.Append(this.arrayElements[_index].transform.DOScale(0.7f, 0.08f));
        s.Append(this.arrayElements[_index].transform.DOScale(1.25f, 0.08f));
        s.Append(this.arrayElements[_index].transform.DOScale(1.0f, 0.04f));
        //this.arrayElements[_index].transform.DORotate(new Vector3(0.0f, 0.0f, 6.0f), 0.1f).SetLoops(2, LoopType.Yoyo);
        this.arrayElements[_index].Activate(true);
        _index++;
    }
}
