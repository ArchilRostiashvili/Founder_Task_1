using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarouselScrollBehaviour : MonoBehaviour
{
    public abstract void Initialize(CarouselScroll scroll);
    public abstract void UpdateScroll(CarouselScroll scroll);
    public abstract void OnScrollChange(ScrollElement newElement);
    public abstract void OnFinishScrollUpdate(CarouselScroll carousel);
    public abstract void Clear();
}
