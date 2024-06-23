using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.Data.LobbySession
{
    [System.Serializable]
    public struct EggPositionData
    {
        [SerializeField] private Transform _eggPositionTR;
        [SerializeField] private int _eggLayerOrder;
        [SerializeField] private int _crackedEggLayerOrder;
        [SerializeField] private bool _isInWater;
        [SerializeField] private int _id;

        public Transform EggTRansform => _eggPositionTR;
        public int ID => _id;
        public int EggLayerOrder => _eggLayerOrder;
        public int CrackedEggLayerOrder => _crackedEggLayerOrder;
        public bool IsInWater => _isInWater;

        public void SetID(int ID)
          => _id = ID;
    }
}