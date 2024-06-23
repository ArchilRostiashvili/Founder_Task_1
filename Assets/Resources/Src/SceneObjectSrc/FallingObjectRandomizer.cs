using System;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectRandomizer : MonoBehaviour
{
    [SerializeField] private List<FallingObjectRandomRow> _fallingObjectRowsList = new List<FallingObjectRandomRow>();

    private void Awake()
    {
        RandomizeObject();
    }

    private void RandomizeObject()
    {
        List<int> randomObjectList = new List<int>();
        
        for (int i = 0; i < _fallingObjectRowsList[0].FallingObjectsCount(); i++)
        {
            randomObjectList.Add(i);
        }

        Utils.Shuffle(ref randomObjectList);
        
        for (int i = 0; i < _fallingObjectRowsList.Count; i++)
        {
            _fallingObjectRowsList[i].SpawnRandomObject(randomObjectList[i]);
        }
    }
}
