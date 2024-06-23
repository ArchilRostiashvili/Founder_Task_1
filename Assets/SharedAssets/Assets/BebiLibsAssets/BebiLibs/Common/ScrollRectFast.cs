using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectFast : ScrollRect, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool _isDragging = false;

    private int _currentDragPointer;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDragging) return;

        _isDragging = true;
        _currentDragPointer = eventData.pointerId;
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != _currentDragPointer)
        {
            return;
        }
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != _currentDragPointer)
        {
            return;
        }
        _isDragging = false;
        base.OnEndDrag(eventData);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _isDragging = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _isDragging = false;
    }
}
