using System;
using UnityEngine;

namespace BebiLibs
{
    public class IosAppsLauncher : IAppsLauncher, IAppsLauncherAsync
    {
        public void Init()
        {

        }

        public bool IsAppInstalled(string appPackage)
        {
            if (appPackage == null || appPackage == "")
            {
                return false;
            }
            int value = 0;
#if UNITY_IOS
            value = _IOSCheckIfThereIsAppWithScheme(appPackage);
#endif

            return value == 1;
            //throw new NotImplementedException();
        }

        public bool IsAppEnabled(string appPackage)
        {
            return true;
        }

        public void GetApp(string appPackage)
        {
            appPackage = appPackage.Replace("color", "");

            string url = "itms-apps://itunes.apple.com/app/viewContentsUserReviews?id=" + appPackage;
            Application.OpenURL(url);
            //throw new NotImplementedException();
        }

        public void LaunchApp(string appPackage)
        {
            if (appPackage == null || appPackage == "")
            {
                return;
            }

            Application.OpenURL(appPackage + "://");
        }

        public void IsAppInstalled(string appPackage, Action<bool> callback)
        {
            int value = 0;
#if UNITY_IOS
            value = _IOSCheckIfThereIsAppWithScheme(appPackage);
#endif
            callback(value == 1);
        }

        public void IsAppEnabled(string appPackage, Action<bool> callback)
        {
            callback(true);
        }

        public void GetApp(string appPackage, Action callback)
        {
            throw new NotImplementedException();
        }

        public void LaunchApp(string appPackage, Action callback)
        {
            throw new NotImplementedException();
        }

#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int _IOSCheckIfThereIsAppWithScheme(string url);
#endif
    }
}