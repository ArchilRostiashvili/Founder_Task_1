using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FloatingItem : MonoBehaviour
{

    [SerializeField]
    private Transform _visual;
    [SerializeField]
    [Tooltip("This can be null if you want to float this item relative to itself")]
    private Transform _localPivotTrans;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float xOffset;
    [SerializeField]
    [Range(0.5f, 5)]
    [Tooltip("Time needed for one animation for X position")]
    private float timingX;
    [SerializeField]
    [Range(0.0f, 1f)]
    [Tooltip("Amount of distance from startPosition at the end of animation for Y")]
    private float yOffset;
    [SerializeField]
    [Range(0.5f, 5)]
    [Tooltip("Time needed for one animation for Y position")]
    private float timingY;
    [SerializeField]
    private Ease _xAxisEase = Ease.InOutQuad;
    [SerializeField]
    private Ease _yAxisEase = Ease.Linear;

    private Vector3 _localPivot;
    private float _observerXoffset, _observerYoffset, _observerXtiming, _observerYtiming;
    private bool _canUseOnvalidate;
    private Ease _observerXEase, _observerYEase;

    public void Init()
    {
        if (_xAxisEase == Ease.Unset)
        {
            _xAxisEase = Ease.InOutQuad;
        }
        if (_yAxisEase == Ease.Unset)
        {
            _yAxisEase = Ease.Linear;
        }

        if (_localPivotTrans == null)
        {
            _localPivot = _visual.localPosition;
        }
        else
        {
            _localPivot = _localPivotTrans.localPosition;
        }

        this.SetObserverValues();

    }

    void OnValidate()
    {
        if (_canUseOnvalidate)
        {
            this.SetObserverValues();

            _visual.DOKill();
            _visual.DOLocalMove(_localPivot, 0.5f).
            OnComplete(() =>
            {
                this.StartFloatingInTheAir(_observerXoffset, _observerXtiming, _observerYoffset, _observerYtiming, _observerXEase, _observerYEase);
            });
        }
    }

    public void StartFloatingInTheAir()
    {
        int signX = Random.Range(0, 2) == 0 ? 1 : -1;//50% chance
        int signY = Random.Range(0, 2) == 0 ? 1 : -1;//50% chance

        if (_localPivotTrans != null && _localPivotTrans != _visual)
        {
            this.transform.SetParent(_localPivotTrans);
        }

        _visual.DOLocalMoveX(_localPivot.x - signX * xOffset, timingX / 2).OnComplete(() =>
        {
            _visual.DOLocalMoveX(_localPivot.x + signX * xOffset, timingX).SetEase(_xAxisEase).SetLoops(-1, LoopType.Yoyo);
        });
        _visual.DOLocalMoveY(_localPivot.y + signY * yOffset, timingY / 2).OnComplete(() =>
        {
            _visual.DOLocalMoveY(_localPivot.y - signY * yOffset, timingY).SetEase(_yAxisEase).SetLoops(-1, LoopType.Yoyo);
            _canUseOnvalidate = true;
        });

    }

    private void StartFloatingInTheAir(float xoffset, float xtiming, float yoffset, float ytiming, Ease xease, Ease yease)
    {
        int signX = Random.Range(0, 2) == 0 ? 1 : -1;//50% chance
        int signY = Random.Range(0, 2) == 0 ? 1 : -1;//50% chance

        if (_localPivotTrans != null && _localPivotTrans != _visual)
        {
            this.transform.SetParent(_localPivotTrans);
        }

        _visual.DOLocalMoveX(_localPivot.x - signX * xoffset, xtiming / 2).OnComplete(() =>
        {
            _visual.DOLocalMoveX(_localPivot.x + signX * xoffset, xtiming).SetEase(xease).SetLoops(-1, LoopType.Yoyo);
        });
        _visual.DOLocalMoveY(_localPivot.y + signY * yoffset, ytiming / 2).OnComplete(() =>
        {
            _visual.DOLocalMoveY(_localPivot.y - signY * yoffset, ytiming).SetEase(yease).SetLoops(-1, LoopType.Yoyo);
        });

    }

    public void StopFloatingInTheAir(bool moveToPos = true)
    {
        _visual.DOKill();
        if (moveToPos)
            _visual.DOLocalMove(_localPivot, 0.5f);
    }

    private void SetObserverValues()
    {
        _observerXoffset = xOffset;
        _observerXtiming = timingX;
        _observerYoffset = yOffset;
        _observerYtiming = timingY;

        _observerXEase = _xAxisEase;
        _observerYEase = _yAxisEase;
    }

}