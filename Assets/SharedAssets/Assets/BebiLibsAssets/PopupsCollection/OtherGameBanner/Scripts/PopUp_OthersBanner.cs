using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BebiLibs.Analytics;

namespace BebiLibs
{
    public class PopUp_OthersBanner : PopUpBase
    {
        public ButtonOtherApp[] arrayButtons;

        public Vector3 startPosition = Vector3.zero;
        private string _parentPopupID;

        public void Activate(List<DataOtherApp> arrayDataOtherApp, string popupID)
        {
            _parentPopupID = popupID;
            if (0 < arrayDataOtherApp.Count)
            {
                gameObject.SetActive(true);

                startPosition = startPosition == Vector3.zero ? TR_Content.position : startPosition;
                //TR_Content.localPosition = new Vector3(0.0f, -580.0f, 0.0f);
                //TR_Content.DOLocalMoveY(0.0f, 0.2f);
                //Debug.Log(startPosition);
                TR_Content.position = new Vector3(startPosition.x, -580.0f, startPosition.z);
                TR_Content.DOMoveY(startPosition.y, 0.2f);

                ManagerSounds.PlayEffect("fx_swoosh2");
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }

            List<DataOtherApp> arrayTempDataOtherApp = new List<DataOtherApp>();
            for (int i = 0; i < arrayDataOtherApp.Count; i++)
            {
                if (!arrayDataOtherApp[i].IsInstalled)
                {
                    arrayTempDataOtherApp.Add(arrayDataOtherApp[i]);
                }
            }

            for (int i = 0; i < arrayDataOtherApp.Count; i++)
            {
                if (arrayDataOtherApp[i].IsInstalled)
                {
                    arrayTempDataOtherApp.Add(arrayDataOtherApp[i]);
                }
            }

            for (int i = 0; i < arrayButtons.Length; i++)
            {
                if (i < arrayTempDataOtherApp.Count)
                {
                    arrayButtons[i].SetData(arrayTempDataOtherApp[i]);
                }
                else
                {
                    arrayButtons[i].SetData(null);
                }
            }

            SharedAnalyticsManager.PopupBannerShow(_parentPopupID);
        }

        public override void Show(bool anim)
        {

        }

        public override void Hide(bool anim = false)
        {
            gameObject.SetActive(false);
        }

        public void Trigger_ButtonClick_Banner(ButtonOtherApp button)
        {
            SharedAnalyticsManager.PopupBannerIconClick(button.CurrentData.app_id, button.CurrentData.test_id, button.CurrentData.IsInstalled, _parentPopupID);
            ManagerSounds.PlayEffect("fx_gameready18");

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopUp_Alert.Activate("TEXT_NO_INTERNET_CONNECTION");
            }

#if UNITY_IOS
           ProcessGoToApp(button.CurrentData, false, "b");
#elif UNITY_ANDROID
            ProcessGoToApp(button.CurrentData, false, "b");
#endif
        }

        public static void ProcessGoToApp(DataOtherApp dataApp, bool dynamicLink, string from)
        {
            bool internetOn = Application.internetReachability != NetworkReachability.NotReachable;

            if (!dataApp.IsInstalled && internetOn)
            {
                SharedAnalyticsManager.InstallSave(dataApp.app_id, dataApp.test_id, from);
            }

            if (dynamicLink && internetOn)
            {
                System.Uri url = DynamicLinkManager.CreateDynamicLink("http://bebi.family/?f=" + dataApp.app_id + "&t=" + dataApp.test_id, "https://bebi" + dataApp.app_id + ".page.link", dataApp.id_ios, dataApp.id_android);
                Application.OpenURL(url.ToString());
            }
            else
            {
                if (internetOn)
                {
#if UNITY_IOS
                        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + dataApp.id_ios);
#elif UNITY_ANDROID
                    Application.OpenURL($"market://details?id={dataApp.id_android}");
#endif
                }

            }
        }
    }
}
