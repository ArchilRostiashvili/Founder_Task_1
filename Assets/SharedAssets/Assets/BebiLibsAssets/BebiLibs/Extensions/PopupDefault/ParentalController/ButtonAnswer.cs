using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace BebiLibs
{
    public class ButtonAnswer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        public int answerValue;

        public float scaleTotal = 0.2f;
        public float scaleTime = 0.1f;
        public float scaleBackTime = 0.1f;
        public UnityEngine.UI.Button.ButtonClickedEvent onClick;
        public bool buttonEnabled = true;

        public Image Image_Back;

        public TextMeshProUGUI Text_Answer;

        private Image _imageInteraction;
        private float _scaleAmount;
        private float _scaleFrom;
        private bool _touchDown;
        private Transform _tr;
        private Coroutine _coroutine;

        void Start()
        {
            _imageInteraction = this.GetComponent<Image>();
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = this.buttonEnabled;
            }

            _scaleFrom = this.transform.localScale.y;
            _scaleAmount = 0.0f;
            _tr = this.gameObject.transform;
            _coroutine = null;
        }

        public void SetValue(int value)
        {
            this.answerValue = value;
            this.Text_Answer.text = value.ToString();
        }

        public void Enable(bool bl)
        {
            this.buttonEnabled = bl;
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = this.buttonEnabled;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this.buttonEnabled)
            {
                return;
            }

            if (_touchDown)
            { return; }

            _touchDown = true;
            if (_coroutine != null)
            {
                this.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = this.StartCoroutine(this.ScaleTo());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!this.buttonEnabled)
            {
                return;
            }

            if (!_touchDown)
            { return; }

            _touchDown = false;
            if (_coroutine != null)
            {
                this.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = this.StartCoroutine(this.ScaleBack());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.buttonEnabled)
            {
                return;
            }

            this.onClick.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.buttonEnabled)
            {
                return;
            }

            if (!_touchDown)
            { return; }

            _touchDown = false;
            if (_coroutine != null)
            {
                this.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = this.StartCoroutine(this.ScaleBack());
        }

        IEnumerator ScaleTo()
        {
            while (true)
            {
                _scaleAmount += this.scaleTotal * (Time.deltaTime / this.scaleTime);
                if (this.scaleTotal <= _scaleAmount)
                {
                    _scaleAmount = this.scaleTotal;
                    _tr.localScale = Vector3.one * (_scaleFrom + _scaleAmount);
                    _coroutine = null;
                    yield break;
                }
                _tr.localScale = Vector3.one * (_scaleFrom + _scaleAmount);
                yield return null;
            }
        }

        IEnumerator ScaleBack()
        {
            while (true)
            {
                _scaleAmount -= this.scaleTotal * (Time.deltaTime / this.scaleBackTime);
                if (_scaleAmount <= 0.0f)
                {
                    _scaleAmount = 0.0f;
                    _tr.localScale = Vector3.one * _scaleFrom;
                    _coroutine = null;
                    yield break;
                }
                _tr.localScale = Vector3.one * (_scaleFrom + _scaleAmount);
                yield return null;
            }
        }

        IEnumerator EnableButton()
        {
            yield return new WaitForSeconds(1.0f);
            this.Enable(true);
        }

        private void OnDestroy()
        {
            this.StopAllCoroutines();
        }
    }
}
