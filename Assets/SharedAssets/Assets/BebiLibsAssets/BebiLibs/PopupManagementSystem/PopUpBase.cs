using BebiLibs.FacadeCommand;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

//#define ADMOB_ACTIVE

namespace BebiLibs
{
    public abstract class PopUpBase : MonoBehaviour
    {
        public System.Action CallBack_OnScaleEnds;

        [Header("Basic Popup Components:")]
        [SerializeField] protected string _popupID;
        [SerializeField] protected Image Image_Back;
        [SerializeField] protected Canvas _popUpCanvas;
        [SerializeField] protected GameObject _popupParentGO;
        [SerializeField] protected Transform TR_Content;


        [Header("Basic Animation Parameters:")]
        [SerializeField] private bool _useAnimationCurve = false;

        [HideField("_useAnimationCurve", false)]
        [SerializeField] private Ease easing = Ease.Linear;
        [HideField("_useAnimationCurve", true)]
        [SerializeField] private AnimationCurve _animationCurve;

        [SerializeField] private float showTime = 0.1f;
        [SerializeField] private float _delayBeforeStart = 0f;
        [SerializeField] private float _startAnimationScale = 0;
        [SerializeField] private float hideTime = 0.1f;

        [HideInInspector] protected float _alphaBackDefault = 0.4f;
        [HideInInspector] protected float _scaleDefault = 1.0f;

        protected bool _isInitialized = false;


        public string PopupID => _popupID;
        public bool IsInitialized => _isInitialized;

        public virtual void Init()
        {
            gameObject.SetActive(false);

            if (_popUpCanvas != null)
                _popUpCanvas = GetComponent<Canvas>();

            if (Image_Back != null)
                _alphaBackDefault = Image_Back.color.a;

            if (TR_Content != null)
                _scaleDefault = TR_Content.localScale.y;

            _isInitialized = true;
        }

        public virtual void Show(bool anim)
        {
            _popupParentGO.SetActive(true);
            gameObject.SetActive(true);

            if (anim)
            {
                SetBackColorAndActivate(0.0f);
                DoBackFadeOutAnimation();

                SetLocalScaleAndActivate(_startAnimationScale);
                DoTRContentScaleAnimation();
            }
            else
            {
                SetBackColorAndActivate(_alphaBackDefault);
                SetLocalScaleAndActivate(_scaleDefault);
            }

        }

        private void SetLocalScaleAndActivate(float scale)
        {
            TR_Content.gameObject.SetActive(true);
            TR_Content.localScale = Vector3.one * scale;
        }

        private void SetBackColorAndActivate(float alpha = 0.0f)
        {
            if (Image_Back != null)
            {
                Image_Back.gameObject.SetActive(true);
                Color colorBack = Image_Back.color;
                colorBack.a = alpha;
                Image_Back.color = colorBack;
            }
        }

        private void DoBackFadeOutAnimation()
        {
            if (Image_Back != null)
            {
                Image_Back.DOFade(_alphaBackDefault, showTime).SetDelay(_delayBeforeStart);
            }
        }


        private void DoTRContentScaleAnimation()
        {
            var tween = TR_Content.DOScale(_scaleDefault, showTime).SetDelay(_delayBeforeStart).OnComplete(() =>
            {
                CallBack_OnScaleEnds?.Invoke();
            });

            SetEasingMode(tween);
        }

        private void SetEasingMode(TweenerCore<Vector3, Vector3, VectorOptions> tween)
        {
            if (_useAnimationCurve)
            {
                tween.SetEase(_animationCurve);
            }
            else
            {
                tween.SetEase(easing);
            }
        }

        virtual public void Hide(bool anim)
        {
            if (anim)
            {
                DoHideAnimation();
            }
            else
            {
                if (Image_Back != null)
                {
                    Image_Back.gameObject.SetActive(false);
                }

                TR_Content.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
#if ADMOB_ACTIVE
    if(anim)
    {
        ManagerAds.ShowInterstitial();
    }
#endif
        }

        private void DoHideAnimation()
        {
            if (Image_Back != null)
            {
                Image_Back.DOFade(0.0f, hideTime);
            }

            TR_Content.DOScale(0.0f, hideTime).OnComplete(() =>
            {
                Image_Back.gameObject.SetActive(false);
                TR_Content.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        virtual public void Trigger_ButtonClick_Close()
        {
            Hide(false);
        }
    }
}
