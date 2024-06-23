using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife
{
    public class CloudSystem : MonoBehaviour
    {
        [SerializeField] private List<ItemCloud> _itemCloudsList = new List<ItemCloud>();

        [SerializeField] private Transform _startTR;
        [SerializeField] private Transform _endTR;

        [SerializeField] private float _speed;
        [SerializeField] private bool _selfInitialize;

        private bool _isInitialized;

        private void Start()
        {
            if (_selfInitialize)
                Init();
        }

        public void Init()
        {
            foreach (ItemCloud itemCloud in _itemCloudsList)
            {
                itemCloud.SetData(_startTR, _endTR, _speed);
                itemCloud.Activate();
            }

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            for (int i = 0; i < _itemCloudsList.Count; i++)
            {
                _itemCloudsList[i].Move();
            }
        }
    }
}

