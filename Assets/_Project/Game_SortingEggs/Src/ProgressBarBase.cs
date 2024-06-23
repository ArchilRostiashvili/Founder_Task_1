using FarmLife;
using UnityEngine;

public abstract class ProgressBarBase : MonoBehaviour
{
    public abstract void SetData(float index, Color color, Sprite additionalSprite = null);
    public abstract void SetData(Color color, Sprite additionalSprite = null);
    public abstract void ProgressUp();

    public abstract void ProgressUp(float increment);

    public abstract void Reset();

    public abstract bool IsFilled();

    public virtual void SetData(float index) { }
}
