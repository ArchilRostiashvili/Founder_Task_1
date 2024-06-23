using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Analytics;
using BebiLibs.PopupManagementSystem;
using BebiLibs.GameApplicationConfig;
using BebiLibs.Analytics.GameEventLogger;

namespace BebiLibs
{
    public class SharePopup : PopUpBase
    {
        public static SharePopup Instance;

        public ButtonScale[] arrayButtons;
        private NativeShare _nativeShare;
        private bool _shareClicked;
        private static string _Source = string.Empty;

        override public void Init()
        {
            base.Init();
        }

        public static void Activate(string source)
        {
            _Source = source;
            if (Instance == null)
            {
                PopupManager.GetPopup<SharePopup>(popup =>
                {
                    Instance = popup;
                    Instance.Show(true);
                });
            }
            else
            {
                Instance.Show(true);
            }
        }

        override public void Show(bool anim)
        {
            if (ManagerApp.Instance != null)
            {
                _nativeShare = new NativeShare();
#if UNITY_IOS
                _nativeShare.SetText(ApplicationConfigProvider.DefaultInstance().CurrentConfig.GetAppShareURl());
#elif UNITY_ANDROID
                _nativeShare.SetText(ApplicationConfigProvider.DefaultInstance().CurrentConfig.GetAppShareURl());
#endif
            }

            AnalyticsManager.LogEvent("show_share", "bid", _Source);

            base.Show(anim);
            _shareClicked = false;
            EnableButtons(true);

            ManagerSounds.PlayEffect("fx_page15");
        }

        public void Trigger_ButtonClick_Share(GameObject go)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(8);
            parameters.Add("id", go.name.Split('_')[1]);
            parameters.Add("bid", _Source);

            AnalyticsManager.LogEvent("click_share", parameters);//pop_share_click

            //EnableButtons(false);

            _shareClicked = true;
            _nativeShare.Share();
        }

        private void OnApplicationFocus(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (_shareClicked)
                {
                    Hide(false);
                }
            }
            else
            {

            }
        }

        private void EnableButtons(bool bl)
        {
            for (int i = 0; i < arrayButtons.Length; i++)
            {
                arrayButtons[i].buttonEnabled = bl;
            }
        }

        public override void Hide(bool anim)
        {
            base.Hide(anim);
            RatePopup.Activate(RatePopup.AUTO, "auto");
        }

        public override void Trigger_ButtonClick_Close()
        {
            AnalyticsManager.LogEvent("close_share", "bid", _Source);
            ManagerSounds.PlayEffect("fx_page17");
            base.Trigger_ButtonClick_Close();
        }
    }
}
