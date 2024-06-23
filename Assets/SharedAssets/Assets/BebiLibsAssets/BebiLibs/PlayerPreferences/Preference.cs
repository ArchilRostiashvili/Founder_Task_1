using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PlayerPreferencesSystem
{
    [System.Serializable]
    public class Preference
    {
        public string Key;
        public PreferenceDataType PreferenceType;
        public PreferenceSourceType SourceType;

        public bool BoolValue;
        public long LongValue;
        public double DoubleValue;
        public string StringValue;


        public Preference(string key, PreferenceDataType type)
        {
            Key = key;
            PreferenceType = type;
        }

        public static Preference CreateBool(string key, bool value)
        {
            Preference preference = new Preference(key, PreferenceDataType.Bool);
            preference.BoolValue = value;
            return preference;
        }

        public static Preference CreateLong(string key, long value)
        {
            Preference preference = new Preference(key, PreferenceDataType.Long);
            preference.LongValue = value;
            return preference;
        }

        public static Preference CreateDouble(string key, double value)
        {
            Preference preference = new Preference(key, PreferenceDataType.Double);
            preference.DoubleValue = value;
            return preference;
        }

        public static Preference CreateString(string key, string value)
        {
            Preference preference = new Preference(key, PreferenceDataType.String);
            preference.StringValue = value;
            return preference;
        }

        public override string ToString()
        {
            return PreferenceType switch
            {
                PreferenceDataType.Bool => BoolValue.ToString(),
                PreferenceDataType.Long => LongValue.ToString(),
                PreferenceDataType.Double => DoubleValue.ToString(),
                PreferenceDataType.String => StringValue,
                _ => "",
            };
        }

    }
}
