using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
namespace BebiLibs.GameSecurity
{
    public class SafePlayerPrefs
    {
        public delegate void StringSetter(string key, string value);
        public delegate string StringGetter(string key, string defaultValue);

        public delegate string StringConverter<T>(T value);
        public delegate T StringParser<T>(string value);

        public static void SetValue<T>(string key, T value, StringSetter stringSetter, StringConverter<T> stringConverter)
        {
            stringSetter(key, StringCipher.Encrypt(stringConverter(value), PrefKeyObfuscator.DataString()));
        }

        public static T GetValue<T>(string key, T defaultValue, StringGetter stringGetter, StringConverter<T> stringConverter, StringParser<T> stringParser)
        {
            string defaultValueCrypt = StringCipher.Encrypt(stringConverter(defaultValue), PrefKeyObfuscator.DataString());
            string encryptedValue = StringCipher.Decrypt(stringGetter(key, defaultValueCrypt), PrefKeyObfuscator.DataString());
            return stringParser(encryptedValue);
        }


        public static void SetString(string key, string value)
        {
            SetValue<string>(key, value, PlayerPrefs.SetString, Convert.ToString);
        }

        public static string GetString(string key, string defaultValue)
        {
            return GetValue<string>(key, defaultValue, PlayerPrefs.GetString, Convert.ToString, Convert.ToString);
        }

        public static void SetFloat(string key, float value)
        {
            SetValue<float>(key, value, PlayerPrefs.SetString, Convert.ToString);
        }

        public static float GetFloat(string key, float defaultValue)
        {
            return GetValue<float>(key, defaultValue, PlayerPrefs.GetString, Convert.ToString, (string floatString) => (float)Convert.ToDouble(floatString));
        }

        public static void SetInt(string key, int value)
        {
            SetValue<int>(key, value, PlayerPrefs.SetString, Convert.ToString);
        }

        public static int GetInt(string key, int defaultValue)
        {
            return GetValue<int>(key, defaultValue, PlayerPrefs.GetString, Convert.ToString, (string intString) => (int)Convert.ToInt32(intString));
        }

        public static void SetBool(string key, bool value)
        {
            SetValue<bool>(key, value, PlayerPrefs.SetString, Convert.ToString);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            return GetValue<bool>(key, defaultValue, PlayerPrefs.GetString, Convert.ToString, Convert.ToBoolean);
        }


    }
}
#endif