using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{

    public class SafeAreaSides : MonoBehaviour
    {
        [SerializeField] private RectTransform _safeAreaPanelRT;


        [Header("Right Effect:")]
        [SerializeField] private bool _includeRight;
        [Range(0, 1)]
        [SerializeField] private float _effectWeightRight = 1f;

        [Header("Left Effect:")]
        [SerializeField] private bool _includeLeft;
        [Range(0, 1)]
        [SerializeField] private float _effectWeightLeft = 1f;

        [Header("Bottom Effect:")]
        [SerializeField] private bool _includeBottom;
        [Range(0, 1)]
        [SerializeField] private float _effectWeightBottom = 1f;

        [Header("Top Effect:")]
        [SerializeField] private bool _includeTop;
        [Range(0, 1)]
        [SerializeField] private float _effectWeightTop = 1f;



        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private ScreenOrientation _lastScreenOrientation;

        private void SetSafeAreaPlane(RectTransform plane)
        {
            _safeAreaPanelRT = plane;
        }

        private void OnEnable()
        {
            Refresh(true);
        }


        private void Update()
        {
            Refresh();
        }

        private void Refresh(bool forceRecalculation = false)
        {
            Rect safeArea = ScreenUtils.GetScreenSafeRect(_includeLeft, _includeRight, _includeTop, _includeBottom, Camera.main);
            ScreenOrientation screenOrientation = Screen.orientation;
            if (safeArea != _lastSafeArea || _lastScreenOrientation != screenOrientation || forceRecalculation)
            {
                UpdateRectTransform(_safeAreaPanelRT);
                _lastScreenOrientation = screenOrientation;
                _lastSafeArea = safeArea;
            }
        }

        public void UpdateRectTransform(RectTransform rectTransform)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = ScreenUtils.GetScreenSize();
            LandscapeLeftSafeArea(rectTransform, safeArea, screenSize);
        }


        private void LandscapeLeftSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorMax;
            Vector2 anchorMin;

            anchorMin = safeRect.position / screenSize;
            anchorMin.y = Mathf.Lerp(0, anchorMin.y, _effectWeightBottom);
            anchorMin.x = Mathf.Lerp(0, anchorMin.x, _effectWeightLeft);

            anchorMax = (safeRect.position + safeRect.size) / screenSize;
            anchorMax.y = Mathf.Lerp(anchorMax.y, 1, 1 - _effectWeightTop);
            anchorMax.x = Mathf.Lerp(anchorMax.x, 1, 1 - _effectWeightRight);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }


        private static void DefaultSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorMin = safeRect.position / screenSize;
            Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

    }
}