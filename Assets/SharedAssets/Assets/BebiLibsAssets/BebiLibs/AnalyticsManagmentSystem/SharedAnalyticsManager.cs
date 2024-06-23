using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using BebiLibs.Analytics.GameEventLogger;

namespace BebiLibs.Analytics
{
    public class SharedAnalyticsManager : AnalyticsHelperBase
    {

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LogAdsEvent(string eventName, string source)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(1);
            parameters.Add("category_name", source);
            AnalyticsManager.LogEvent(eventName, parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void RateUnlockEvent(string eventName, long activationTime = long.MinValue)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(1);
            if (activationTime != long.MinValue)
            {
                parameters.Add("sec", activationTime);
                AnalyticsManager.LogEvent(eventName, parameters);
            }
            else
            {
                AnalyticsManager.LogEvent(eventName);
            }
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void SetScene(string screenName, string screenClass = "MainClass")
        {
#if ACTIVATE_ANALYTICS
            GameParameterBuilder parameterBuilder = new GameParameterBuilder(1);
            AnalyticsManager.LogEvent(() => Firebase.Analytics.FirebaseAnalytics.EventScreenView, screenName, screenClass);
#endif
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void ShopOpen(string actionOrigin)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("bid", actionOrigin);
            AnalyticsManager.LogEvent("shop_show", parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void ShopClose(string actionOrigin, long activationTime = 0)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("bid", actionOrigin);
            AnalyticsManager.LogEvent("shop_close", parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void ShopClick(string actionOrigin, List<IGameParameter> extraParameters = null)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("bid", actionOrigin);

            if (extraParameters != null)
            {
                parameters.Add(extraParameters);
            }

            AnalyticsManager.LogEvent("click_shop", parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void ShopPassParental(string actionOrigin)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("bid", actionOrigin);
            AnalyticsManager.LogEvent("prnt_pass_shop", parameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopUpAd_Click(string action, string appID, string testID)
        {

            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);

            AnalyticsManager.LogEvent(action, parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopUpAd_InstallClick(string action, string appID, string testID, bool isInstalled)
        {


            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("inst", isInstalled ? 1 : 0);

            AnalyticsManager.LogEvent(action, parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopUpAd_InstallPass(string appID, string testID, string firstAppID, string firstAppTestID, bool isInstalled)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(5);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("f_aid", firstAppID);
            parameters.Add("f_tid", firstAppTestID);
            parameters.Add("inst", isInstalled ? 1 : 0);

            AnalyticsManager.LogEvent("p_ad_inst", parameters);
        }



        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopUpAd_Close(string appID, string testID, int closestate, int seconds)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(4);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("act", closestate);
            parameters.Add("sec", seconds);

            AnalyticsManager.LogEvent("p_ad_cl", parameters);

        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopUpAd_Change(string appID, string testID, string firstAppID, string firstAppTestID, int index)
        {

            GameParameterBuilder parameters = new GameParameterBuilder(5);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("f_aid", firstAppID);
            parameters.Add("f_tid", firstAppTestID);
            parameters.Add("ind", index);

            AnalyticsManager.LogEvent("p_ad_cha", parameters);
        }



        [Conditional("ACTIVATE_ANALYTICS")]
        public static void InstallSave(string appID, string testID, string from)
        {
            //UnityEngine.Debug.Log("----------- InstallSave  " + (testID + "|" + from));
            PlayerPrefs.SetString(appID, testID + "|" + from);
            PlayerPrefs.Save();
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void Installed(string appID, string testID, string from)
        {

            GameParameterBuilder parameters = new GameParameterBuilder(3);

            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("f", from);
            AnalyticsManager.LogEvent("inst_stats", parameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopupBannerShow(string popUp)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(1);
            parameters.Add("p", popUp);
            AnalyticsManager.LogEvent("show_app_banner", parameters);//popup_bad_show
        }



        [Conditional("ACTIVATE_ANALYTICS")]
        public static void PopupBannerIconClick(string appID, string testID, bool isInstalled, string popUp)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(4);

            parameters.Add("p", popUp);
            parameters.Add("aid", appID);
            parameters.Add("tid", testID);
            parameters.Add("inst", isInstalled ? 1 : 0);

            AnalyticsManager.LogEvent("p_b_c", parameters);//popup_bad_click
        }
    }
}