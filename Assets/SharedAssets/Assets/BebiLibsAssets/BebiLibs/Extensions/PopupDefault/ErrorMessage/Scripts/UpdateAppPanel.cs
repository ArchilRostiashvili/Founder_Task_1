using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.Analytics;

namespace BebiLibs
{
    public class UpdateAppPanel : ErrorMessagePanel
    {
        [Header("apple app store")]
        public string appStoreName = "App Store";
        public Color appStoreColor;

        [Header("google play store")]
        public string playStoreName = "Play Store";
        public Color playStoreColor;

        public string googleURL = "https://play.google.com/store/apps/details?id=bebi.family.kids.learning.games";
        public string appleURL = "https://apps.apple.com/us/app/bebi-toddlers-puzzle-games/id1571483537";

        public override void Trigger_ButtonClick_ButtonSubmit()
        {
            AnalyticsManager.LogEvent("update_click");
#if UNITY_ANDROID
            Application.OpenURL(this.googleURL);
#elif UNITY_IOS
            Application.OpenURL(this.appleURL);
#endif
            //base.Trigger_ButtonClick_ButtonSubmit();
        }

        internal override void Show(string error)
        {
            base.Show(error);
        }

        public override string ProcessContentText()
        {
            //return base.ProcessContentText();
            if (!LocalizationManager.TryGetTranslation(_contentText, out string message))
            {
                message = _contentText;
            }

            message = string.Format(message, this.GetColorString(), this.GetStoreName());
            return message;
        }

        public string GetColorString()
        {
            Color color;
#if UNITY_ANDROID
            color = this.playStoreColor;
#elif UNITY_IOS
            color = this.appStoreColor;
#endif
            //return "#" + ColorUtility.ToHtmlStringRGBA(color);
            return null;
        }

        public string GetStoreName()
        {
#if UNITY_ANDROID
            return this.playStoreName;
#elif UNITY_IOS
            return this.appStoreName;
#endif
        }
    }
}
