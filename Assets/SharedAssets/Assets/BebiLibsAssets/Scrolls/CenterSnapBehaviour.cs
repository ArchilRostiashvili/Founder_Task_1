using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CenterSnapBehaviour : CarouselScrollBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public bool EnableSnapping = true;
    [SerializeField] private float _dragMaxDelta = 50;

    private Vector3 _startScroll = new Vector3(float.MaxValue, float.MaxValue);
    private Vector3 _defaultScrollPosition;
    private ScrollRect _scrollRect;
    private RectTransform _scrollContentRT;

    private List<ScrollElement> _scrollElementList = new List<ScrollElement>();
    private ScrollElement _autoScrollTarget;
    private float _scrollPositionX = 0;
    private bool _isMovingToTarget = false;

    private CarouselScroll _carouselScroll;

    private List<IScrollChangeListener> _scrollChangeListenerList = new List<IScrollChangeListener>();

    private float _safeOffset;
    private Vector3 _targetScrollPosition;

    public override void Initialize(CarouselScroll scroll)
    {
        _scrollChangeListenerList.Clear();
        _scrollRect = scroll._scrollRect;
        _scrollContentRT = scroll._carouselScrollContentRT;
        _scrollElementList = scroll._scrollElementList;
        _carouselScroll = scroll;

        _autoScrollTarget = _scrollElementList[0];
        Camera camera = Camera.main;
        _startScroll = _scrollContentRT.position;

        _safeOffset = (camera.ScreenToWorldPoint(Screen.safeArea.min) - camera.ViewportToWorldPoint(Vector3.zero)).x;
        //Debug.Log("Safe Offset: " + _safeOffset);
    }

    private void OnEnable()
    {
        _defaultScrollPosition = _scrollContentRT.position;
    }

    private void OnDisable()
    {
        if (_scrollContentRT == null) return;
        _scrollContentRT.transform.DOKill();
        _scrollContentRT.position = _defaultScrollPosition;
        _autoScrollTarget = _scrollElementList[0];
        // Vector3 scrollPosition = _scrollContentRT.transform.position;
        // float offset = _scrollPositionX - _autoScrollTarget.position.x;
        // offset += _safeOffset / 2;
        // _targetScrollPosition = new Vector3(scrollPosition.x + offset, scrollPosition.y, scrollPosition.z);
        // _scrollContentRT.transform.position = _targetScrollPosition;

        _isMovingToTarget = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_scrollContentRT != null)
        {
            _startScroll = _scrollContentRT.position;
            _isMovingToTarget = true;
            _scrollContentRT.transform.DOKill();
        }
        else
        {
            Debug.LogError("Scroll Content is null, You should initialize CenterSnapBehaviour first", this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SnapClosestElement();
    }

    public void SnapClosestElement()
    {
        if (!EnableSnapping) return;
        if (_carouselScroll == null || _autoScrollTarget == null) return;

        float delta = _scrollContentRT.position.x - _startScroll.x;

        if (Mathf.Abs(delta) > _dragMaxDelta)
        {
            _autoScrollTarget = delta < 0 ? FindCloseItem(false) : FindCloseItem(true);
        }

        Vector3 scrollPosition = _scrollContentRT.transform.position;
        float offset = _scrollPositionX - _autoScrollTarget.position.x;

        _scrollRect.velocity = Vector2.zero;
        _isMovingToTarget = true;

        float speed = 0.01579f * Mathf.Abs(offset) + 0.1842f;

        offset += _safeOffset / 2;
        _targetScrollPosition = new Vector3(scrollPosition.x + offset, scrollPosition.y, scrollPosition.z);
        _scrollContentRT.transform.DOMove(_targetScrollPosition, speed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _isMovingToTarget = false;
        });

        _scrollChangeListenerList.ForEach(x => x.OnScrollChange(_autoScrollTarget.PaginationID));
    }

    private ScrollElement FindCloseItem()
    {
        float distanceMin = float.MaxValue;
        ScrollElement scrollMin = null;

        for (int i = 0; i < _scrollElementList.Count; i++)
        {
            ScrollElement element = _scrollElementList[i];

            //float xPosMin = (element.position.x - element.worldSize.x / 2) - _scrollPositionX;
            float xPosMin = element.position.x - _scrollPositionX;
            if (Mathf.Abs(xPosMin) < distanceMin)
            {
                distanceMin = Mathf.Abs(xPosMin);
                scrollMin = element;
            }
        }
        return scrollMin;
    }


    private ScrollElement FindCloseItem(bool left)
    {
        float distanceMin = float.MaxValue;
        ScrollElement scrollMin = null;

        for (int i = 0; i < _scrollElementList.Count; i++)
        {
            ScrollElement element = _scrollElementList[i];
            int direction = left ? 1 : -1;
            float xPosMin = element.position.x + direction * element.WorldSize.x / 2;
            if (Mathf.Abs(xPosMin) < distanceMin)
            {
                distanceMin = Mathf.Abs(xPosMin);
                scrollMin = element;
            }
        }
        return scrollMin;
    }


    private ScrollElement GetNextElement()
    {
        int index = _scrollElementList.IndexOf(_autoScrollTarget);
        return _scrollElementList[(index + 1) % _scrollElementList.Count];
    }

    private ScrollElement GetPreviousElement()
    {
        int index = _scrollElementList.IndexOf(_autoScrollTarget);
        return _scrollElementList[Utils.Mod(index - 1, _scrollElementList.Count)];
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Just For OnBeginDrag And OnEndDrag To work
    }

    public override void Clear()
    {
        foreach (var item in _scrollElementList)
        {
            if (item != null)
            {
                GameObject.Destroy(item);
            }
        }
        _scrollElementList.Clear();
    }

    public void AddScrollChangeListener(IScrollChangeListener listener)
    {
        if (!_scrollChangeListenerList.Contains(listener)) _scrollChangeListenerList.Add(listener);
    }

    public void RemoveScrollChangeListener(IScrollChangeListener listener)
    {
        int index = _scrollChangeListenerList.IndexOf(listener);
        if (index >= 0) _scrollChangeListenerList.RemoveAt(index);
    }


    public override void UpdateScroll(CarouselScroll scroll)
    {
        if (!_isMovingToTarget)
        {
            _autoScrollTarget = FindCloseItem();
        }
    }

    public override void OnScrollChange(ScrollElement newElement)
    {

    }

    public override void OnFinishScrollUpdate(CarouselScroll scroll)
    {
        _startScroll = _scrollContentRT.position;
        SnapClosestElement();
    }
}
