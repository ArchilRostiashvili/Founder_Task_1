using UnityEngine;

namespace BebiLibs
{
    public class CrossPlatformInput : MonoBehaviour
    {
        public static bool InputClick = false;
        public static bool InputUp = false;
        public static bool InputHold = false;
        public static Vector3 InputPosition;
        public static Vector3 InputWorldPosition;
        private static Camera _mainCamera;
        private static CrossPlatformInput _instance;

        [HideInInspector]
        private static int _activeFingerID;
        private static bool _tryOtherTouch;
        private static Vector2 _tryOtherTouchPosition;
        public static int ActiveFingerID
        {
            get => _activeFingerID;
        }

        public static Camera mainCamera
        {
            get => _mainCamera == null ? _mainCamera = Camera.main : _mainCamera;
            set => _mainCamera = _mainCamera == null ? value : _mainCamera;
        }

        private void OnEnable()
        {
            _mainCamera = Camera.main;
        }


        private void Awake()
        {

            if (_instance == null)
            {
                _instance = this;
            }
        }


        private void Update()
        {
            if (_mainCamera == null)
            {
                Debug.LogWarning("Main Camera is null");
                return;
            }

            int touchCount = Input.touchCount;
            if (touchCount == 0)
            {
                _activeFingerID = -1;
                InputHold = false;
            }

            InputClick = false;
            InputUp = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            InputClick = Input.GetMouseButtonDown(0);
            InputHold = Input.GetMouseButton(0);
            InputUp = Input.GetMouseButtonUp(0);
            InputPosition = Input.mousePosition;
            InputWorldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(InputPosition);
#endif

            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    if (_activeFingerID == -1)
                    {
                        _tryOtherTouch = false;
                        _activeFingerID = touch.fingerId;
                        InputClick = true;
                        InputUp = false;
                        InputHold = false;
                        InputPosition = touch.position;
                        InputWorldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(InputPosition);
                    }
                    else if (_activeFingerID != touch.fingerId && _activeFingerID != -1 && _tryOtherTouch == false)
                    {
                        _tryOtherTouch = true;
                        _tryOtherTouchPosition = InputPosition;
                    }
                }
                else
                if (touch.phase == TouchPhase.Stationary && _activeFingerID == touch.fingerId && _tryOtherTouch)
                {
                    _activeFingerID = -1;
                    _tryOtherTouch = false;
                    InputUp = true;
                    InputClick = false;
                    InputHold = false;
                    InputPosition = _tryOtherTouchPosition;
                    InputWorldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(_tryOtherTouchPosition);
                }
                else
                if (touch.phase == TouchPhase.Ended && _activeFingerID == touch.fingerId)
                {
                    InputClick = false;
                    InputHold = false;
                    InputUp = true;
                    _tryOtherTouch = false;
                    _activeFingerID = -1;
                    InputPosition = touch.position;
                    InputWorldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(InputPosition);
                }
                else
                if (touch.phase == TouchPhase.Moved && _activeFingerID == touch.fingerId)
                {
                    InputClick = false;
                    InputUp = false;
                    InputHold = true;
                    InputPosition = touch.position;
                    InputWorldPosition = (Vector2)_mainCamera.ScreenToWorldPoint(InputPosition);
                }
            }
        }
    }
}