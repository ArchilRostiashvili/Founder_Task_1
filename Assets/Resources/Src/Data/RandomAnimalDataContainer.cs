using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FarmLife.Data.LobbySession
{
    [System.Serializable]
    public class RandomAnimalDataContainer
    {
        public string RandomAnimalID;
        public AssetReference RandomAnimalReference;
        public Vector2 AnimalScaleUpValue = Vector2.one;
    }
}