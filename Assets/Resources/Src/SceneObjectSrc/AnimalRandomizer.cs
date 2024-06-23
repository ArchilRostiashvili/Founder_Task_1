using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRandomizer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _animalsList = new List<GameObject>();

    private void Start()
    {
        _animalsList.GetRandomElement().SetActive(true);
    }
}
