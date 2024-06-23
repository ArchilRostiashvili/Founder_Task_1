using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartArrowShaker : MonoBehaviour
{
    void OnDisable()
    {
        this.transform.DOKill();
    }

    void OnEnable()
    {
        float angle = 8.0f;
        this.transform.parent.DOScale(1.13f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        this.transform.DORotate(new Vector3(0.0f, 0.0f, -angle), 0.4f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
