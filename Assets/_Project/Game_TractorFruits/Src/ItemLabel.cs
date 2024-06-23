using System.Collections;
using System.Collections.Generic;
using FarmLife;
using UnityEngine;

public class ItemLabel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private FeelAnimator _animator;

    public void SetData(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Show()
    {
        _animator.Play(AnimationNamesData.ANIM_SHOW);
    }

    public void Hide()
    {
        _animator.Play(AnimationNamesData.ANIM_HIDE);
    }
}
