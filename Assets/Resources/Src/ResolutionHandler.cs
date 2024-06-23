using System;
using UnityEngine;

namespace FarmLife
{
    public class ResolutionHandler : MonoBehaviour
    {
        [SerializeField] private float proportion = 1f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            SetSize();
        }

        private void Update()
        {
            SetSize();
        }

        private void SetSize()
        {
            float unitsPerPixel = proportion / _mainCamera.pixelWidth;
            float desiredHalfHeight = 0.5f * unitsPerPixel * _mainCamera.pixelHeight;

            _mainCamera.orthographicSize = desiredHalfHeight;
        }
    }
}