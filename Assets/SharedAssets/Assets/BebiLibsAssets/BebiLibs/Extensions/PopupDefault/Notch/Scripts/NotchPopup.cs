using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.PopupManagementSystem;
using UnityEngine.UI;

public class NotchPopup : MonoBehaviour
{
    private static NotchPopup _Instance;

    [SerializeField] private RectTransform _safeAreaPanelRT;
    [Range(0, 1)]
    [Tooltip("When weight is 1 safe area matches device safe area, 0 safe area has no effect, only works with landscape mode")]
    [SerializeField] private float _effectWeight = 1f;

    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation _lastScreenOrientation;

    public static NotchPopup GetDefaultInstance()
    {
        GameObject notchPopupPrefab = Resources.Load<GameObject>("NotchPopup");
        GameObject notchInstanceOBJ = GameObject.Instantiate(notchPopupPrefab);
        _Instance = notchInstanceOBJ.GetComponent<NotchPopup>();
        return _Instance;
    }

    private void SetSafeAreaPlane(RectTransform plane)
    {
        _safeAreaPanelRT = plane;
    }

    void Awake()
    {
        Refresh();
        _safeAreaPanelRT.gameObject.SetActive(true);
    }

    private void Update()
    {
        Refresh();
    }

    public static void Show()
    {
        if (_Instance == null)
        {
            _Instance = GetDefaultInstance();
        }
        _Instance.Show(false);
    }

    public static void Hide()
    {
        if (_Instance != null)
        {
            _Instance.Hide(false);
        }

    }

    public void Hide(bool anim)
    {
        _safeAreaPanelRT.gameObject.SetActive(false);
        enabled = false;
    }

    public void Show(bool anim)
    {
        _safeAreaPanelRT.gameObject.SetActive(true);
        enabled = true;
        Refresh();
    }

    private void Refresh()
    {
        Rect safeArea = Screen.safeArea;
        ScreenOrientation screenOrientation = Screen.orientation;
        if (safeArea != _lastSafeArea || _lastScreenOrientation != screenOrientation)
        {
            UpdateRectTransform(_safeAreaPanelRT, _effectWeight);
            _lastScreenOrientation = screenOrientation;
            _lastSafeArea = safeArea;
        }
    }

    private static void UpdateRectTransform(RectTransform rectTransform, float weight)
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

        rectTransform.ForceUpdateRectTransforms();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }


    private static void Portrait(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
    {
        Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
        anchorMax.x = 0;
        rectTransform.anchorMin = anchorMax;
        rectTransform.anchorMax = Vector2.one;
    }

    private static void PortraitUpsideDown(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
    {
        Vector2 anchorMin = safeRect.position / screenSize;
        anchorMin.x = 1;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = anchorMin;
    }

    private static void LandscapeLeftSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
    {
        Vector2 anchorMin = safeRect.position / screenSize;
        anchorMin.y = 1;
        anchorMin.x = Mathf.Lerp(0, anchorMin.x, weight);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = anchorMin;
    }

    private static void LandscapeRightSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize, float weight)
    {
        Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
        anchorMax.y = 0;
        anchorMax.x = Mathf.Lerp(anchorMax.x, 1, 1 - weight);
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = anchorMax;

    }

    private static void DefaultSafeArea(RectTransform rectTransform, Rect safeRect, Vector2 screenSize)
    {
        Vector2 anchorMin = safeRect.position / screenSize;
        Vector2 anchorMax = (safeRect.position + safeRect.size) / screenSize;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
