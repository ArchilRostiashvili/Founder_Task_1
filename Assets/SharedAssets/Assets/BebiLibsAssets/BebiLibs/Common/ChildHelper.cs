using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildHelper : MonoBehaviour
{
    public Transform TR_Hand;
    public SpriteRenderer SR_Hand;
    public Transform TR_HandInner;

    public Transform TR_Selection;
    public SpriteRenderer SR_Selection;

    public int indexTime;
    public float[] totalTimeToWait;

    public float timeWaitValue = -1.0f;

    public float timeMove;

    private bool _done;

    public Action CallBackHelpNeeded;
    public Action OnSingleHelpComplete;

    private Sequence _s;

    private Transform _parent;

    private void OnDestroy()
    {
        if (TR_Hand != null) TR_Hand.DOKill();
        if (SR_Hand != null) SR_Hand.DOKill();
        if (TR_HandInner != null) TR_HandInner.DOKill();
        if (TR_Selection != null) TR_Selection.DOKill();
        if (SR_Selection != null) SR_Selection.DOKill();
        if (transform != null) transform.DOKill();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _done = false;
        indexTime = 0;
        _parent = transform.parent;
    }

    public void Activate(Vector3 from, Vector3 to)
    {
        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }

        TR_Hand.gameObject.SetActive(true);

        TR_Hand.DOKill();
        SR_Hand.DOKill();
        SR_Selection.DOKill();
        if (_s != null)
        {
            _s.Kill();
            _s = null;
        }

        TR_Hand.position = from;
        SR_Hand.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        SR_Hand.DOFade(1.0f, 0.15f);

        TR_Selection.position = from;
        Color c = SR_Selection.color;
        c.a = 0.0f;
        SR_Selection.gameObject.SetActive(true);
        SR_Selection.color = c;
        SR_Selection.DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        float timeLeft = 0.1f;

        _s = DOTween.Sequence();
        _s.AppendInterval(0.5f);
        _s.Append(TR_Hand.DOMove(to, timeMove));
        _s.AppendInterval(0.5f);
        _s.Append(SR_Hand.DOFade(0.0f, timeLeft));
        _s.AppendInterval(1.0f);
        _s.SetLoops(2);
        _s.OnComplete(() =>
        {
            Playing();
            ContinueHelperHandCountDown();
        });
        _s.SetId(transform);
    }

    public void Activate(Vector3 from)
    {
        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }

        TR_Hand.gameObject.SetActive(true);

        TR_Hand.DOKill();
        SR_Hand.DOKill();
        SR_Selection.DOKill();
        if (_s != null)
        {
            _s.Kill();
            _s = null;
        }

        float timeMove1 = 0.2f;

        TR_HandInner.localPosition = new Vector3(0.7f, 0.0f, 0.0f);
        TR_Hand.position = from;
        SR_Hand.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        SR_Hand.DOFade(1.0f, 0.15f);
        //TR_HandInner.DOLocalMoveX(5.0f, timeMove).SetLoops(-1, LoopType.Yoyo);

        TR_Selection.position = from;
        Color c = SR_Selection.color;
        c.a = 0.0f;
        SR_Selection.gameObject.SetActive(true);
        SR_Selection.color = c;
        SR_Selection.DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        _s = DOTween.Sequence();
        _s.Append(TR_HandInner.DOLocalMoveX(0.0f, timeMove1));
        _s.AppendInterval(0.5f);
        _s.Append(TR_HandInner.DOLocalMoveX(0.7f, timeMove1 * 1.5f));
        _s.AppendInterval(0.1f);
        _s.SetLoops(4);
        _s.OnComplete(() =>
        {
            Playing();
            ContinueHelperHandCountDown();
        });
        _s.SetId(transform);
    }

    private float _scaleDefault;
    public void Activate(Transform parent)
    {
        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }

        TR_Hand.parent = parent;
        SR_Selection.transform.parent = parent;

        _scaleDefault = TR_Hand.localScale.y;

        TR_Hand.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        TR_HandInner.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        TR_Selection.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        TR_Hand.gameObject.SetActive(true);

        TR_Hand.DOKill();
        SR_Hand.DOKill();
        SR_Selection.DOKill();
        if (_s != null)
        {
            _s.Kill();
            _s = null;
        }

        float timeMove1 = 0.2f;


        SR_Hand.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        SR_Hand.DOFade(1.0f, 0.1f);



        Color c = SR_Selection.color;
        c.a = 0.0f;
        SR_Selection.gameObject.SetActive(true);
        SR_Selection.color = c;
        SR_Selection.DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        _s = DOTween.Sequence();
        _s.Append(TR_HandInner.DOScale(_scaleDefault * 1.15f, timeMove1));
        _s.AppendInterval(0.5f);
        _s.Append(TR_HandInner.DOScale(_scaleDefault, timeMove1 * 1.5f));
        _s.AppendInterval(0.1f);
        _s.SetLoops(4);
        _s.OnComplete(() =>
        {
            OnSingleHelpComplete?.Invoke();
            Playing();
            ContinueHelperHandCountDown();
        });
        _s.SetId(transform);
    }

    public void Activate(Vector3 from, Action onComplete)
    {
        StopHelperHandCountDown();

        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }

        TR_Hand.gameObject.SetActive(true);

        TR_Hand.DOKill();
        SR_Hand.DOKill();
        SR_Selection.DOKill();
        if (_s != null)
        {
            _s.Kill();
            _s = null;
        }

        float timeMove1 = 0.2f;

        TR_HandInner.localPosition = new Vector3(0.7f, 0.0f, 0.0f);
        TR_Hand.position = from;
        SR_Hand.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        SR_Hand.DOFade(1.0f, 0.15f);
        //TR_HandInner.DOLocalMoveX(5.0f, timeMove).SetLoops(-1, LoopType.Yoyo);

        TR_Selection.position = from;
        Color c = SR_Selection.color;
        c.a = 0.0f;
        SR_Selection.gameObject.SetActive(true);
        //Common.DebugLog("activate");
        SR_Selection.color = c;
        SR_Selection.DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        _s = DOTween.Sequence();
        _s.Append(TR_HandInner.DOLocalMoveX(0.0f, timeMove1));
        _s.AppendInterval(0.5f);
        _s.Append(TR_HandInner.DOLocalMoveX(0.7f, timeMove1 * 1.5f));
        _s.AppendInterval(0.1f);
        _s.SetLoops(4);
        _s.OnComplete(() =>
        {
            onComplete();
            Playing();
            ContinueHelperHandCountDown();
        });
        _s.SetId(transform);
    }

    public void Kill()
    {
        _done = true;
        Stop();
        timeWaitValue = -1.0f;
    }

    public void Stop()
    {
        if (!TR_Hand.gameObject.activeSelf)
        {
            return;
        }
        TR_Hand.DOKill();
        SR_Hand.DOKill();
        SR_Selection.DOKill();
        TR_HandInner.DOKill();
        if (_s != null)
        {
            _s.Kill();
            _s = null;
        }

        //SR_Hand.DOFade(0.0f, 0.1f);
        //SR_Selection.DOFade(1.0f, 0.1f);

        TR_Hand.gameObject.SetActive(false);
        SR_Selection.gameObject.SetActive(false);
    }

    public void Playing()
    {/*
        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }
        */
        _isInAction = false;
        timeWaitValue = totalTimeToWait[indexTime];
        Stop();
    }

    public void Trigger_Help()
    {
        timeWaitValue = 0.0f;
        //ManagerAnalytics.Analytics_SimpleEvent("HelpIsOn");
        CallBackHelpNeeded?.Invoke();
    }

    public void Upgrade()
    {
        indexTime++;
        if (totalTimeToWait.Length <= indexTime)
        {
            indexTime = totalTimeToWait.Length - 1;
        }
        timeWaitValue = totalTimeToWait[indexTime];
        Stop();
    }

    private void Update()
    {
        if (_done || _isInAction)
        {
            return;
        }

        if (0.0f < timeWaitValue)
        {
            timeWaitValue -= Time.deltaTime;
            if (timeWaitValue <= 0.0f)
            {
                timeWaitValue = 0.0f;
                CallBackHelpNeeded?.Invoke();
            }
        }
    }



    public void EnterFrame()
    {
        if (_done)
        {
            return;
        }

        if (0.0f < timeWaitValue)
        {
            timeWaitValue -= Time.deltaTime;
            if (timeWaitValue <= 0.0f)
            {
                timeWaitValue = 0.0f;
                CallBackHelpNeeded?.Invoke();
            }
        }
    }

    public void SetLayerSelection(int index, string layer)
    {
        SR_Selection.sortingLayerName = layer;
        SR_Selection.sortingOrder = index;
    }

    public bool IsInAction
    {
        get
        {
            return _isInAction;
        }
    }

    private bool _isInAction;
    public void StopHelperHandCountDown()
    {
        _isInAction = true;
    }

    public void ContinueHelperHandCountDown()
    {
        _isInAction = false;
    }
}