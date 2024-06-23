using BebiLibs;
using BebiLibs.Analytics;
using BebiLibs.Analytics.GameEventLogger;
using DynamicAnalyticsLogger;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GamePromotionLogger : MonoBehaviour
{
    public const string DYNAMIC_LINK_PARAM_ID = "linkData";
    public const string REPEAT_KEY = "_repeat";
    public const string DYNAMIC_LINK_PROPERTY_KEY = "DL_P_";
    public const string DYNAMIC_LINK_EVENT_KEY = "DL_E_";

    private PersistentBoolean _isTestAnalyticsDataSend = new PersistentBoolean("DynamicLinkTestAnalyticsDataSent", false);
    private string _lastEventID = string.Empty;
    public DynamicLinkData DynamicLinkData;

    private void Awake()
    {
        DynamicLinkReceiver.AddReceiveListener(OnDynamicLinkReceived, true);
        DeepLinkReceiver.AddReceiveListener(OnDeepLinkReceived, true);
    }

    private void OnDeepLinkReceived(List<DeepLinkData> allDeepLinkUrls, DeepLinkData recededDeepLinkUrl)
    {
        OnDeepLinkReceived(new List<string>(), recededDeepLinkUrl.ParameterDictionary);
    }

    private void OnDeepLinkReceived(List<string> dynamicLinkUrls, Dictionary<string, string> parsedDynamicLinkData)
    {
        OnLinkReceived(dynamicLinkUrls, parsedDynamicLinkData);
    }

    private void OnDynamicLinkReceived(List<string> dynamicLinkUrls, Dictionary<string, string> parsedDynamicLinkData)
    {
        OnLinkReceived(dynamicLinkUrls, parsedDynamicLinkData);
    }

    private void OnLinkReceived(List<string> dynamicLinkUrls, Dictionary<string, string> parsedDynamicLinkData)
    {

        if (parsedDynamicLinkData.ContainsKey(DYNAMIC_LINK_PARAM_ID))
        {
            string base64String = parsedDynamicLinkData[DYNAMIC_LINK_PARAM_ID];
            if (!TryDecodeBaseString(base64String, out string decodedString))
            {
                Debug.LogError("Unable To Decode Link Data From Dynamic Link URL: " + base64String);
                return;
            }
            if (!TryGetDynamicLinkAnalyticsData(decodedString, out DynamicLinkData data))
            {
                Debug.LogError("Unable To Parse URL Link Data from Decoded string: " + decodedString);
                return;
            }
            LogDynamicLinkAnalyticsData(data);
        }
        else
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "#GamePromotionLogger: DynamicLink data does not contains valid parameter");
        }
    }

    private void LogDynamicLinkAnalyticsData(DynamicLinkData dynamicLinkData)
    {
        SendPropertyData(dynamicLinkData);
        SendEventData(dynamicLinkData);
        PlayerPrefs.Save();
    }


    private void SendPropertyData(DynamicLinkData dynamicLinkData)
    {
        foreach (var item in dynamicLinkData.PropertiesEntryList)
        {
            string propertyName = item.Key;

            if (!propertyName.EndsWith(REPEAT_KEY))
            {
                string prefName = DYNAMIC_LINK_PROPERTY_KEY + dynamicLinkData.EventID + "_" + item.Key;

                bool hasKey = PlayerPrefs.HasKey(prefName);
                if (!hasKey)
                {
                    AnalyticsManager.SetProperty(item.Key, item.Value);
                    PlayerPrefs.SetString(prefName, item.Value);
                }
            }
            else
            {
                AnalyticsManager.SetProperty(item.Key.Replace(REPEAT_KEY, ""), item.Value);
            }
        }
    }

    private void SendEventData(DynamicLinkData dynamicLinkData)
    {
        foreach (var item in dynamicLinkData.DynamicEventsList)
        {
            string eventName = item.EventName;
            GameParameterBuilder parameterBuilder = new GameParameterBuilder();
            foreach (var property in item.PropertiesEntryList)
            {
                parameterBuilder.Add(property.Key.Replace(REPEAT_KEY, ""), property.Value);
            }

            if (!eventName.EndsWith(REPEAT_KEY))
            {
                string prefName = DYNAMIC_LINK_EVENT_KEY + dynamicLinkData.EventID + "_" + item.EventName;
                if (!PlayerPrefs.HasKey(prefName))
                {
                    AnalyticsManager.LogEvent(eventName, parameterBuilder);
                    PlayerPrefs.SetString(prefName, item.EventName);
                }
            }
            else
            {
                AnalyticsManager.LogEvent(eventName.Replace(REPEAT_KEY, ""), parameterBuilder);
            }
        }
    }


    private bool TryGetDynamicLinkAnalyticsData(string jsonString, out DynamicLinkData data)
    {
        try
        {
            data = JsonConvert.DeserializeObject<DynamicLinkData>(jsonString);
            return data != null;
        }
        catch (System.Exception error)
        {
            Debug.LogError($"Unable To json string value: {jsonString}, Error {error}");
            data = null;
            return false;
        }

    }

    private bool TryDecodeBaseString(string deepLinkParamsBase64, out string decodedString)
    {
        try
        {
            byte[] data = Convert.FromBase64String(deepLinkParamsBase64);
            decodedString = Encoding.UTF8.GetString(data);
            return true;
        }
        catch (Exception error)
        {
            Debug.LogError($"Unable To Parse Base64 value: {deepLinkParamsBase64}, Error {error}");
            decodedString = string.Empty;
            return false;
        }
    }

}
