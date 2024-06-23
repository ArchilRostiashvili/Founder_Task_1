using UnityEngine;
using UnityEngine.Events;

namespace FarmLife
{
    public class MinigameInteractableBase : MonoBehaviour
    {
        public UnityEvent OnInteract;

        [SerializeField] protected bool _interactOnInitialization;

        private MinigameInteractable _minigameInteractable;
        [SerializeField] private bool _clickEnabled;


        public void SetClickEnabled(bool value) => _clickEnabled = value;

        protected virtual void Update()
        {
            if (!_clickEnabled) return;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _minigameInteractable = HitPoint(_wp);
                TapDetected(_minigameInteractable);
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                Vector3 _wp = Camera.main.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    _minigameInteractable = HitPoint(_wp);
                    TapDetected(_minigameInteractable);
                }
            }
#endif
        }

        protected virtual void TapDetected(MinigameInteractable minigameInteractable)
        {
            if (minigameInteractable != null)
            {
                OnInteract?.Invoke();
            }
        }

        private MinigameInteractable HitPoint(Vector2 p)
        {
            Collider2D[] array = Physics2D.OverlapPointAll(p);

            MinigameInteractable interactable = null;
            for (int i = 0; i < array.Length; i++)
            {
                interactable = array[i].GetComponent<MinigameInteractable>();
                if (interactable != null)
                {
                    return interactable;
                }
            }

            return null;
        }
    }
}