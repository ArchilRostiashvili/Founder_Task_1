using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.UI.Button;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
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

        public bool isPaused;

        private Image _imageInteraction;
        private bool _touchDown;
        private Transform _transform;

        private Vector3 _startScale = Vector3.one;

        private bool _isInitialize = false;
        private bool _isHitDown = false;
        private bool _isClicked = false;

        public Vector2 position => transform.position;
        public Vector2 localPosition => transform.localPosition;

        protected void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            if (!_isInitialize)
            {
                _transform = GetComponent<Transform>();
                _startScale = _transform.localScale;
            }
            _imageInteraction = GetComponent<Image>();
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = enabled;
            }
            _isInitialize = true;
            _isClicked = false;
        }

        public void SetImage(Sprite sprite)
        {
            if (_imageInteraction != null)
            {
                _imageInteraction.sprite = sprite;
            }
        }
        public void Enable(bool bl)
        {
            buttonEnabled = bl;
            if (_imageInteraction != null)
            {
                _imageInteraction.raycastTarget = buttonEnabled;
            }
            _isClicked = false;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _isHitDown = true;
            if (!buttonEnabled || !_isInitialize) return;
            if (_touchDown) return;

            _isClicked = false;
            _touchDown = true;
            _transform.DOKill();
            _transform.localScale = _startScale;
            _transform.DOScale(_startScale + (_startScale * scaleTotal), scaleTime).SetEase(scaleEaseing);
        }

        public virtual bool IsPointClick(System.Action callBack)
        {
            if (!_isInitialize || !_isHitDown) return false;
            _isHitDown = false;
            _touchDown = false;

            _transform.DOKill();
            _transform.localScale = _startScale;
            _transform.DOPunchScale(Vector3.one * scaleTotal, scaleTime, 1, 0).SetEase(scaleEaseing).OnComplete(() =>
           {
               callBack?.Invoke();
           });
            return true;
        }

        public void SetActive(bool value)
        {
            this.gameObject.SetActive(value);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
            ScaleToDefault();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
            ScaleToDefault();
        }

        private void ScaleToDefault()
        {
            if (isPaused)
            {
                _touchDown = false;
                _isClicked = false;
                transform.localScale = _startScale;
                transform.DOKill();
            }
        }

        public virtual void Bounce()
        {
            _transform.DOKill();
            _transform.localScale = _startScale;
            _transform.DOScale(_startScale + (_startScale * scaleTotal), scaleTime).SetEase(scaleEaseing);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!buttonEnabled || !_isInitialize || _isClicked) return;
            if (!_touchDown) return;
            _touchDown = false;
            _isClicked = false;
            _transform.DOKill();
            _transform.DOScale(_startScale, scaleBackTime + 0.2f).SetEase(scaleBackEasing);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!buttonEnabled)
            {
                return;
            }
            _isClicked = true;
            onClick?.Invoke();
            //transform.localScale = startScale;
        }

        protected void OnDisable()
        {
            if (!buttonEnabled || !_isInitialize) return;
            _transform.DOKill();
            transform.localScale = _startScale;
            _isClicked = false;
        }

        private void OnDestroy()
        {
            if (_transform != null)
                _transform.DOKill();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonEnabled || !_isInitialize || _isClicked) return;
            if (!_touchDown) return;

            _isClicked = false;
            _touchDown = false;
            _transform.DOKill();
            _transform.DOScale(_startScale, scaleBackTime).SetEase(scaleBackEasing);
        }


        public void SetStartScale(Vector3 startScale)
        {
            _startScale = startScale;
        }


#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Button Scale", false, 10)]
        static void CreateButtonScale(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Button_Scale");
            go.AddComponent<Image>().sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (!go.TryGetComponent<RectTransform>(out RectTransform rect))
            {
                go.AddComponent<RectTransform>();
            }
            go.AddComponent<ButtonScale>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/Button Scale - Text", false, 10)]
        static void CreateButtonScale_Text(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Button_Scale");
            go.AddComponent<Image>().sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            GameObject text = new GameObject("Text");
            if (!text.TryGetComponent<RectTransform>(out RectTransform rect))
            {
                text.AddComponent<RectTransform>();
            }
            text.AddComponent<TMPro.TextMeshPro>().text = "New Text";
            text.transform.SetParent(go.transform);
            if (!go.TryGetComponent<RectTransform>(out RectTransform rect1))
            {
                go.AddComponent<RectTransform>();
            }
            go.AddComponent<ButtonScale>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif
    }
}
