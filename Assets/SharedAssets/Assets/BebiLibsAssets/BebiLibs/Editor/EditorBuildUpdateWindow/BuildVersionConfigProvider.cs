using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomEditorBuildWindow
{
    public class BuildVersionConfigProvider
    {
        // Find Or Create Config Asset by type BuildVersionUpdatedConfig at Asset/Resources folder using UnityEditor AssetDatabase 
        public static BuildVersionUpdateConfig FindOrCreateAsset()
        {
            BuildVersionUpdateConfig asset = Resources.Load<BuildVersionUpdateConfig>("BuildVersionUpdatedConfig");
            if (asset == null)
            {
                Debug.Log("BuildVersionUpdatedConfig: Create new BuildVersionUpdatedConfig asset");
                asset = ScriptableObject.CreateInstance<BuildVersionUpdateConfig>();
                AutoConfigureAsset(asset);
                AssetDatabase.CreateAsset(asset, "Assets/Resources/BuildVersionUpdatedConfig.asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                SetDefaultBuildVersionUpdateHandlers(asset);
            }

            return asset;
        }

        //Configure BuildVersionUpdatedConfig BuildVersionDataList for Android and iOS
        private static void AutoConfigureAsset(BuildVersionUpdateConfig config)
        {
            SetDefaultBuildVersionTargets(config);
            SetDefaultBuildVersionUpdateHandlers(config);
        }

        private static void SetDefaultBuildVersionTargets(BuildVersionUpdateConfig config)
        {
            config.BuildVersionDataList.Clear();
            config.BuildVersionDataList.Add(new GameBuildVersionData()
            {
                BuildVersionName = "Android Build",
                AppBuildTarget = BuildTarget.Android,
                VersionString = PlayerSettings.bundleVersion,
                BuildNumber = PlayerSettings.Android.bundleVersionCode,
            });
            config.BuildVersionDataList.Add(new GameBuildVersionData()
            {
                BuildVersionName = "iOS Build",
                AppBuildTarget = BuildTarget.iOS,
                VersionString = PlayerSettings.bundleVersion,
                BuildNumber = int.Parse(PlayerSettings.iOS.buildNumber),
            });
        }

        public static void SetDefaultBuildVersionUpdateHandlers(BuildVersionUpdateConfig config)
        {
            config.BuildVersionHandlers.Clear();
            config.BuildVersionHandlers.Add(new BuildVersionHandler()
            {
                BuildTarget = BuildTarget.Android,
                BuildNumberGetter = () => PlayerSettings.Android.bundleVersionCode,
                BuildNumberSetter = (value) => PlayerSettings.Android.bundleVersionCode = value
            });

            config.BuildVersionHandlers.Add(new BuildVersionHandler()
            {
                BuildTarget = BuildTarget.iOS,
                BuildNumberGetter = () => int.Parse(PlayerSettings.iOS.buildNumber),
                BuildNumberSetter = (value) => PlayerSettings.iOS.buildNumber = value.ToString()
            });
        }
    }

}
