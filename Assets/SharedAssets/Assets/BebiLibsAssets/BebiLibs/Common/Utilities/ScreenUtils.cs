using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{

    public static class ScreenUtils
    {
        public static Vector2 GetScreenSize(Camera camera = null)
        {
#if UNITY_EDITOR
            Camera mainCamera = camera == null ? Camera.main : camera;
            if (mainCamera != null)
            {
                return new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
            }
            else
            {
                return RuntimeScreenSize;
            }
#else
        return RuntimeScreenSize;
#endif
        }

        public static Bounds ScreenBounds2D => new Bounds(Vector2.zero, 2 * Camera.main.ViewportToWorldPoint(Vector2.one));

        public static Rect GetScreenRect(Camera camera = null)
        {
            Vector2 screenSize = GetScreenSize(camera);
            return new Rect(0, 0, screenSize.x, screenSize.y);
        }


        public static Vector2 RuntimeScreenSize => new Vector2(Screen.width, Screen.height);

        public static Bounds GetScreenBounds(bool includeLeft, bool includeRight, bool includeTop, bool includeButton, Camera camera)
        {
            Rect safeRect = Screen.safeArea;
            Rect screenRect = GetScreenRect(camera);

            float x = includeLeft ? safeRect.x : screenRect.x;
            float rightOffset = includeLeft ? screenRect.x : safeRect.x;
            float width = includeRight ? safeRect.width + rightOffset : screenRect.width - x;

            float y = includeButton ? safeRect.y : screenRect.y;
            float topOffset = includeButton ? screenRect.y : safeRect.y;
            float height = includeTop ? safeRect.height + topOffset : screenRect.height - y;

            Rect finalRect = new Rect(x, y, width, height);
            Vector2 safeOffset = camera.ScreenToWorldPoint(finalRect.center);
            finalRect.center = screenRect.size / 2.0f;
            Bounds bounds = new Bounds(safeOffset, 2 * (Vector2)camera.ScreenToWorldPoint(finalRect.max));
            return bounds;
        }

        public static Rect GetScreenSafeRect(bool includeLeft, bool includeRight, bool includeTop, bool includeButton, Camera camera)
        {
            Rect safeRect = Screen.safeArea;
            Rect screenRect = GetScreenRect(camera);

            float x = includeLeft ? safeRect.x : screenRect.x;
            float rightOffset = includeLeft ? screenRect.x : safeRect.x;
            float width = includeRight ? safeRect.width + rightOffset : screenRect.width - x;

            float y = includeButton ? safeRect.y : screenRect.y;
            float topOffset = includeButton ? screenRect.y : safeRect.y;
            float height = includeTop ? safeRect.height + topOffset : screenRect.height - y;

            Rect finalRect = new Rect(x, y, width, height);
            finalRect.center = screenRect.size / 2.0f;
            return finalRect;
        }


        public static Bounds GetScreenBounds(bool includeLeft, bool includeRight, bool includeTop, bool includeButton)
        {
            return GetScreenBounds(includeLeft, includeRight, includeTop, includeButton, Camera.main);
        }


        public static bool IsRectTransformVisible(this RectTransform rectTransform, Camera camera)
        {
            Bounds screenBounds = GetScreenBounds(true, false, false, false, camera);
            return IsPartlyVisible(rectTransform, screenBounds);
        }

        static Vector3[] _ObjectCornersArray = new Vector3[4];
        public static bool IsPartlyVisible(this RectTransform rectTransform, Bounds screenBounds)
        {
            rectTransform.GetWorldCorners(_ObjectCornersArray);
            for (var i = 0; i < _ObjectCornersArray.Length; i++)
            {
                if (screenBounds.Contains((Vector2)_ObjectCornersArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
