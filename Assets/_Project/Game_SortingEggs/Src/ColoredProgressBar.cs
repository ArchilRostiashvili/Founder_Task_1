using BebiLibs;
using DG.Tweening;
using FarmLife;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColoredProgressBar : ProgressBarBase
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField][HideField("_hasSpriteSizer", true)] private ItemSpriteSizer _itemSpriteSizer;
    [SerializeField] private TextMeshProUGUI _tmpro;
    [SerializeField] private bool _hasSpriteSizer = true;
    [SerializeField] private bool _fillWithAnimation;
    [SerializeField][HideField("_hasSpriteSizer", false)] Image _contentImage;
    [SerializeField] private ItemSpriteSizer _bubbleSpriteSizer = null;
    private float _partCounter;
    private float _targetValue;

    private int _correctCounter;

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
            if(_bubbleSpriteSizer != null)
                _bubbleSpriteSizer.SetSprite(additinalSprite);
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

    public void ProgressUpWithoutAnimation(float increment)
    {
        _slider.value += increment;
    }

    public override void Reset()
    {
        _slider.DOKill();
        _targetValue = 0;
        _slider.value = 0;
        _correctCounter = 0;

        if (_tmpro != null)
        {
            _tmpro.text = _correctCounter.ToString();
        }
    }

    public override bool IsFilled()
    => _slider.value >= 0.99f;
}
