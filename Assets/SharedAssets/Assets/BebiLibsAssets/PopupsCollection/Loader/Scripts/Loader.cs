using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;
using BebiLibs.PopupManagementSystem;
using UnityEngine.UI;

namespace BebiLibs
{
    public class Loader : PopUpBase
    {
        public TextMeshProUGUI Text_Message;
        public TextMeshProUGUI Text_Error;
        public GameObject GO_CircleAnimation;

        [SerializeField] private Image _backgroundImage;
        [SerializeField] private LoaderData _defaultLoaderData;

        public LoaderData DefaultLoaderData => _defaultLoaderData;

        public override void Init()
        {
            TR_Content.gameObject.SetActive(false);
            _popupParentGO.gameObject.SetActive(false);
        }


        public static void Show(string message = "TEXT_LOADING", float time = 0.0f, string error = "", bool anim = true)
        {
            PopupManager.GetPopup<Loader>((popup) =>
            {
                LoaderData loaderData = new LoaderData(popup.DefaultLoaderData)
                {
                    message = message,
                    hideDelay = time,
                    error = error,
                    anim = anim
                };

                popup.ShowLoader(loaderData);
            });
        }

        public static void Show(LoaderData loaderData)
        {
            PopupManager.GetPopup<Loader>((popup) =>
            {
                popup.ShowLoader(loaderData);
            });
        }


        public static void SetMessage(string message, float time = 0.0f, string error = "", bool anim = true)
        {
            PopupManager.GetPopup<Loader>((popup) =>
            {
                popup.SetLoaderMessage(message, time, error, anim);
            });
        }

        public static void Hide()
        {
            PopupManager.GetPopup<Loader>((popup) =>
            {
                //Debug.Log("Hide Loader " + Time.frameCount);
                popup.HideLoader();
            });
        }


        public void ShowLoader(LoaderData loaderData)
        {
            // Debug.Log("Show Loader " + Time.frameCount);
            string message = loaderData.message;
            bool anim = loaderData.anim;
            string error = loaderData.error;
            float hideDelayTime = loaderData.hideDelay;

            if (LocalizationManager.TryGetTranslation(message, out string newMessage))
            {
                message = newMessage;
            }

            GO_CircleAnimation.SetActive(anim);

            _popupParentGO.gameObject.SetActive(true);
            TR_Content.gameObject.SetActive(true);

            Text_Message.text = message;
            Text_Error.text = error;

            if (hideDelayTime > 0)
            {
                StartCoroutine(DelayHide(hideDelayTime));
            }

            _backgroundImage.color = loaderData.backgroundColor;
        }

        public void SetLoaderMessage(string message, float time = 0.0f, string error = "", bool anim = true)
        {
            StopAllCoroutines();
            LoaderData loaderData = new LoaderData(_defaultLoaderData)
            {
                message = message,
                hideDelay = time,
                error = error,
                anim = anim
            };
            ShowLoader(loaderData);
        }

        public void HideLoader()
        {
            // Debug.Log("Hide Loader " + Time.frameCount);
            //Text_Message.DOKill();
            _popupParentGO.gameObject.SetActive(false);
            TR_Content.gameObject.SetActive(false);
            //Debug.Log("Hide Loader");
            StopAllCoroutines();
        }

        private IEnumerator DelayHide(float time)
        {
            yield return new WaitForSeconds(time);
            HideLoader();
            //Debug.Log("Delayed Hide Loader " + Time.frameCount);
        }
    }

    [System.Serializable]
    public class LoaderData
    {
        public string message = "TEXT_LOADING";
        public float hideDelay = 0.0f;
        public string error = "";
        public bool anim = true;
        public Color backgroundColor = new Color32(35, 194, 255, 255);

        public LoaderData()
        {

        }

        public LoaderData(LoaderData loaderData)
        {
            message = loaderData.message;
            hideDelay = loaderData.hideDelay;
            error = loaderData.error;
            anim = loaderData.anim;
            backgroundColor = loaderData.backgroundColor;
        }

        public static LoaderData Default()
        {
            return new LoaderData();
        }

        public static LoaderData OnlyColor(Color color, float delay)
        {
            return new LoaderData()
            {
                message = " ",
                hideDelay = delay,
                error = string.Empty,
                anim = false,
                backgroundColor = color
            };
        }
    }
}
