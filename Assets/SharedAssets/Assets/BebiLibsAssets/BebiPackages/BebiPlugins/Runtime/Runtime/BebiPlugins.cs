using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

using BebiLibs.GameApplicationConfig;

namespace BebiLibs
{
    public class BebiPlugins : MonoBehaviour
    {
        public static event System.Action BrowserCloseEvent;
        public static event System.Action BrowserLoadedEvent;

        private static BebiPluginConfig _PluginConfig;
        private static ApplicationConfig _ApplicationConfigs;
        private static PluginController _PluginController;
        private static bool _IsBrowserOpen = false;
        private static bool _IsInitialized = false;

        private void Awake()
        {
            _ApplicationConfigs = ApplicationConfigProvider.DefaultInstance().CurrentConfig;
            _PluginConfig = Resources.Load<BebiPluginConfig>("BebiPluginConfig");

            if (_ApplicationConfigs == null)
            {
                Debug.LogWarning("ApplicationConfig is null");
                return;
            }

            if (_PluginConfig == null)
            {
                Debug.LogWarning("BebiPluginData is null");
                return;
            }

            _PluginConfig.GroupID = _ApplicationConfigs.GetGroupID();
            _PluginConfig.FullGroupID = _ApplicationConfigs.GetFullGroupID();
            _PluginConfig.BundleID = _ApplicationConfigs.GetBundleID();


#if UNITY_EDITOR
            _PluginController = new EditorController();
#elif UNITY_ANDROID
            _PluginController = new AndroidPluginController();
#elif UNITY_IOS
            _PluginController = new IosPluginController();
#endif

            _PluginController.Initialize(_PluginConfig);
            _IsInitialized = _PluginConfig != null && _PluginController != null;
        }


        public void GetMessageFromPlugin(string message)
        {
            Debug.LogError("Message from Review " + message);
        }

        public void OnBrowserClosed(string status)
        {
            BrowserCloseEvent?.Invoke();
        }

        public static void RequestInAppReview()
        {
            _PluginController.RequestInAppReview();
        }

        public static void OpenURL(string url)
        {
            _IsBrowserOpen = true;
            if (_PluginConfig.UseDefaultBrowser)
            {
                Application.OpenURL(url);
                return;
            }

            _PluginController.OpenURL(url);
        }

        public static void CloseBrowser()
        {
            _PluginController.CloseBrowser();
            _IsBrowserOpen = false;
        }

        public static void WriteSharedString(string key, string value)
        {
            _PluginController.WriteSharedString(key, value);
        }

        public static string ReadSharedString(string key, string defaultValue)
        {
            return _PluginController.ReadSharedString(key, defaultValue);
        }

        //9247TGP5N5.group.com.educational.baby.games
        public static string ReadSharedString(string packageName, string key, string defaultValue)
        {
            return _PluginController.ReadSharedStringFromOtherApp(packageName, key, defaultValue);
        }

        public static bool IsAppInstalled(string packageName)
        {
            return _PluginController.IsAppInstalled(packageName);
        }


        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && _IsBrowserOpen)
            {
                _IsBrowserOpen = false;
                BrowserCloseEvent?.Invoke();
            }
            else if (pauseStatus && _IsBrowserOpen)
            {
                BrowserLoadedEvent?.Invoke();
            }
        }
    }
}
