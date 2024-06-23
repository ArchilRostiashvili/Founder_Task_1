using System.Collections;
using System.Collections.Generic;
using FarmLife;
using UnityEngine;

public class DefaultProgressBar : ProgressBarBase
{
    [SerializeField] private List<SpriteRenderer> _spriteRenderersList = new List<SpriteRenderer>();

    private int _counter;

    public override void SetData(float index, Color color, Sprite additionalSprite = null)
    {
    }

    public override void SetData(Color color, Sprite additionalSprite = null)
    {
        throw new System.NotImplementedException();
    }

    public override void ProgressUp()
    {
        SpriteRenderer currentSr = _spriteRenderersList[_counter];

        if (currentSr != null) currentSr.color = new Color(1, 1, 1, 1);

        _counter++;
    }

    public override void ProgressUp(float increment)
    {
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsFilled()
    {
        throw new System.NotImplementedException();
    }

    public override void SetData(float index)
    {
        throw new System.NotImplementedException();
    }
}
