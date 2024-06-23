

namespace BebiLibs
{
    public interface IAppsLauncher : IFeature
    {
        void Init();
        bool IsAppInstalled(string appPackage);
        bool IsAppEnabled(string appPackage);
        void GetApp(string appPackage);
        void LaunchApp(string appPackage);
    }
}