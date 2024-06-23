using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PluginController
{
    public BebiPluginConfig BebiPluginConfig;

    public virtual void Initialize(BebiPluginConfig pluginData)
    {
        BebiPluginConfig = pluginData;
    }

    public void GetMessageFromPlugin(string message)
    {
        Debug.LogError("Message from Review " + message);
    }

    public virtual void OnBrowserClosed(string status) { }
    public abstract void RequestInAppReview();
    public abstract void OpenURL(string url);
    public abstract void CloseBrowser();

    public abstract void WriteSharedString(string key, string value);
    public abstract string ReadSharedStringFromOtherApp(string packageName, string key, string defaultValue);
    public abstract string ReadSharedString(string key, string defaultValue);

    public abstract void OnApplicationPause(bool pauseStatus);
    public abstract bool IsAppInstalled(string bundleID);
}
