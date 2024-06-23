using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.Data.LobbySession
{
    [System.Serializable]
    public class LobbyContentItem
    {
        [SerializeField] private GameObject _contentItemGO;
        [SerializeField] private string _contentItemID;

        public GameObject ContentGO => _contentItemGO;
        public string ContentItemID => _contentItemID;
    }
}