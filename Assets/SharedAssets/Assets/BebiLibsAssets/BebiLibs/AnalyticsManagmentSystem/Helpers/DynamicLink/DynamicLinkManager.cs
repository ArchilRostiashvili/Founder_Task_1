using BebiLibs.Analytics;
using BebiLibs.Analytics.GameEventLogger;
using System.Diagnostics;
using UnityEngine;

namespace BebiLibs
{
    //Class is temporary not in use
    public class DynamicLinkManager : MonoBehaviour
    {
        private void InitializeDynamicLinks()
        {
#if IS_ACTIVE_FireBaseDynamic
            DynamicLinks.DynamicLinkReceived += this.OnDynamicLink;
#endif
        }

        private void OnDynamicLink(object sender, System.EventArgs args)
        {
#if IS_ACTIVE_FireBaseDynamic
            string fromID = null;
            string testID = null;

            ReceivedDynamicLinkEventArgs dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            string url = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString;
            if (dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString == null) return;

            string[] words = url.Split('?');
            if (words.Length == 2)
            {
                url = words[1];
                words = url.Split('&');
                string[] parts;
                for (int i = 0; i < words.Length; i++)
                {
                    parts = words[i].Split('=');
                    if (parts[0] == "f")
                        fromID = parts[1];
                    else
                    if (parts[0] == "t")
                        testID = parts[1];
                }
            }

            if (fromID == "s" || fromID == "sg")
            {
                if (PlayerPrefs.GetInt("IsFirstTime1", 0) == 0)
                {
                    PlayerPrefs.SetInt("IsFirstTime1", 1);
                    LogEvent("dl_share", "f", fromID);
                    SetUserProperty("dl", fromID);
                }
                return;
            }

            if (fromID == null || testID == null)
            {
                LogEvent("dl_er", "er", dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
                return;
            }

            if (PlayerPrefs.GetInt("IsFirstTime2", 0) == 0)
            {
                PlayerPrefs.SetInt("IsFirstTime2", 1);
                SetUserProperty("dl", fromID);
                Analytics_InstalledDynamicLink("dl_inst", fromID, testID);
            }
            else
            {
                Analytics_InstalledDynamicLink("dl_open", fromID, testID);
            }
#endif
        }

        public static System.Uri CreateDynamicLink(string url, string uriPrefix, string iosID, string androidID)
        {
#if IS_ACTIVE_FireBaseDynamic
            string userID = PlayerPrefs.GetString("UserID");
            var components = new DynamicLinkComponents(new System.Uri(url), uriPrefix)
            {
                IOSParameters = new IOSParameters(iosID),
                AndroidParameters = new AndroidParameters(androidID)
            };
            return components.LongDynamicLink;
#else
            return null;
#endif
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void Analytics_InstalledDynamicLink(string action, string fromID, string testID)
        {
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("aid", fromID);
            gameParameterBuilder.Add("tid", testID);
            AnalyticsManager.LogEvent(action, gameParameterBuilder.GetParameters());
        }
    }
}
