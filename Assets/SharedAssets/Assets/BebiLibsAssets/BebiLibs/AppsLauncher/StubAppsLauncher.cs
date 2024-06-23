using System;
using UnityEngine;

namespace BebiLibs
{
    public class StubAppsLauncher : IAppsLauncher, IAppsLauncherAsync
    {
        public void Init()
        {

        }

        public bool IsAppInstalled(string appPackage)
        {
            Common.DebugLog($"IsAppInstalled({appPackage})");
            return false;
        }

        public bool IsAppEnabled(string appPackage)
        {
            Common.DebugLog($"IsAppEnabled({appPackage})");
            return false;
        }

        public void GetApp(string appPackage)
        {
            Common.DebugLog($"GetApp({appPackage})");
        }

        public void LaunchApp(string appPackage)
        {
            Common.DebugLog($"LaunchApp({appPackage})");
        }

        public void IsAppInstalled(string appPackage, Action<bool> callback)
        {
            Common.DebugLog($"IsAppInstalled({appPackage}, callback<bool>)");
        }

        public void IsAppEnabled(string appPackage, Action<bool> callback)
        {
            Common.DebugLog($"IsAppEnabled({appPackage}, callback<bool>)");
        }

        public void GetApp(string appPackage, Action callback)
        {
            Common.DebugLog($"GetApp({appPackage}, callback)");
        }

        public void LaunchApp(string appPackage, Action callback)
        {
            Common.DebugLog($"LaunchApp({appPackage}, callback)");
        }
    }
}