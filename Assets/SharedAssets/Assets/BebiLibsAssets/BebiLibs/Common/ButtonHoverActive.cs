using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class ButtonHoverActive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool buttonEnabled = true;
        [Header("Scale Data")]
        public Ease scaleEaseing = Ease.Linear;
        public Ease scaleBackEasing = Ease.Linear;
        public float scaleTotal = 0.2f;
        public float scaleTime = 0.1f;
        public float scaleBackTime = 0.1f;

        [Header("Events")]
        public ButtonClickedEvent onClick = new ButtonClickedEvent();


        private Image _imageInteraction;
        private bool _touchDown;
        private Transform _transform;
        [HideInInspector]

        public Vector3 startScale;
        private bool _isInitialize = false;

        protected void Start()
        {
            //base.Start();
            this.Init();
        }

        public virtual void Init()
        {
            if (_isInitialize == false)
            {
                _transform = this.GetComponent<Transform>();
                this.startScale = _transform.localScale;
            }
            _imageInteraction = this.GetComponent<Image>();
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = this.enabled;
            }
            _isInitialize = true;
        }

        public void Enable(bool bl)
        {
            this.buttonEnabled = bl;
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = this.buttonEnabled;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.buttonEnabled || !_isInitialize) return;
            if (_touchDown == false) return;
            _touchDown = false;
            _transform.DOKill();
            _transform.DOScale(this.startScale, this.scaleBackTime).SetEase(this.scaleBackEasing);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.buttonEnabled || !_isInitialize) return;
            if (_touchDown) return;
            _touchDown = true;
            _transform.DOKill();
            _transform.DOScale(this.startScale + (this.startScale * this.scaleTotal), this.scaleTime).SetEase(this.scaleEaseing);
            this.onClick?.Invoke();
        }

        protected void OnDisable()
        {
            if (!this.buttonEnabled || !_isInitialize) return;
            _transform.DOKill();
            this.transform.localScale = this.startScale;
        }
    }
}
