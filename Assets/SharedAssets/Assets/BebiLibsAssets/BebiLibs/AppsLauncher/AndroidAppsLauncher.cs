using System;
using UnityEngine;

namespace BebiLibs
{
    public class AndroidAppsLauncher : IAppsLauncher, IAppsLauncherAsync
    {
#if UNITY_ANDROID
        private AndroidJavaClass _up;
        private AndroidJavaObject _ca;
        private AndroidJavaObject _packageManager;
        private AndroidJavaObject _launchIntent;
#endif
        public void Init()
        {
#if UNITY_ANDROID
            _up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _ca = _up.GetStatic<AndroidJavaObject>("currentActivity");
            _packageManager = _ca.Call<AndroidJavaObject>("getPackageManager");
            _launchIntent = null;
#endif
        }

        public bool IsAppInstalled(string bundleID)
        {
            if (bundleID == null || bundleID == "")
            {
                return false;
            }

#if UNITY_ANDROID
            try
            {
                _launchIntent = _packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);
            }
            catch (Exception ex)
            {
                Common.DebugLog("exception" + ex.Message);
            }

            if (_launchIntent == null)
            {
                return false;
            }

            return true;
#else
            return false;
#endif
        }

        public bool IsAppEnabled(string bundleID)
        {
            return true;
        }

        public void GetApp(string bundleID)
        {
            //Nativizer.Call("GetApp", appPackage);
        }

        public void LaunchApp(string bundleID)
        {
            if (bundleID == null || bundleID == "")
            {
                return;
            }

#if UNITY_ANDROID
            //Nativizer.Call("LaunchApp", appPackage);
            bool fail = false;
            //AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            //AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            //AndroidJavaObject launchIntent = null;
            try
            {
                _launchIntent = _packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);
            }
            catch (System.Exception)
            {
                fail = true;
            }

            if (fail)
            {
                Application.OpenURL($"market://details?id={bundleID}");
            }
            else
            {
                _ca.Call("startActivity", _launchIntent);
            }
#endif
        }

        public void IsAppInstalled(string appPackage, Action<bool> callback)
        {
            //Nativizer.Call("IsAppInstalled", appPackage, new NativizerCallbacks.NativizerBooleanCallback().setCallback(callback));
        }

        public void IsAppEnabled(string appPackage, Action<bool> callback)
        {
            //Nativizer.Call("IsAppEnabled", appPackage, new NativizerCallbacks.NativizerBooleanCallback().setCallback(callback));
        }

        public void GetApp(string appPackage, Action callback)
        {
            //Nativizer.Call("GetApp", appPackage, new NativizerCallbacks.NativizerVoidCallback().setCallback(callback));
        }

        public void LaunchApp(string appPackage, Action callback)
        {
            //Nativizer.Call("LaunchApp", appPackage, new NativizerCallbacks.NativizerVoidCallback().setCallback(callback));
        }
    }
}