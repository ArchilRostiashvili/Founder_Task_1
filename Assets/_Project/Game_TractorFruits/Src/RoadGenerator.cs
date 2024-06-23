using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    public class RoadGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _movingObject;

        [SerializeField] private List<GameObject> _roadsList = new List<GameObject>();
        [SerializeField] private List<GameObject> _backgroundsList = new List<GameObject>();

        [SerializeField] private float _moveSpeed;

        [SerializeField] private Transform _point1Road;
        [SerializeField] private Transform _point2Road;
        [SerializeField] private Transform _point1BG;
        [SerializeField] private Transform _point2BG;

        private Transform _stopPoint;
        private Transform _stopTarget;

        private bool _isActivated = false;
        private bool _stopPointSetted = false;
        private bool _callbackSent = false;

        private Action OnPoint;

        public void Activate()
        {
            _isActivated = true;
            _stopPointSetted = false;
            _callbackSent = false;
        }

        public void Deactivate()
        {
            _isActivated = false;
        }

        public void SetStopPoint(Transform stopPointTR, Transform stopTargetTR, Action callback)
        {
            _stopPoint = stopPointTR;
            _stopTarget = stopTargetTR;
            _stopPointSetted = true;
            OnPoint = callback;
        }

        private void Update()
        {
            if (_stopPointSetted && _stopTarget.position.x <= _stopPoint.position.x)
            {
                if (!_callbackSent)
                {
                    OnPoint?.Invoke();
                    _callbackSent = true;
                }

                return;
            }



            if (!_isActivated)
                return;

            for (int i = 0; i < _roadsList.Count; i++)
            {
                if (_roadsList[i].transform.position.x < _point1Road.position.x)
                {
                    _roadsList[i].transform.position = new Vector3(_point2Road.position.x,
                        _roadsList[i].transform.position.y, _roadsList[i].transform.position.z);
                }
            }

            for (int i = 0; i < _backgroundsList.Count; i++)
            {
                if (_backgroundsList[i].transform.position.x < _point1BG.position.x)
                {
                    _backgroundsList[i].transform.position = new Vector3(_point2BG.position.x,
                        _backgroundsList[i].transform.position.y, _backgroundsList[i].transform.position.z);
                }
            }

            _movingObject.transform.Translate(Vector3.left * (_moveSpeed * Time.deltaTime));
        }
    }
}