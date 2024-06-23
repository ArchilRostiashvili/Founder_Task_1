using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EducationalGames.Helpers
{
    public class ClickDetector : MonoBehaviour
    {
        public System.Action<Clickable> ClickDetectedEvent;

        private bool _clickEnabled;

        public void SetClickEnabled(bool value) => _clickEnabled = value;

        private void Update()
        {
            if (!_clickEnabled) return;

            Clickable clickable = null;
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                clickable = HitPoint(_wp);
                ClickDetectedEvent?.Invoke(clickable);
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                Vector3 _wp = Camera.main.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    clickable = HitPoint(_wp);
                    ClickDetectedEvent?.Invoke(clickable);
                }
            }
#endif
        }

        private Clickable HitPoint(Vector2 p)
        {
            Collider2D[] array = Physics2D.OverlapPointAll(p);

            Clickable clickable = null;
            for (int i = 0; i < array.Length; i++)
            {
                clickable = array[i].GetComponent<Clickable>();
                if (clickable != null)
                {
                    return clickable;
                }
            }
            return null;
        }
    }

    public abstract class Clickable : MonoBehaviour
    {

    }
}

