using System;

namespace BebiLibs
{
    public interface IAppsLauncherAsync : IFeature

    {
    void IsAppInstalled(string appPackage, Action<bool> callback);
    void IsAppEnabled(string appPackage, Action<bool> callback);
    void GetApp(string appPackage, Action callback);
    void LaunchApp(string appPackage, Action callback);
    }
}