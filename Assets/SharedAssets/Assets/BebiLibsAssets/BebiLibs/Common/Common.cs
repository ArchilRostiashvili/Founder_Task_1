using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;


public class Common
{
    public static int TimeCurrent()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        return currentEpochTime;
    }

    public static int SecondsElapsed(int t1)
    {
        int difference = Common.TimeCurrent() - t1;

        return Mathf.Abs(difference);
    }

    private static float _oldPpu = 0.0f;
    public static float PPU
    {
        get
        {
            if (Camera.main != null && _oldPpu == 0.0f)
            {
                _oldPpu = (Camera.main.orthographicSize * 2.0f) / (float)Screen.height;
            }
            return _oldPpu;
        }
    }

    public static int TimeNow_Seconds2020()
    {
        DateTime dateTimeToday = DateTime.Now;
        TimeSpan timeSpan = dateTimeToday.Subtract(new DateTime(2020, 1, 1, 0, 0, 0));
        return (int)timeSpan.TotalSeconds;
    }

    //private static bool _isTest = false;
    public static double TimeNow_Seconds()
    {
        DateTime dateTimeToday = DateTime.Now;
        /*
        if (_isTest)
        {
            int hours = PlayerPrefs.GetInt("TestIndex", 0) * 6;
            dateTimeToday = dateTimeToday.AddHours(hours);
        }
        */
        //Common.DebugLog("vvvvvvvvvvvvvv");
        //Common.DebugLog("-----" + dateTimeToday.ToShortDateString());
        TimeSpan timeSpan = dateTimeToday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return timeSpan.TotalSeconds;
    }

    public static double TimeNow_Hours()
    {
        DateTime dateTimeToday = DateTime.Now;
        /*
        if (_isTest)
        {
            int hours = PlayerPrefs.GetInt("TestIndex", 0) * 6;
            dateTimeToday = dateTimeToday.AddHours(hours);
        }
        */
        TimeSpan timeSpan = dateTimeToday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return timeSpan.TotalHours;
    }

    public static double TimeNow_Minutes()
    {
        DateTime dateTimeToday = DateTime.Now;
        /*
        if (_isTest)
        {
            int hours = PlayerPrefs.GetInt("TestIndex", 0) * 6;
            dateTimeToday = dateTimeToday.AddHours(hours);
        }
        */
        TimeSpan timeSpan = dateTimeToday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return timeSpan.TotalMinutes;
    }

    public static double TimeNow_Milliseconds()
    {
        DateTime dateTimeToday = DateTime.Now;
        /*
        if (_isTest)
        {
            int hours = PlayerPrefs.GetInt("TestIndex", 0) * 6;
            dateTimeToday = dateTimeToday.AddHours(hours);
        }
        */
        TimeSpan timeSpan = dateTimeToday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return timeSpan.TotalMilliseconds;
    }


    [Conditional("DebugEnabled")]
    public static void DebugLog(string log)
    {
        UnityEngine.Debug.Log(log);
    }
}//}
