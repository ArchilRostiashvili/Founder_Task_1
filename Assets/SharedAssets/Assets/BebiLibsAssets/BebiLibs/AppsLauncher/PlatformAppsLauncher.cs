namespace BebiLibs
{
    public class PlatformAppsLauncher : IAppsLauncher
    {
        private IAppsLauncher _actualAppsLauncher;

        public void Init()
        {

        }

        public bool IsAppInstalled(string appPackage)
        {
            return _actualAppsLauncher.IsAppInstalled(appPackage);
        }

        public bool IsAppEnabled(string appPackage)
        {
            return _actualAppsLauncher.IsAppEnabled(appPackage);
        }

        public void GetApp(string appPackage)
        {
            _actualAppsLauncher.GetApp(appPackage);
        }

        public void LaunchApp(string appPackage)
        {
            _actualAppsLauncher.LaunchApp(appPackage);
        }

        public PlatformAppsLauncher()
        {
#if UNITY_EDITOR
            _actualAppsLauncher = new StubAppsLauncher();
#elif UNITY_ANDROID
            _actualAppsLauncher = new AndroidAppsLauncher();
#elif UNITY_IOS
            _actualAppsLauncher = new IosAppsLauncher();
#else
            _actualAppsLauncher = new StubAppsLauncher();
#endif

            _actualAppsLauncher.Init();
        }
    }
}