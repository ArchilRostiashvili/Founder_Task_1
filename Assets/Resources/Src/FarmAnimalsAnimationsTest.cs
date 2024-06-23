using Bebi.FarmLife;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimalsAnimationsTest : MonoBehaviour
{
    [SerializeField] private List<FarmAnimalAnimator> _availableAnimals = new List<FarmAnimalAnimator>();

    public void SetAllHappy()
    {
        foreach (var item in _availableAnimals)
        {
            item.SetHappy();
        }
    }

    public void SetAllExcited()
    {
        foreach (var item in _availableAnimals)
        {
            item.SetExcited();
        }
    }

    public void SetAllSad()
    {
        foreach (var item in _availableAnimals)
        {
            item.SetSad();
        }
    }
}
