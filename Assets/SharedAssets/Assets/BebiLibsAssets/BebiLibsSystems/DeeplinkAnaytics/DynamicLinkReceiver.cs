using System;
using System.Collections;
using System.Collections.Generic;
#if ACTIVATE_FIREBASE
using Firebase.DynamicLinks;
#endif
using UnityEngine;
using BebiLibs;
using BebiLibs.Analytics;


//TextMeshPro;ACTIVATE_FIREBASE;ACTIVATE_ANALYTICS;ACTIVATE_REMOTE_CONFIG;UNITY_PURCHASING
public class DynamicLinkReceiver : MonoBehaviour
{
    public delegate void DynamicLinkReceivedDelegate(List<string> dynamicLinkUrls, Dictionary<string, string> parsedDynamicLinkData);
    private static Dictionary<string, string> _parsedDynamicLinkData = new Dictionary<string, string>();
    private static List<string> _dynamicLinkUrlList = new List<string>();
    private static event DynamicLinkReceivedDelegate DynamicLinkReceiverEvent;

    [Header("Editor: ")]
    [SerializeField] internal bool _logInEditor;

    private void Awake()
    {
        FirebaseDependencyResolver.AddInitializationListener(OnFirebaseInitialize, true);
    }

    private void OnFirebaseInitialize(bool isFirebaseAvailable)
    {
#if ACTIVATE_FIREBASE
        if (isFirebaseAvailable)
        {
            DynamicLinks.DynamicLinkReceived += OnDynamicLinkReceive;
        }
        else
        {
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "Unable To Register As Firebase Deep Link Listener");
        }
#endif
    }

    private void OnDynamicLinkReceive(object sender, EventArgs args)
    {
#if ACTIVATE_FIREBASE
        if (args is not ReceivedDynamicLinkEventArgs dynamicLinkEventArgs)
        {
            Debug.LogWarning("Received Dynamic Link Arguments is null");
            return;
        }

        string url = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString;

        Dictionary<string, string> gameURl = DeepLinkReceiver.GetLinkParamsFromURL(url);
        LogReceivedDynamicLinkData(gameURl);
        AppendDynamicLinkUrls(url);
        UpdateDynamicLinkData(gameURl);
        DynamicLinkReceiverEvent?.Invoke(_dynamicLinkUrlList, _parsedDynamicLinkData);
#endif
    }

    private void AppendDynamicLinkUrls(string newUrl)
    {
        if (_dynamicLinkUrlList.Contains(newUrl) == false)
        {
            _dynamicLinkUrlList.Add(newUrl);
        }
    }

    private void UpdateDynamicLinkData(Dictionary<string, string> newDynamicLinkData)
    {
        foreach (var item in newDynamicLinkData)
        {
            if (!_parsedDynamicLinkData.ContainsKey(item.Key))
            {
                _parsedDynamicLinkData.Add(item);
            }
            else
            {
                _parsedDynamicLinkData[item.Key] = item.Value;
            }
        }
    }

    private void LogReceivedDynamicLinkData(Dictionary<string, string> gameURl)
    {
        if (_logInEditor && Debug.isDebugBuild)
        {
            foreach (var item in gameURl)
            {
                Debug.Log($"Parameter ID: {item.Key}, Data: {item.Value}");
            }
        }
    }

    public static void AddReceiveListener(DynamicLinkReceivedDelegate onDynamicLinkInitialized, bool immediate)
    {
        DynamicLinkReceiverEvent += onDynamicLinkInitialized;

        if (immediate && _parsedDynamicLinkData.Count > 0)
        {
            onDynamicLinkInitialized?.Invoke(_dynamicLinkUrlList, _parsedDynamicLinkData);
        }
    }

    public static void RemoveReceiveListener(DynamicLinkReceivedDelegate onDynamicLinkInitialized)
    {
        DynamicLinkReceiverEvent -= onDynamicLinkInitialized;
    }
}
