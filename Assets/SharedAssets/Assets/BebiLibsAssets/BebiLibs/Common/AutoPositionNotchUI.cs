
using UnityEngine;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class AutoPositionNotchUI : MonoBehaviour
    {
        public Vector2 position;
        public Vector2 anchor;

        public Vector2 offsetEdge;

        private Vector2 _origin;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private ScreenOrientation _lastScreenOrientation;

        public void ChangePosition(Vector2 p)
        {
            this.position = p;
            this.UpdateTransform();
        }

        private void Awake()
        {

            this.Refresh();
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_origin, 0.1f);
            Gizmos.DrawWireSphere(this.transform.position, 0.1f);
            Gizmos.DrawLine(_origin, this.transform.position);
        }
#endif

        public void Update()
        {
            this.Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = Screen.safeArea;
            ScreenOrientation screenOrientation = Screen.orientation;
            //if (safeArea != _lastSafeArea || _lastScreenOrientation != screenOrientation)
            //{
            this.UpdateTransform();
            _lastScreenOrientation = screenOrientation;
            _lastSafeArea = safeArea;
            //}

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                this.UpdateTransform();
            }
#endif
        }

        public void UpdateTransform()
        {
            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    this.Portrait(safeArea, screenSize);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    this.PortraitUpsideDown(safeArea, screenSize);
                    break;
                case ScreenOrientation.LandscapeLeft:
                    this.LandscapeLeftSafeArea(safeArea, screenSize);
                    break;
                case ScreenOrientation.LandscapeRight:
                    this.LandscapeRightSafeArea(safeArea, screenSize);
                    break;
            }
        }


        private void Portrait(Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorTemp = this.anchor;
            float yMax = (safeRect.position.y + safeRect.size.y) / screenSize.y;
            float valueYDiff = 1.0f - yMax;

            if (0.5f < this.anchor.y)
            {
                anchorTemp.y -= valueYDiff;
            }
            else
            {

            }
            this.UpdatePosition(anchorTemp);
        }

        private void PortraitUpsideDown(Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorTemp = this.anchor;
            float yMin = safeRect.position.y / screenSize.y;
            if (this.anchor.x < 0.5f)
            {
                anchorTemp.x += yMin;
            }
            else
            {
                float yMax = (safeRect.position.y + safeRect.size.y) / screenSize.y;
                float valueYDiff = 1.0f - yMax;
                anchorTemp.y -= valueYDiff * 0.2f * this.offsetEdge.x;
            }
            this.UpdatePosition(anchorTemp);
        }

        private void LandscapeLeftSafeArea(Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorTemp = this.anchor;
            float xMin = safeRect.position.x / screenSize.x;

            if (this.anchor.x < 0.5f)
            {
                anchorTemp.x += xMin;
            }
            else
            {
                //Debug.Log("safeRect.position.x = " + safeRect.position.x);
                //Debug.Log("safeRect.size.x = " + safeRect.size.x);
                //Debug.Log("screenSize.x = " + screenSize.x);
                float xMax = (safeRect.position.x + safeRect.size.x) / screenSize.x;
                float valueXDiff = (1.0f - xMax);
                anchorTemp.x -= valueXDiff * this.offsetEdge.x;
            }
            this.UpdatePosition(anchorTemp);
        }

        private void LandscapeRightSafeArea(Rect safeRect, Vector2 screenSize)
        {
            Vector2 anchorTemp = this.anchor;
            float xMax = (safeRect.position.x + safeRect.size.x) / screenSize.x;
            float valueXDiff = 1.0f - xMax;

            if (0.5f < this.anchor.x)
            {
                anchorTemp.x -= valueXDiff;
            }
            else
            {
                float xMin = safeRect.position.x / screenSize.x;
                anchorTemp.x += xMin * 0.2f * this.offsetEdge.x;
            }
            this.UpdatePosition(anchorTemp);
        }

        private void UpdatePosition(Vector2 anchorTemp)
        {
            _origin = new Vector2((anchorTemp.x - 0.5f) * Screen.width * Common.PPU, (anchorTemp.y - 0.5f) * Screen.height * Common.PPU);
            anchorTemp = _origin + this.position;

            if (anchorTemp.x != Mathf.Infinity && anchorTemp.y != Mathf.Infinity)
            {
                this.transform.localPosition = new Vector3(anchorTemp.x, anchorTemp.y, 0);
            }
        }
    }
}