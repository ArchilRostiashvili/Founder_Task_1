using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BebiLibs
{
    public class ApplicationSessionInfo
    {
        private const string _SESSION_COUNT_PREF_KEY = "Applicaton_Session_Count";
        private const string _LAST_SESSION_DATE_PREF_KEY = "Applicaton_Last_Session_Date";
        private const string _PLAYED_DAY_COUNT_PREF_KEY = "Application_Played_Day_Count";


        public static long SessionCount { get; private set; }
        public static bool IsFistSession { get; private set; }

        public static int PlayedDayCount { get; private set; }
        public static DateTime LastSessionStartLocalDate { get; private set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnSessionStarts()
        {
            //Debug.Log("ApplicationSessionInfo.OnSessionStarts()");
            UpdateSessionCount();
            UpdatePlayedDayCount();
            SaveUpdates();
        }

        private static void UpdateSessionCount()
        {
            int sessionCount = PlayerPrefs.GetInt(_SESSION_COUNT_PREF_KEY, -1);

            sessionCount += 1;

            SessionCount = sessionCount;
            IsFistSession = sessionCount == 0;

            PlayerPrefs.SetInt(_SESSION_COUNT_PREF_KEY, sessionCount);
        }

        private static void UpdatePlayedDayCount()
        {
            string lastTimeString = PlayerPrefs.GetString(_LAST_SESSION_DATE_PREF_KEY, TimeToString(DateTime.Now));

            PlayedDayCount = GetPlayedDayCountFromPref();
            if (TryParseStringToDate(lastTimeString, out DateTime lastSavedTime))
            {
                LastSessionStartLocalDate = lastSavedTime;
                if (lastSavedTime.Date != DateTime.Now.Date)
                {
                    PlayedDayCount += 1;
                    SetPlayedDayCountFromPref(PlayedDayCount);
                }
            }

            PlayerPrefs.SetString(_LAST_SESSION_DATE_PREF_KEY, TimeToString(DateTime.Now));
        }

        private static int GetPlayedDayCountFromPref() => PlayerPrefs.GetInt(_PLAYED_DAY_COUNT_PREF_KEY, 0);
        private static void SetPlayedDayCountFromPref(int value) => PlayerPrefs.SetInt(_PLAYED_DAY_COUNT_PREF_KEY, value);

        private static string TimeToString(DateTime dateTime) => dateTime.ToString("o");
        private static bool TryParseStringToDate(string timeString, out DateTime dateTime) => DateTime.TryParse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime);

        private static void SaveUpdates() => PlayerPrefs.Save();
    }
}
