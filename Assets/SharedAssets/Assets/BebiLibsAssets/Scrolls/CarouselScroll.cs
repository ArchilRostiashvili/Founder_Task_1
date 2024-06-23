using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BebiLibs;
using System.Linq;
using TMPro;
using I2.Loc;

public class CarouselScroll : UIBehaviour
{
    //[SerializeField] bool _autoInitialize = false;
    private System.Action<CarouselScroll> _scrollLayoutUpdateEvent;

    [SerializeField] internal RectTransform _carouselViewportRT;
    [SerializeField] internal RectTransform _carouselScrollContentRT;

    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private ContentSizeFitter _contentSizeFitter;

    //[SerializeField] private string _scrollSound = "fx_scroll";

    [HideInInspector] internal List<ScrollElement> _scrollElementList = new List<ScrollElement>();
    [HideInInspector] internal ScrollRect _scrollRect;

    [SerializeField] private ScrollElement _elementToMove;
    private ScrollElement _currentTargetElement;

    [Header("Extra Behaviors")]
    [SerializeField] private List<CarouselScrollBehaviour> _arrayScrollExtensions;

    private bool _isInitialized = false;

    private LayoutState _layoutState = LayoutState.NONE;

    private Rect _viewPortRect;
    public Rect viewRect => _viewPortRect;

    public bool RecalculationLayoutOnEnable = true;

    public void Init()
    {
        if (_isInitialized) return;
        _scrollRect = GetComponent<ScrollRectFast>();
        ScrollElement[] scrollElements = _carouselScrollContentRT.GetComponentsInChildren<ScrollElement>();

        _scrollElementList.Clear();
        for (int i = 0; i < scrollElements.Length; i++)
        {
            _scrollElementList.Add(scrollElements[i]);
            scrollElements[i].Init();
        }

        _elementToMove = _scrollElementList[0];
        _currentTargetElement = _elementToMove;
        _arrayScrollExtensions.ForEach(x => x.Initialize(this));

        _isInitialized = true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RecalculateLayout();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _layoutState = LayoutState.NONE;
        StopAllCoroutines();
        RecalculateLayout();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && RecalculationLayoutOnEnable)
        {
            ForceRecalculateLayout();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _scrollRect.velocity = Vector2.zero;
    }

    public void MoveTo(ScrollElement scrollElement)
    {
        _elementToMove = scrollElement;
        _currentTargetElement = scrollElement;

        if (_layoutState != LayoutState.UPDATED)
        {
            Debug.LogWarning($"Unable To Perform MoveTo {scrollElement}, Layout is Still Updating. Add Listener To LayoutStateUpdateEvent");
            return;
        }


        _scrollRect.enabled = false;
        _scrollRect.velocity = Vector2.zero;

        Vector3 scrollPosition = _carouselScrollContentRT.transform.position;
        if (_currentTargetElement == null)
        {
            _scrollRect.enabled = true;
            _scrollRect.velocity = Vector2.zero;
            return;
        }

        if (_currentTargetElement.CanCenterElement || _currentTargetElement.WorldSize.x < _viewPortRect.size.x)
        {
            float xPos = _currentTargetElement.position.x - _viewPortRect.center.x;
            _carouselScrollContentRT.transform.position = scrollPosition + new Vector3(-xPos, 0, 0);
        }
        else
        {
            float xPos = _currentTargetElement.position.x - _viewPortRect.center.x;
            xPos += _viewPortRect.size.x / 2 - _currentTargetElement.WorldSize.x / 2.0f;
            _carouselScrollContentRT.transform.position = scrollPosition + new Vector3(-xPos, 0, 0);
        }

        _scrollRect.enabled = true;
        _scrollRect.velocity = Vector2.zero;

        if (_currentTargetElement != null)
        {
            _arrayScrollExtensions.ForEach(x => x.OnScrollChange(_currentTargetElement));
        }
    }

    public void AddLayoutStateUpdateEvent(System.Action<CarouselScroll> layoutStateUpdateEvent, bool immediate = false)
    {
        _scrollLayoutUpdateEvent = layoutStateUpdateEvent;
        if (immediate && _layoutState == LayoutState.UPDATED)
        {
            layoutStateUpdateEvent?.Invoke(this);
        }
    }

    public void RemoveLayoutStateUpdateEventListeners()
    {
        _scrollLayoutUpdateEvent = null;
    }

    public void ForceRecalculateLayout()
    {
        StopAllCoroutines();
        _layoutState = LayoutState.NONE;
        RecalculateLayout();
    }

    private void RecalculateLayout()
    {
        if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
        {
            Debug.LogError("GameObject Should be enabled to run this method");
            return;
        }

        if (_layoutState == LayoutState.UPDATED)
        {
            _scrollLayoutUpdateEvent?.Invoke(this);
        }

        if (_layoutState == LayoutState.UPDATED || _layoutState == LayoutState.UPDATING || !_isInitialized) return;
        _layoutState = LayoutState.UPDATING;


        _scrollRect.velocity = Vector2.zero;
        StartCoroutine(UpdatePos());

        IEnumerator UpdatePos()
        {
            yield return WaitForFrame(2);

            SetMainLayoutSystemActive(true);

            yield return WaitForFrame(1);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_carouselScrollContentRT);
            LayoutRebuilder.MarkLayoutForRebuild(_carouselScrollContentRT);

            yield return WaitForFrame(3);

            SetMainLayoutSystemActive(false);

            Vector3 sizeDelta = _carouselScrollContentRT.sizeDelta;
            Rect rect = Utils.GetWorldRect(_carouselViewportRT);

            _viewPortRect = rect;

            _scrollElementList.ForEach(x => x.UpdatePositions());

            for (int i = 0; i < _scrollElementList.Count; i++)
            {
                ScrollElement scrollElement = _scrollElementList[i];
                scrollElement.LeftSpawnPosition = _viewPortRect.min.x - scrollElement.WorldSize.x / 2;
                float distance = GetDistance(i) + scrollElement.WorldSize.x / 2;
                scrollElement.RightSpawnPosition = distance + _viewPortRect.min.x;
            }

            _carouselScrollContentRT.sizeDelta = sizeDelta;
            _layoutState = LayoutState.UPDATED;

            _arrayScrollExtensions.ForEach(x => x.OnFinishScrollUpdate(this));
            _scrollLayoutUpdateEvent?.Invoke(this);
        }
    }

    private IEnumerator WaitForFrame(int count)
    {
        for (int i = 0; i < count; i++)
            yield return new WaitForEndOfFrame();
    }

    public void SetMainLayoutSystemActive(bool value)
    {
        if (_horizontalLayoutGroup != null)
        {
            _horizontalLayoutGroup.enabled = value;
        }
        if (_contentSizeFitter != null)
        {
            _contentSizeFitter.enabled = value;
        }
    }

    public float GetDistance(int startIndex)
    {
        float distance = 0;
        for (int i = startIndex; i < _scrollElementList.Count + startIndex - 1; i++)
        {
            int nextIndex = (i + 1) % _scrollElementList.Count;
            distance += _scrollElementList[nextIndex].WorldSize.x;
        }
        return distance;
    }

    public ScrollElement FindNextSnapElement()
    {
        float distanceMin = float.MaxValue;
        ScrollElement scrollMin = null;

        for (int i = 0; i < _scrollElementList.Count; i++)
        {
            ScrollElement element = _scrollElementList[i];
            float xPosMin = element.position.x;

            if (Mathf.Abs(xPosMin) < distanceMin)
            {
                distanceMin = Mathf.Abs(xPosMin);
                scrollMin = element;
            }
        }
        return scrollMin;
    }


    private void UpdateScrollEvents()
    {
        _elementToMove = FindNextSnapElement();
        if (_currentTargetElement != _elementToMove)
        {
            foreach (var extension in _arrayScrollExtensions)
            {
                extension.OnScrollChange(_elementToMove);
            }
            _currentTargetElement = _elementToMove;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _scrollElementList.Count; i++)
        {
            if (_scrollElementList[i] != null)
            {
                GameObject.Destroy(_scrollElementList[i].gameObject);
            }
        }

        _scrollElementList.Clear();


        foreach (var item in _arrayScrollExtensions)
        {
            item.Clear();
        }

    }


    private void Update()
    {
        if (_layoutState != LayoutState.UPDATED || !_isInitialized) return;

        for (int i = 0; i < _arrayScrollExtensions.Count; i++)
        {
            _arrayScrollExtensions[i].UpdateScroll(this);
        }

        if (Mathf.Abs(_scrollRect.velocity.x) > 5f)
        {
            UpdateScrollEvents();
        }

        for (int i = 0; i < _scrollElementList.Count; i++)
        {
            ScrollElement se = _scrollElementList[i];
            Vector2 newRectPos = se.position;

            if (newRectPos.x > se.RightSpawnPosition)
            {
                float offset = newRectPos.x - se.RightSpawnPosition;
                se.position = new Vector2(se.LeftSpawnPosition + offset, newRectPos.y);
            }

            if (newRectPos.x < se.LeftSpawnPosition)
            {
                float offset = se.LeftSpawnPosition - newRectPos.x;
                se.position = new Vector2(se.RightSpawnPosition - offset, newRectPos.y);
            }
        }
    }


    public enum LayoutState
    {
        NONE, UPDATING, UPDATED
    }
}
