using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    [CreateAssetMenu(fileName = "TractorFruitsData", menuName = "FarmLife/Data/Minigames/TractorFruits")]
    public class TractorFruitsData : MiniGameBaseData
    {
        [SerializeField] private List<FruitData> _fruitDataList = new List<FruitData>();
        [SerializeField] private List<AnimalData> _animalDataList = new List<AnimalData>();
        private List<FruitData> _activeFruitDataList = new List<FruitData>();

        public List<FruitData> FruitData => _fruitDataList;
        public List<FruitData> ActiveFruitList => _activeFruitDataList;
        public List<AnimalData> AnimalList => _animalDataList;

        public void CreateData(int amount)
        {
            _activeFruitDataList.Clear();

            if (amount >= _fruitDataList.Count)
            {
                Debug.LogError("amount cant be more than data list itself");
                return;
            }
            Utils.Shuffle(ref _fruitDataList);

            for (int i = 0; i < amount; i++)
                _activeFruitDataList.Add(_fruitDataList[i]);
        }
    }
}