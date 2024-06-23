using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : PluginController
{

    public override void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public override void WriteSharedString(string key, string value)
    {
        //Debug.LogWarning("Write To: " + key + "_" + BebiPluginConfig.BundleID + " Value: " + value);
        PlayerPrefs.SetString(key + "_" + BebiPluginConfig.BundleID, value);
    }

    public override string ReadSharedString(string key, string defaultValue)
    {
        //Debug.LogWarning("Read From: " + key + "_" + BebiPluginConfig.BundleID);
        if (PlayerPrefs.HasKey(key + "_" + BebiPluginConfig.BundleID) == false)
        {
            PlayerPrefs.SetString(key + "_" + BebiPluginConfig.BundleID, defaultValue);
        }

        return PlayerPrefs.GetString(key + "_" + BebiPluginConfig.BundleID, defaultValue);
    }

    public override string ReadSharedStringFromOtherApp(string packageName, string key, string defaultValue)
    {
        if (PlayerPrefs.HasKey(key + "_" + packageName) == false)
        {
            PlayerPrefs.SetString(key + "_" + packageName, defaultValue);
        }

        //Debug.LogWarning("Read Shared String From: " + key + "_" + packageName);
        return PlayerPrefs.GetString(key + "_" + packageName, defaultValue);
    }

    public override void RequestInAppReview()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + BebiPluginConfig.BundleID);
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/app/id" + BebiPluginConfig.BundleID);
#endif
    }

    public override void CloseBrowser()
    {

    }

    public override void OnApplicationPause(bool pauseStatus)
    {

    }

    public override bool IsAppInstalled(string bundleID)
    {
        return BebiPluginConfig.IsAppInstalledEditor;
    }
}
