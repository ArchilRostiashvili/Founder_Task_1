using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class SafeAreaWorld : MonoBehaviour
    {
        [SerializeField] private Vector2 _offest = Vector2.zero;
        [SerializeField] private Vector2 _anchor = Vector2.zero;

        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private ScreenOrientation _lastScreenOrientation;

        [SerializeField] private bool _ignoreSafeArea = false;
        [SerializeField] private bool _ignoreTopArea = false;
        [SerializeField] private bool _ignoreBottomArea = false;
        [SerializeField] private bool _ignoreLeftArea = false;
        [SerializeField] private bool _ignoreRightArea = false;
        [SerializeField] private bool _refreshOnEnable = false;

        private Rect _screenRect;

        private void Awake()
        {
            if (Camera.main == null) return;
            this.Refresh();
            //Debug.Log("Awake Area World");
        }

        private void OnEnable()
        {
            if(_refreshOnEnable) this.Refresh();
        }

        private Rect GetScreenRect()
        {
            return new Rect(0, 0, Screen.width, Screen.height);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (Camera.main == null) return;
                this.Refresh();
            }

            if (Screen.safeArea != _lastSafeArea || _lastScreenOrientation != Screen.orientation)
            {
                this.Refresh();
                _lastSafeArea = Screen.safeArea;
                _lastScreenOrientation = Screen.orientation;
            }
        }



        public Vector2 GetPosition()
        {
            Rect safeArea = _ignoreSafeArea == true ? this.GetScreenRect() : Screen.safeArea;

            float x = _ignoreLeftArea ? 0 : safeArea.x;
            float y = _ignoreBottomArea ? 0 : safeArea.y;
            float width = _ignoreRightArea ? Screen.width : safeArea.width;
            float height = _ignoreTopArea ? Screen.height : safeArea.height;

            safeArea = new Rect(x, y, width, height);


            Vector2 anchoredScreenPosition;
            //Vector2 anchorWoldPoint = (safeArea.min + safeArea.size) * _anchor;
            float xPos = Mathf.Lerp(safeArea.min.x, Mathf.Clamp(safeArea.max.x, 0, Screen.width), _anchor.x);
            float yPos = Mathf.Lerp(safeArea.min.y, Mathf.Clamp(safeArea.max.x, 0, Screen.height), _anchor.y);
            anchoredScreenPosition = new Vector2(xPos, yPos);

            Vector2 elementPosition = Camera.main.ScreenToWorldPoint(anchoredScreenPosition) + (Vector3)_offest;

            return elementPosition;
        }

        private void Refresh()
        {
            //Debug.Log("Refresh Area World");
            if (Camera.main != null)
            {
                this.transform.position = this.GetPosition();
            }
        }
    }
}
