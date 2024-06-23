using BebiAnimations.Libs.Core;
using System;
using UnityEngine;


namespace BebiInteractions.Libs
{
    public class InteractableItemBase : MonoBehaviour
    {
        public string ItemID;
        public bool _isEnabled;
        protected bool _isDone;

        public Transform ContentTR;
        [HideInInspector] public int ItemCount;
        [SerializeField] private Collider2D _collider;

        public virtual void Reset()
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }

            //_bebiAnimator.Initialize();

            _isDone = false;
            Enable(true);
        }

        public virtual void Enable(bool isEnabled)
        {
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
            Enable(false);
            _isDone = true;
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
        }

        public bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public bool Collider
        {
            get
            {
                return _collider;
            }
        }
    }
}
