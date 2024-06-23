using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidPluginController : PluginController
{
    public AndroidJavaObject UnityContext;
    public AndroidJavaObject UnityActivity;
    public AndroidJavaObject PackageManager;
    public AndroidJavaObject BebiPluginInstance;

    public override void Initialize(BebiPluginConfig pluginData)
    {
        base.Initialize(pluginData);
        GetAndroidPluginInstance();
        GetAndroidPluginContexts();
        InitializeAndroidPlugin();
    }

    private void GetAndroidPluginInstance()
    {
        try
        {
            AndroidJavaClass bebiPluginClass = new AndroidJavaClass("com.bebi.family.bebiplugin.BebiPluginController");
            BebiPluginInstance = bebiPluginClass.CallStatic<AndroidJavaObject>("GetInstance");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unable To access bebi plugin java instance, error: " + e);
        }
    }

    private void GetAndroidPluginContexts()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            UnityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            UnityContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");
            PackageManager = UnityActivity.Call<AndroidJavaObject>("getPackageManager");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unable To access bebi plugin java instance, error: " + e);
        }
    }

    public void InitializeAndroidPlugin()
    {
        if (BebiPluginInstance == null)
        {
            Debug.LogWarning("BebiPluginInstance is null");
            return;
        }

        BebiPluginInstance.Call("Initialize", UnityContext, UnityActivity);
    }

    public override void CloseBrowser()
    {

    }

    public override void OpenURL(string url)
    {
        if (BebiPluginInstance != null)
        {
            BebiPluginInstance.Call("OpenURL", url, UnityContext, UnityActivity);
        }
        else
        {
            Application.OpenURL(url);
        }
    }

    public override void WriteSharedString(string key, string value)
    {
        if (BebiPluginInstance == null)
        {
            Debug.LogWarning("BebiPluginInstance is null");
            return;
        }

        BebiPluginInstance.Call("WriteSharedString", "container", key, value, UnityContext, UnityActivity);
    }

    public override string ReadSharedString(string key, string defaultValue)
    {
        if (BebiPluginInstance == null)
        {
            return string.Empty;
        }

        return BebiPluginInstance.Call<string>("ReadSharedString", "container", key, defaultValue, UnityContext, UnityActivity);
    }

    public override string ReadSharedStringFromOtherApp(string packageName, string key, string defaultValue)
    {
        if (BebiPluginInstance == null)
        {
            return string.Empty;
        }

        return BebiPluginInstance.Call<string>("ReadSharedStringFromOtherApp", packageName, "container", key, defaultValue, UnityContext, UnityActivity);
    }

    public override void RequestInAppReview()
    {
        if (BebiPluginInstance == null)
        {
            Debug.LogWarning("BebiPluginInstance is null");
            return;
        }

        BebiPluginInstance.Call("RequestInAppReview", UnityContext, UnityActivity);
    }

    public override bool IsAppInstalled(string packageName)
    {
        if (BebiPluginInstance == null)
        {
            Debug.LogWarning("BebiPluginInstance is null");
            return false;
        }

        return BebiPluginInstance.Call<bool>("IsPackageInstalled", packageName, UnityContext, UnityActivity);
    }

    public override void OnApplicationPause(bool pauseStatus)
    {

    }

}
