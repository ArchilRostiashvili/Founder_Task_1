using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Analytics
{
    public class AnalyticsDictionary : MonoBehaviour
    {
        private static Dictionary<string, string> _GlobalAnalyticsParameters = new Dictionary<string, string>();

        public static string GetValue(ConstantKey key)
        {
            if (_GlobalAnalyticsParameters.TryGetValue(key, out string value))
            {
                return value;
            }
            return "null";
        }

        public static void StoreValue(ConstantKey key, string value)
        {
            if (_GlobalAnalyticsParameters.ContainsKey(key))
            {
                _GlobalAnalyticsParameters[key] = value;
            }
            else
            {
                _GlobalAnalyticsParameters.Add(key, value);
            }
        }
    }
}
