using System;

namespace BebiLibs
{
    public class PlatformAppsLauncherAsync : IAppsLauncherAsync
    {
        private IAppsLauncherAsync actualAppsLauncher;

        public void IsAppInstalled(string appPackage, Action<bool> callback)
        {
            actualAppsLauncher.IsAppInstalled(appPackage, callback);
        }

        public void IsAppEnabled(string appPackage, Action<bool> callback)
        {
            actualAppsLauncher.IsAppEnabled(appPackage, callback);
        }

        public void GetApp(string appPackage, Action callback)
        {
            actualAppsLauncher.GetApp(appPackage, callback);
        }

        public void LaunchApp(string appPackage, Action callback)
        {
            actualAppsLauncher.LaunchApp(appPackage, callback);
        }

        public PlatformAppsLauncherAsync()
        {
            #if UNITY_EDITOR
                actualAppsLauncher = new StubAppsLauncher();
            #elif UNITY_ANDROID
                actualAppsLauncher = new StubAppsLauncher();
            #elif UNITY_IOS
                actualAppsLauncher = new StubAppsLauncher();
            #else
                actualAppsLauncher = new StubAppsLauncher();
            #endif
        }
    }
}