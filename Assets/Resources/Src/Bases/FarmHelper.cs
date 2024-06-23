using BebiLibs;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmHelper : MonoBehaviour
{
    public System.Action HelpNeededEvent;

    [SerializeField] private GameObject _handGO;
    [SerializeField] private SpriteRenderer _handRenderer;
    [SerializeField] private float maxTimer = 8f;
    [SerializeField] private List<Transform> _positionsList;
    [SerializeField] private GameObject _glowGO;

    private float _currentTimer;
    private bool _helperCountdownDisabled;

    private bool _isActive;
    private Sequence _sequence;

    private Vector2 _glowScale;

    public void Init()
    {
        _glowScale = _glowGO.transform.localScale;
        _currentTimer = maxTimer;
    }

    private void Update()
    {
        if (!_isActive || _helperCountdownDisabled)
            return;

        _currentTimer -= Time.deltaTime;

        if (_currentTimer <= 0)
        {
            _helperCountdownDisabled = true;
            HelpNeededEvent?.Invoke();
        }
    }

    public void DisableHelper()
    {
        StopAllCoroutines();
        _handGO.transform.DOKill();
        _helperCountdownDisabled = true;
        HideHelper();
    }

    public void Reset()
    {
        StopAllCoroutines();

        _handGO.transform.parent = transform;
        _glowGO.transform.parent = transform;
        _currentTimer = maxTimer;
        _helperCountdownDisabled = false;
        HideHelper();
    }

    public void HideHelper()
    {
        _handGO.transform.DOKill();
        _handGO.SetActive(false);
        _glowGO.SetActive(false);
    }

    public void ShowTapHelper(Vector3 tapPosition)
    {
        if (!_isActive) return;

        _handGO.SetActive(true);
        DisableSequence();

        float timeMove1 = 0.2f;

        _handGO.transform.position = tapPosition;
        _glowGO.transform.position = tapPosition;
        _handRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _handRenderer.DOFade(1.0f, 0.1f);
        _glowGO.gameObject.SetActive(true);

        _sequence = DOTween.Sequence();

        float scale = _handGO.transform.localScale.y;

        _sequence.Append(_handGO.transform.DOScale(scale * 1.15f, timeMove1));
        _sequence.AppendInterval(0.5f);
        _sequence.Append(_handGO.transform.DOScale(scale, timeMove1 * 1.5f));
        _sequence.AppendInterval(0.1f);
        _sequence.SetLoops(4);
        _sequence.OnComplete(() =>
        {
            Reset();
        });
        _sequence.SetId(transform);
    }

    public void ShowTapHelper(Transform parent, bool disableAfterShow = false)
    {
        if (!_isActive) return;
        _handGO.SetActive(true);

        DisableSequence();

        float timeMove1 = 0.2f;
        float scale = _handGO.transform.localScale.y;

        _handGO.transform.parent = parent;
        _glowGO.transform.parent = parent;
        _handGO.transform.localPosition = Vector2.zero;
        _glowGO.transform.localPosition = Vector2.zero;
        // _glowGO.transform.localScale = Vector2.zero;
        _glowGO.transform.localScale = _glowScale;
        _handRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _handRenderer.DOFade(1.0f, 0.1f);
        _glowGO.gameObject.SetActive(true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_handGO.transform.DOScale(scale * 1.15f, timeMove1));
        _sequence.AppendInterval(0.5f);
        _sequence.Append(_handGO.transform.DOScale(scale, timeMove1 * 1.5f));
        _sequence.AppendInterval(0.1f);
        _sequence.SetLoops(4);
        _sequence.OnComplete(() =>
        {
            _handGO.transform.parent = this.transform;
            _handGO.transform.localScale = Vector2.one * scale;
            _glowGO.transform.localScale = _glowScale;
            Reset();
        });
        _sequence.SetId(transform);
    }

    public void ShowDragHelper(Vector3 from, Vector3 to, Ease ease = Ease.InOutCubic, System.Action AfterHelperEvent = null, float time = 2f)
    {
        if (!_isActive) return;

        DisableSequence();

        _handGO.transform.position = from;
        _handGO.SetActive(true);
        _handRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _handRenderer.DOFade(1.0f, 0.15f);

        _glowGO.transform.position = from;
        _glowGO.SetActive(true);

        _handGO.transform.DOMove(to, time * 0.7f).SetEase(ease).SetLoops(2).OnComplete(() =>
          {
              AfterHelperEvent?.Invoke();
              Reset();
          });
    }

    public void ShowDragHelper(Transform from, Vector3 to, Ease ease = Ease.InOutCubic, System.Action AfterHelperEvent = null, float time = 2f)
    {
        if (!_isActive) return;

        DisableSequence();

        _handGO.transform.parent = from;
        _handGO.transform.localPosition = Vector2.zero;
        _handRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _handRenderer.DOFade(1.0f, 0.15f);

        _glowGO.transform.parent = from;
        _glowGO.transform.localPosition = Vector2.zero;
        _glowGO.SetActive(true);
        _handGO.SetActive(true);
        // float timeLeft = 0.1f;
        _glowGO.transform.localScale = Vector2.one;
        _handGO.transform.DOMove(to, 0.8f).OnComplete(() =>
        {
            _handGO.SetActive(false);
            _handGO.transform.localPosition = Vector2.zero;
            ManagerTime.Delay(0.2f, () =>
            {
                _handGO.SetActive(true);
                _handGO.transform.DOMove(to, 0.8f).OnComplete(() =>
                {
                    AfterHelperEvent?.Invoke();
                    Reset();
                });
            });
        });
    }

    private void DisableSequence()
    {
        _handGO.transform.DOKill();
        _handRenderer.DOKill();

        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }
    }

    public void SetIsActive(bool value)
    {
        _isActive = value;
    }

    public void ShowZigzagHelper(Transform from = null, Transform to = null, System.Action afterHelpAction = null)
    {
        Vector2 fromPosition = from == null ? _positionsList[0].position : from.position;
        _handGO.transform.position = fromPosition;
        _glowGO.transform.position = fromPosition;

        if (_handRenderer != null)
        {
            _handRenderer.SetAlpha(1f);
        }

        _handGO.SetActive(true);
        _glowGO.SetActive(true);

        _sequence = DOTween.Sequence();

        if (to != null)
        {
            _sequence.AppendInterval(0.2f);
            _sequence.Append(_handGO.transform.DOMove(to.position, 0.4f));
            _sequence.AppendInterval(0.2f);
        }

        for (int i = 0; i < _positionsList.Count; i++)
        {
            _sequence.AppendInterval(0.2f);
            _sequence.Append(_handGO.transform.DOMove(_positionsList[i].position, 0.4f));
            _sequence.AppendInterval(0.2f);
        }

        _sequence.SetLoops(2, LoopType.Restart).OnComplete(() =>
        {
            if (_handRenderer != null)
            {
                _handRenderer.DOFade(0f, 0.3f);
            }

            Reset();
        });
        _sequence.AppendInterval(0.2f);
    }

    public void ShowZigzagHelper(Vector2 from, Transform to = null, System.Action afterHelpAction = null)
    {
        _handGO.transform.position = from;

        if (_handRenderer != null)
        {
            _handRenderer.SetAlpha(1f);
        }

        _handGO.SetActive(true);

        _sequence = DOTween.Sequence();

        if (to != null)
        {
            _sequence.AppendInterval(0.2f);
            _sequence.Append(_handGO.transform.DOMove(to.position, 0.4f));
            _sequence.AppendInterval(0.2f);
        }
        _sequence.AppendInterval(0.2f);
        _sequence.Append(_handGO.transform.DOMove(to.position + new Vector3(0.7f, 0.45f), 0.4f));
        _sequence.AppendInterval(0.2f);
        _sequence.Append(_handGO.transform.DOMove(to.position + new Vector3(-1.2f, -0.4f), 0.4f));
        _sequence.AppendInterval(0.2f);
        _sequence.Append(_handGO.transform.DOMove(to.position + new Vector3(0.5f, 0.2f), 0.4f));
        _sequence.AppendInterval(0.2f);
        _sequence.AppendInterval(0.2f);
        _sequence.Append(_handGO.transform.DOMove(to.position, 0.4f));
        _sequence.AppendInterval(0.2f);

        _sequence.SetLoops(2, LoopType.Restart).OnComplete(() =>
        {
            if (_handRenderer != null)
            {
                _handRenderer.DOFade(0f, 0.3f);
            }

            Reset();
        });
        _sequence.AppendInterval(0.2f);
    }
}
