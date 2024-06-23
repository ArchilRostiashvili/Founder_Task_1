using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.Data.LobbySession
{
    [System.Serializable]
    public struct RandomContentPositionData
    {
        [SerializeField] private Transform _contentPositionTR;
        [SerializeField] private int _contentID;

        private string _contentName;

        public int ContentID => _contentID;
        public string ContentName => _contentName;
        public Transform ContentPositionTR => _contentPositionTR;

        public RandomContentPositionData(Transform contentPositionTR, int contentID, string contentName)
        {
            _contentPositionTR = contentPositionTR;
            _contentID = contentID;
            _contentName = contentName;
        }

    }
}
