using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{

    public class SafeArea : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _safeAreaPanel;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private ScreenOrientation _lastScreenOrientation;

        [Range(0, 1)]
        [Tooltip("When weight is 1 safe area matches device safe area, 0 safe area has no effect, only works with landscape mode")]
        public float effectWeight = 1f;

        private void SetSafeAreaPlane(RectTransform plane)
        {
            _safeAreaPanel = plane;
        }

        private void OnEnable()
        {
            this.Refresh(true);
        }


        private void Update()
        {
            this.Refresh();
        }

        private void Refresh(bool forceRecalculation = false)
        {
            Rect safeArea = Screen.safeArea;
            ScreenOrientation screenOrientation = Screen.orientation;
            if (safeArea != _lastSafeArea || _lastScreenOrientation != screenOrientation || forceRecalculation)
            {
                UpdateRectTransform(_safeAreaPanel, this.effectWeight);
                _lastScreenOrientation = screenOrientation;
                _lastSafeArea = safeArea;
            }
        }

        public static void UpdateRectTransform(RectTransform rectTransform, float weight)
        {
            Rect safeArea = Screen.safeArea;

            Vector2 screenSize = ScreenUtils.GetScreenSize();
            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    Portrait(rectTransform, safeArea, screenSize, weight);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    PortraitUpsideDown(rectTransform, safeArea, screenSize, weight);
                    break;
                case ScreenOrientation.LandscapeLeft:
                    LandscapeLeftSafeArea(rectTransform, safeArea, screenSize, weight);
                    break;
                case ScreenOrientation.LandscapeRight:
                    LandscapeRightSafeArea(rectTransform, safeArea, screenSize, weight);
                    break;
            }
        }


        private static void Portrait(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
        {
            Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
            anchorMax.x = 1;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = anchorMax;
        }

        private static void PortraitUpsideDown(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
        {
            Vector2 anchorMin = safeRect.position / screenSize;
            anchorMin.x = 0;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = Vector2.one;
        }

        private static void LandscapeLeftSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
        {
            Vector2 anchorMin = safeRect.position / screenSize;
            anchorMin.y = 0;
            anchorMin.x = Mathf.Lerp(0, anchorMin.x, weight);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = Vector2.one;
        }

        private static void LandscapeRightSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
        {
            Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
            anchorMax.y = 1;
            anchorMax.x = Mathf.Lerp(anchorMax.x, 1, 1 - weight);

            rectTransform.anchorMin = Vector2.zero;
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