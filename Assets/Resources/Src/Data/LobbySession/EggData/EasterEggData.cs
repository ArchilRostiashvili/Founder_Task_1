using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using UnityEngine;

namespace FarmLife.Data.LobbySession
{
    public class EasterEggData
    {
        public int Index;
        public FarmAnimalAnimator RandomAnimalGO;
        public Sprite EggSprite;
        public bool IsCracked;
        public EggPositionData EggPositionData;

        public EasterEggData(Sprite eggSprite, EggPositionData eggPositionData, FarmAnimalAnimator randomAnimal, int index)
        {
            EggSprite = eggSprite;
            EggPositionData = eggPositionData;
            RandomAnimalGO = randomAnimal;
            Index = index;
        }
    }
}