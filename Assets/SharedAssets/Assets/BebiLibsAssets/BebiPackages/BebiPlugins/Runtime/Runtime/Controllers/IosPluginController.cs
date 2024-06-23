using BebiLibs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class IosPluginController : PluginController
{
    public override void Initialize(BebiPluginConfig pluginData)
    {
        base.Initialize(pluginData);
        Debug.Log("Initialize Sharing System With ID " + pluginData.GroupID);
        UserDefaultView.InitSharingSystem(pluginData.GroupID);
    }

    public override void OpenURL(string url)
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SFSafariView.LaunchUrl(url);
    }

    public override void CloseBrowser()
    {
        SFSafariView.Dismiss();
    }

    public override void WriteSharedString(string key, string value)
    {
        Debug.Log("Writing Into Key: " + key + "_" + BebiPluginConfig.BundleID);
        UserDefaultView.WriteSharedString(key + "_" + BebiPluginConfig.BundleID, value);
    }

    public override string ReadSharedString(string key, string defaultValue)
    {
        Debug.Log("Reading From Key: " + key + "_" + BebiPluginConfig.BundleID);
        return UserDefaultView.ReadSharedString(key + "_" + BebiPluginConfig.BundleID, defaultValue);
    }

    public override string ReadSharedStringFromOtherApp(string bundleID, string key, string defaultValue)
    {
        Debug.Log("Reading Shared Value From Key: " + key + "_" + bundleID);
        return UserDefaultView.ReadSharedString(key + "_" + bundleID, defaultValue);
    }

    public override void RequestInAppReview()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }

    public override bool IsAppInstalled(string bundleID)
    {
        return CheckAppInstallation.IsAppInstalled(bundleID + "://");
    }

    public override void OnApplicationPause(bool pauseStatus)
    {

    }

}
