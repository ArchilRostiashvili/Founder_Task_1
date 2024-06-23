using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Android;

namespace BebiLibs
{
    public class ButtonDragController : MonoBehaviour
    {

#if AMAZON_BUILD
        private void OnEnable()
        {
            float ScreenDPI = Screen.dpi != 0 ? Screen.dpi : 213.0f;
            int _dragTH = (int)Mathf.Ceil((0.035f * ScreenDPI) - 2.83333f);
            _dragTH = Mathf.Clamp(_dragTH, 3, 12);

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                eventSystem.pixelDragThreshold = _dragTH;
            }

            Debug.LogWarning("Drag Threshold: " + _dragTH + ", Screen DPI: " + Screen.dpi);
        }
#endif
    }
}
