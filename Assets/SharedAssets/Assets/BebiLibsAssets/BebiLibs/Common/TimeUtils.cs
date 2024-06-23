using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public static class TimeUtils
    {
        public const string FMT = "O";

        public static TimeSpan RemainingTime(DateTime oldTime, TimeSpan unlockTime)
        {
            TimeSpan timeDuration = DateTime.Now - oldTime;
            TimeSpan timeForUnlock = unlockTime - timeDuration;
            return timeForUnlock;
        }

        public static void SaveCurrentTime(string id)
        {
            string strDate = DateTime.Now.ToString(FMT);
            PlayerPrefs.SetString(id, strDate);
        }

        public static string TimeToString(DateTime time)
        {
            return time.ToString(FMT, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static void SaveTime(string id, DateTime time)
        {
            string strDate = time.ToString(FMT);
            PlayerPrefs.SetString(id, strDate);
        }

        public static bool TryLoadTime(string timeString, out DateTime time)
        {
            if (DateTime.TryParseExact(timeString, FMT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dataTime))
            {
                time = dataTime;
                return true;
            }
            else
            {
                time = DateTime.Now;
                return false;
            }
        }

        public static bool TryLoadSavedTime(string timeString, out DateTime time)
        {
            string data = PlayerPrefs.GetString(timeString);
            if (DateTime.TryParseExact(data, FMT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dataTime))
            {
                time = dataTime;
                return true;
            }
            else
            {
                time = DateTime.Now;
                return false;
            }
        }


        public static double ToUnixTime(DateTime dateTime)
        {
            double unixTimeStamp;
            DateTime zuluTime = dateTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1).ToUniversalTime();
            unixTimeStamp = zuluTime.Subtract(unixEpoch).TotalSeconds;
            return unixTimeStamp;
        }

        public static long UnixTimeInSeconds()
        {
            long unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1).ToUniversalTime();
            unixTimeStamp = (long)zuluTime.Subtract(unixEpoch).TotalSeconds;
            return unixTimeStamp;
        }
    }
}
