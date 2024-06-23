using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using DG.Tweening;
using FarmLife;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using System;

public class ForwardProgressBar : ProgressBarBase
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField][HideField("_hasSpriteSizer", true)] private ItemSpriteSizer _itemSpriteSizer;
    [SerializeField] private TextMeshProUGUI _tmpro;
    [SerializeField] private bool _hasSpriteSizer = true;
    [SerializeField] private bool _fillWithAnimation;
    [SerializeField][HideField("_hasSpriteSizer", false)] Image _contentImage;
    private float _partCounter;
    private float _targetValue;
    private int _correctCounter;
    private Vector2 _anchoredPosition;

    public override void SetData(float partCount, Color color, Sprite additinalSprite = null)
    {
        _partCounter = partCount;

        if (color != Color.white)
        {
            _fillImage.color = color;
        }

        if (_itemSpriteSizer != null)
        {
            _itemSpriteSizer.gameObject.SetActive(true);
        }
        else if (_contentImage != null)
        {
            _contentImage.sprite = additinalSprite;
        }
    }

    public override void SetData(float partCount)
    {
        _partCounter = partCount;
    }

    public override void SetData(Color color, Sprite additionalSprite = null)
    {
        if (color != Color.white)
        {
            _fillImage.color = color;
        }

        if (_itemSpriteSizer != null)
        {
            if (additionalSprite != null)
            {
                _itemSpriteSizer.SetSprite(additionalSprite);
                _itemSpriteSizer.gameObject.SetActive(true);
            }
        }
    }

    public override void ProgressUp()
    {
        _slider.value += 1f / _partCounter;
        float length = Mathf.Abs(_anchoredPosition.x / _partCounter);

        _fillImage.rectTransform.anchoredPosition += new Vector2(length, 0);

        if (_tmpro != null)
        {
            _correctCounter++;
            _tmpro.text = _correctCounter.ToString();
        }
    }

    public override void ProgressUp(float increment)
    {
        if (_fillWithAnimation)
        {
            _targetValue += increment;
            DOTween.To(() => _slider.value, x => _slider.value = x, _targetValue, 0.2f);
        }

        else
        {
            _slider.value += increment;
        }
    }

    public override void Reset()
    {
        _targetValue = 0;
        _slider.value = 0;
        _fillImage.rectTransform.anchoredPosition = _anchoredPosition;
    }

    public override bool IsFilled()
    => _slider.value >= 0.99f;

    public void SetPosition()
    {
        _anchoredPosition = _fillImage.rectTransform.anchoredPosition;
    }
}
