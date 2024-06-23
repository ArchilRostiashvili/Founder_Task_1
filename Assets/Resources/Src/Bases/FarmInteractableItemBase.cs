using UnityEngine;

namespace FarmLife
{
    public class FarmInteractableItemBase : MonoBehaviour
    {
        public string ItemID;
        public bool _isEnabled;
        protected bool _isDone;
        protected bool _isDeactivated;

        public Transform ContentTR;
        public Transform HelperPointTR;

        [HideInInspector] public int ItemCount;
        [SerializeField] private Collider2D _collider;

        [SerializeField] private int _maxTaps;

        private int _correctCount;
        private int _tapCount;

        public virtual void Reset()
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }

            _tapCount = 0;

            _isDone = false;
            Enable(true);
        }

        public virtual void Enable(bool isEnabled)
        {
            if (_isDeactivated)
                return;

            if (_isDone)
            {
                return;
            }

            _isEnabled = isEnabled;
            if (_collider != null)
            {
                _collider.enabled = isEnabled;
            }
        }

        public virtual void Done()
        {
            _correctCount++;
            if (_correctCount < ItemCount)
            {
                return;
            }

            Enable(false);
            _isDone = true;
        }

        public virtual void Deactivate()
        {
            _isDeactivated = true;
            _isEnabled = false;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        public bool IsDone
        {
            get { return _isDone; }
            set { _isDone = value; }
        }

        public bool Collider
        {
            get { return _collider; }
        }

        public bool IsTappedMax
        {
            get { return _tapCount > _maxTaps; }
        }

        public void CountTap()
        {
            _tapCount++;
        }

        public bool ObjectIsVisible()
        {
            Vector3 objectPosition = transform.position;
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(objectPosition);

            return (viewportPosition.x is > 0 and < 1 && viewportPosition.y is > 0 and < 1);
        }
    }
}