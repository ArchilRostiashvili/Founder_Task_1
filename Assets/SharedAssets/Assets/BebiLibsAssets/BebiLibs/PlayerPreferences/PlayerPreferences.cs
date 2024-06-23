using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace BebiLibs.PlayerPreferencesSystem
{
    public sealed class PlayerPreferences
    {
        private static Dictionary<string, Preference> _PreferencesDictionary = new Dictionary<string, Preference>();
        private static string _FileName = "PlayerPreferences.dat";
        private static string _FilePath;

        private static bool _IsSaveIsInUse = false;
        private static bool _SaveAfterFinish;

        internal static Dictionary<string, Preference> GamePreferences => _PreferencesDictionary;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            //Debug.Log("PlayerPreferences.Initialize()");
            InitializeCorrectPreferencesPath();
            LoadPreferencesBinaryDataFromFile();
        }

        private static void InitializeCorrectPreferencesPath()
        {
#if UNITY_EDITOR
            _FilePath = Path.Combine(Application.dataPath, _FileName);
            UriBuilder uriBuilder = new UriBuilder(_FilePath);
            _FilePath = uriBuilder.Path;
#else
            _FilePath = Path.Combine(Application.persistentDataPath, _FileName);
            UriBuilder uriBuilder = new UriBuilder(_FilePath);
            _FilePath = uriBuilder.Path;
#endif
        }

        private static void LoadPreferencesBinaryDataFromFile()
        {
            if (!File.Exists(_FilePath))
            {
                File.Create(_FilePath);
                return;
            }

            using FileStream fs = new FileStream(_FilePath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _PreferencesDictionary = (Dictionary<string, Preference>)formatter.Deserialize(fs);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to deserialize. Reason: " + e.Message);
            }
        }

        public static void SetBool(string key, bool value)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                _PreferencesDictionary[key].BoolValue = value;
            }
            else
            {
                _PreferencesDictionary.Add(key, Preference.CreateBool(key, value));
            }
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                return _PreferencesDictionary[key].BoolValue;
            }
            else
            {
                SetBool(key, defaultValue);
                return defaultValue;
            }
        }

        public static void SetLong(string key, long value)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                _PreferencesDictionary[key].LongValue = value;
            }
            else
            {
                _PreferencesDictionary.Add(key, Preference.CreateLong(key, value));
            }
        }

        public static long GetLong(string key, long defaultValue)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                return _PreferencesDictionary[key].LongValue;
            }
            else
            {
                SetLong(key, defaultValue);
                return defaultValue;
            }
        }

        public static void SetDouble(string key, double value)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                _PreferencesDictionary[key].DoubleValue = value;
            }
            else
            {
                _PreferencesDictionary.Add(key, Preference.CreateDouble(key, value));
            }
        }

        public static double GetDouble(string key, double defaultValue)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                return _PreferencesDictionary[key].DoubleValue;
            }
            else
            {
                SetDouble(key, defaultValue);
                return defaultValue;
            }
        }


        public static void SetString(string key, string value)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                _PreferencesDictionary[key].StringValue = value;
            }
            else
            {
                _PreferencesDictionary.Add(key, Preference.CreateString(key, value));
            }
        }

        public static string GetString(string key, string defaultValue)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                return _PreferencesDictionary[key].StringValue;
            }
            else
            {
                SetString(key, defaultValue);
                return defaultValue;
            }
        }

        public static void DeleteKey(string key)
        {
            if (_PreferencesDictionary.ContainsKey(key))
            {
                _PreferencesDictionary.Remove(key);
            }
        }

        public static void DeleteAll()
        {
            _PreferencesDictionary.Clear();
        }

        public static bool HasKey(string key)
        {
            return _PreferencesDictionary.ContainsKey(key);
        }

        public static void LogState()
        {
            string result = "Player Pref State: \n";
            foreach (KeyValuePair<string, Preference> de in _PreferencesDictionary)
            {
                result += de.Key + " lives at " + de.Value + ".\n";
            }
            Debug.Log(result);
        }

        private static List<System.Action<string>> _SaveCallbacksList = new List<System.Action<string>>();

        public static async void SaveAsync(Action<string> onComplete = null)
        {
            if (_IsSaveIsInUse)
            {
                _SaveAfterFinish = true;
                AppendAction(onComplete);
                return;
            }

            _IsSaveIsInUse = true;
            AppendAction(onComplete);

            Task<string> saveTask = Save();
            await saveTask;

            CallSaveTasks(saveTask.Result);

            _IsSaveIsInUse = false;
            if (_SaveAfterFinish)
            {
                _SaveAfterFinish = false;
                SaveAsync();
            }
        }

        private static void AppendAction(Action<string> onComplete = null)
        {
            if (onComplete != null && !_SaveCallbacksList.Contains(onComplete))
            {
                _SaveCallbacksList.Add(onComplete);
            }
        }

        private static void CallSaveTasks(string result)
        {
            foreach (var item in _SaveCallbacksList)
            {
                item?.Invoke(result);
            }
            _SaveCallbacksList.Clear();
        }

        private static async Task<string> Save()
        {
            if (string.IsNullOrWhiteSpace(_FilePath))
            {
                InitializeCorrectPreferencesPath();
            }

            using FileStream fileStream = new FileStream(_FilePath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, _PreferencesDictionary);
                await fileStream.WriteAsync(memoryStream.GetBuffer());
                fileStream.Close();
                return string.Empty;
            }
            catch (Exception e)
            {
                fileStream.Close();
                Debug.LogError("Failed to serialize. Reason: " + e.Message);
                return e.Message;
            }
        }
    }
}
