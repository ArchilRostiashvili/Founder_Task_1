using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using UnityEngine;

namespace FarmLife.Data
{

    [CreateAssetMenu(fileName = "AnimalData", menuName = "FarmLife/AnimalData")]
    public class RandomAnimalData : ScriptableObject
    {
        [SerializeField] private string _animalID;
        [SerializeField] private GameObject _animalGO;
        public GameObject AnimalGO => _animalGO;
    }
}