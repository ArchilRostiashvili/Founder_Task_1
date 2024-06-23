using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ItemInteractable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public UnityEvent<ItemInteractable> onDragStart;
    public UnityEvent<ItemInteractable> onDrag;
    public UnityEvent<ItemInteractable> onDragEnd;

    [SerializeField] private Collider2D _collider;
    protected Vector2 _offset;
    private Vector3 _startingPosition;

    public bool IsDragEnabled => _isDragEnabled;
    public Collider2D GetCollider => _collider;
    public Vector3 GetStartingPosition() => _startingPosition;

    private bool _hasDragStarted;
    protected bool _isDragEnabled = true;

    public bool HasDragStarted => _hasDragStarted;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        this.StartCoroutine(this.GetPosition());
    }

    IEnumerator GetPosition()
    {
        yield return new WaitForSeconds(0.01f);
        _startingPosition = this.transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDragEnabled) return;

        _hasDragStarted = true;

        this.transform.DOKill();
        _offset = this.transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
        this.onDragStart?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragEnabled || !_hasDragStarted) return;

        this.onDrag?.Invoke(this);
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) + _offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragEnabled || !_hasDragStarted) return;

        _hasDragStarted = false;

        this.onDragEnd?.Invoke(this);
    }

    public void SetDragEnabled(bool value)
    {
        _isDragEnabled = value;
    }

    public void SetStartingPosition(Vector3 position)
    {
        _startingPosition = position;
    }
    public void ResizeOverTime(float value, float duration)
    {
        transform.DOScale(value, duration);
    }

    public void MoveToLocationWithJump(Vector3 location, float jumpHeight, float duration, bool isLocal = true, System.Action onComplete = null, System.Action onJumpStart = null)
    {
        if (isLocal)
            this.transform.DOLocalMove(location + Vector3.up * jumpHeight, duration / 2).OnComplete(() =>
              {
                  onJumpStart?.Invoke();

                  this.transform.DOLocalMove(location, duration / 2).OnComplete(() =>
                  {
                      onComplete?.Invoke();
                  });
              });
        else
            this.transform.DOMove(location + Vector3.up * jumpHeight, duration / 2).OnComplete(() =>
              {
                  onJumpStart?.Invoke();

                  this.transform.DOMove(location, duration / 2).OnComplete(() =>
                  {
                      onComplete?.Invoke();
                  });
              });
    }

    public void MoveToLocation(Vector3 location, float duration, bool isLocal = true, System.Action onComplete = null)
    {
        if (isLocal)
            this.transform.DOLocalMove(location, duration).OnComplete(() => { onComplete?.Invoke(); });
        else
            this.transform.DOMove(location, duration).OnComplete(() => { onComplete?.Invoke(); });
    }

    public bool IsAtStartingPosition(bool isLocal = true) => isLocal ? this.transform.localPosition == _startingPosition : this.transform.position == _startingPosition;

    public void ResetPosition(float duration, bool isLocal = true, System.Action onComplete = null, bool anim = true)
    {
        this.transform.DOKill();

        if (isLocal)
            this.transform.DOLocalMove(_startingPosition, duration).OnComplete(() => { onComplete?.Invoke(); });
        else
            this.transform.DOMove(_startingPosition, duration).OnComplete(() => { onComplete?.Invoke(); });
    }
}
