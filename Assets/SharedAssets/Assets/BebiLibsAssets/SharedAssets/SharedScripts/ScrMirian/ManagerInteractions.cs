using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Libs_Mirian
{

    public class ManagerInteractions : MonoBehaviour
    {

        public const int MAX_TOUCH_DETECT_COUNT = 30;
        public static ManagerInteractions instance;

        public bool allowMultiTouch = false;
        [SerializeField] private List<InteractableBase> _arrayInteractableBase = new List<InteractableBase>();
        [SerializeField] private InteractableBase[] _touchReferences = new InteractableBase[MAX_TOUCH_DETECT_COUNT];
        private bool _isTouchReferencesDirty = false;
        public System.Action OnInactive;
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public static void Register(InteractableBase interactable)
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(interactable.transform);// );
                instance = obj.AddComponent<ManagerInteractions>();
            }

            instance.IsActive = true;
            instance._arrayInteractableBase.Add(interactable);
        }

        private void Update()
        {
            if (!this.IsActive)
            {
                return;
            }

            this.UpdateTouches();

            for (int i = _arrayInteractableBase.Count - 1; i >= 0; i--)
            {
                _arrayInteractableBase[i].Run();
            }
        }

        public static void SetState(bool active)
        {
            if (!active)
            {
                instance.OnInactive?.Invoke();
                //Debug.Log("Here");
            }
            instance.enabled = active;
        }

        public bool isElementSelected = false;
        public int selectedFingerID = 0;

        private void UpdateTouches()
        {
#if UNITY_EDITOR
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                _touchReferences[0] = this.FindObjectForTouch(position);
            }

            if (_touchReferences[0] != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _touchReferences[0].OnInteractBegin(position);
                }
                else if (_touchReferences[0].isInteracted)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _touchReferences[0].OnInteractMove(position);
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        _touchReferences[0].OnInteractEnd();
                        _touchReferences[0] = null;
                    }
                }
            }
#endif

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                touch.position = Camera.main.ScreenToWorldPoint(touch.position);

                //int fingerID = allowMultiTouch ? touch.fingerId : 0;
                int fingerID = touch.fingerId;
                if (fingerID >= MAX_TOUCH_DETECT_COUNT) continue;

                if (touch.phase == TouchPhase.Began && isElementSelected == false)
                {
                    InteractableBase intBase = this.FindObjectForTouch(touch.position);
                    _touchReferences[fingerID] = intBase;

                    if (intBase != null && allowMultiTouch == false && intBase.GetInteractCondition())
                    {
                        isElementSelected = true;
                        selectedFingerID = fingerID;
                    }
                }


                if (_touchReferences[fingerID] != null)
                {
                    _isTouchReferencesDirty = true;

                    if (touch.phase == TouchPhase.Began)
                    {
                        _touchReferences[fingerID].OnInteractBegin(touch.position);
                    }
                    else if (_touchReferences[fingerID].isInteracted)
                    {
                        if (touch.phase == TouchPhase.Moved)
                        {
                            _touchReferences[fingerID].OnInteractMove(touch.position);
                        }
                        else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                        {
                            _touchReferences[fingerID].OnInteractStatic(touch.position);
                            if (!_touchReferences[fingerID].col.OverlapPoint(touch.position))
                            {
                                _touchReferences[fingerID].OnInteractEnd();
                                _touchReferences[fingerID] = null;

                                if (fingerID == selectedFingerID)
                                {
                                    isElementSelected = false;
                                }
                            }
                        }
                        else
                        {
                            if (fingerID == selectedFingerID)
                            {
                                isElementSelected = false;
                            }

                            _touchReferences[fingerID].OnInteractEnd();
                            _touchReferences[fingerID] = null;
                        }
                    }
                }
            }


            if (Input.touchCount == 0)
            {
                if (_isTouchReferencesDirty == true)
                {
                    for (int i = 0; i < _touchReferences.Length; i++)
                    {
                        if (_touchReferences[i] != null && _touchReferences[i].isInteracted)
                        {
                            Debug.LogError("Unusual Case");
                            _touchReferences[i].OnInteractEnd();
                            _touchReferences[i] = null;
                        }
                    }
                    _isTouchReferencesDirty = false;
                }
                this.isElementSelected = false;
            }

        }

        // private InteractableBase FindObjectForTouch(Touch touch)
        // {
        //     InteractableBase target = null;

        //     foreach (InteractableBase interactable in _arrayInteractableBase)
        //     {
        //         if (interactable.col.OverlapPoint(touch.position) && (target == null || target.interactableLayer < interactable.interactableLayer))
        //         {
        //             target = interactable;
        //         }
        //     }

        //     return target;
        // }

        private InteractableBase FindObjectForTouch(Vector2 position)
        {
            InteractableBase target = null;

            for (int i = _arrayInteractableBase.Count - 1; i >= 0; i--)
            {
                InteractableBase interactable = _arrayInteractableBase[i];
                if (interactable != null && interactable.col != null)
                {
                    if (interactable.col.OverlapPoint(position) && (target == null || target.interactableLayer < interactable.interactableLayer))
                    {
                        target = interactable;
                    }
                }
                else
                {
                    _arrayInteractableBase.RemoveAt(i);
                }
            }
            return target;
        }
    }
}