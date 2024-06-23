using System.Collections;
using System.Collections.Generic;
using BebiLibs.Analytics;
using UnityEngine;
using System.Linq;
using BebiLibs.RemoteConfigSystem.Variables;

#if ACTIVATE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif

namespace BebiLibs.RemoteConfigSystem
{
    public class RemoteConfigManager : GenericSingletonClass<RemoteConfigManager>
    {
        public static event System.Action OnLoadFinishedEvent;

        public static bool IsLoadFinished { get; private set; } = false;
        public static float TimeOutInSeconds { get; private set; } = 15f;
        public static string UserToken { get; private set; }
        public static string UserInstallationID { get; private set; }

        private static bool _IsInEditor = false;
        private static bool _IsFirebaseInitialized = false;

        private bool _isAsyncLoadFinished = false;

        private static RemoteConfigSettings _Settings;
        private RequestStatus _fetchStatus = RequestStatus.Failed;

        [ObjectInspector()]
        [SerializeField] private List<RemoteVariable> _allFetchedVariables = new List<RemoteVariable>();

        protected override void OnInstanceAwake()
        {
            _Settings = RemoteConfigSettings.DefaultInstance();
            TimeOutInSeconds = _Settings.TimeOutInSeconds;

#if UNITY_EDITOR
            _IsInEditor = true;
#endif
        }


        private void Start()
        {
#if ACTIVATE_REMOTE_CONFIG
            WaitForDataFetch();
            FirebaseDependencyResolver.AddInitializationListener(OnFirebaseInitialize, true);
#else
            IsLoadFinished = true;
            _isAsyncLoadFinished = true;
#endif
        }


#if ACTIVATE_REMOTE_CONFIG
        private void OnFirebaseInitialize(bool isSuccessful)
        {
            if (isSuccessful)
            {
                OnFirebaseAppDependencyFix();
                UpdateUserTokenAndID();
            }
            else
            {
                _isAsyncLoadFinished = true;
            }
            _IsFirebaseInitialized = true;
        }
#endif

        private void OnDisable()
        {
            StopAllCoroutines();
        }


        public static void UseVariable(string variableKey)
        {
#if ACTIVATE_REMOTE_CONFIG  
            if (Instance == null)
            {
                DefaultInstance();
            }


            if (!RemoteConfigManager.IsLoadFinished || !_IsFirebaseInitialized)
            {
                Debug.LogFormat($"Sending activation event \"{variableKey}\" before initializing RemoteConfig or Firebase components may cause unexpected behaviour");
                Instance.LockAllVariable();
                PlayerPrefs.Save();
                return;
            }

            if (!TryGetRemoteVariable(variableKey, out RemoteVariable remoteVariable))
            {
                Debug.LogError($"Remote Variable With Key {variableKey} not fount in remote variable list");
                return;
            }
            else if (remoteVariable.IsRemoteValue && !remoteVariable.IsActivationEventSent)
            {
                AnalyticsManager.SetProperty(remoteVariable.Key, remoteVariable.ToString());
                AnalyticsManager.LogEvent(remoteVariable.eventName);
                remoteVariable.IsActivationEventSent = true;
            }
            else if (remoteVariable.IsUnique && !remoteVariable.IsDataValueAssigned)
            {
                remoteVariable.IsActivationEventSent = true;
                remoteVariable.IsDataValueAssigned = true;
            }
            PlayerPrefs.Save();
#endif
        }

        private static bool TryGetRemoteVariable(string variableKey, out RemoteVariable remoteVariable)
        {
            if (Instance == null)
            {
                DefaultInstance();
            }

            if ((!IsLoadFinished || !_IsFirebaseInitialized) && !Debug.isDebugBuild)
            {
                Debug.LogWarning($"Getting values \"{variableKey}\" before initializing RemoteConfig or Firebase components may cause unexpected behaviour");
            }

            if (_Settings == null)
            {
                Debug.LogError("ManagerRemoteConfig: There is no DataRemote Instance under Resources folder, Create using RemoteConfigEditor");
                remoteVariable = null;
                return false;
            }

            if (_Settings.TryGetVariable(variableKey, out remoteVariable))
            {
                return true;
            }
            else
            {
                Debug.LogError($"ManagerRemoteConfig: Variable Named {variableKey} Not Found In Remote Varable List");
                remoteVariable = null;
                return false;
            }
        }


        //Default To string.Empty
        public static bool TryGetString(string variableKey, out string value, bool sendActivationEvent = false)
        {
            if (TryGetRemoteVariable(variableKey, out RemoteVariable variable))
            {
                if (variable.typeCode != System.TypeCode.String)
                {
                    value = string.Empty;
                    return false;
                }

                if (sendActivationEvent)
                {
                    UseVariable(variableKey);
                }

                value = _IsInEditor && !variable.UpdateInEditor ? (string)variable.GetTestDeviceValue() : (string)variable.GetRemoteValue();
                return true;
            }

            value = string.Empty;
            return false;
        }

        //Default To false
        //Retune does not represents remote config value 
        public static bool TryGetBool(string variableKey, out bool value, bool sendActivationEvent = false)
        {
            if (TryGetRemoteVariable(variableKey, out RemoteVariable variable))
            {
                if (variable.typeCode != System.TypeCode.Boolean)
                {
                    value = false;
                    return false;
                }

                if (sendActivationEvent)
                {
                    UseVariable(variableKey);
                }

                value = _IsInEditor && !variable.UpdateInEditor ? (bool)variable.GetTestDeviceValue() : (bool)variable.GetRemoteValue();
                return true;
            }

            value = false;
            return false;
        }

        // Default To 0.0f
        public static bool TryGetFloat(string variableKey, out float value, bool sendActivationEvent = false)
        {
            if (TryGetRemoteVariable(variableKey, out RemoteVariable variable))
            {
                if (variable.typeCode != System.TypeCode.Double)
                {
                    value = 0;
                    return false;
                }

                if (sendActivationEvent)
                {
                    UseVariable(variableKey);
                }

                value = _IsInEditor && !variable.UpdateInEditor ? (float)variable.GetTestDeviceValue() : (float)variable.GetRemoteValue();
                return true;
            }

            value = 0.0f;
            return false;
        }


        // Default To 0
        public static bool TryGetInt(string variableKey, out int value, bool sendActivationEvent = false)
        {
            if (TryGetRemoteVariable(variableKey, out RemoteVariable variable))
            {
                if (variable.typeCode != System.TypeCode.Int32)
                {
                    value = 0;
                    return false;
                }

                if (sendActivationEvent)
                {
                    UseVariable(variableKey);
                }

                value = _IsInEditor && !variable.UpdateInEditor ? (int)variable.GetTestDeviceValue() : (int)variable.GetRemoteValue();
                return true;
            }

            value = 0;
            return false;
        }


#if ACTIVATE_REMOTE_CONFIG
        public void OnFirebaseAppDependencyFix()
        {
            try
            {
                ConfigSettings configSettings = new ConfigSettings()
                {
                    FetchTimeoutInMilliseconds = (ulong)(_Settings.TimeOutInSeconds * 1000),
                    MinimumFetchInternalInMilliseconds = (ulong)(_Settings.FetchIntervalInSeconds * 1000)
                };

                Dictionary<string, object> defaultDict = CreateDefaultDict();
                FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                remoteConfig.SetConfigSettingsAsync(configSettings).ContinueWith(task =>
                {
                    remoteConfig.SetDefaultsAsync(defaultDict).ContinueWith(task =>
                    {
                        FetchData(remoteConfig);
                    });
                });
            }
            catch (System.Exception e)
            {
                _isAsyncLoadFinished = true;
                Debug.LogError("Firebase Remote Config data fetch error " + e);
            }
        }

        private void FetchData(FirebaseRemoteConfig remoteConfig)
        {
            remoteConfig.FetchAsync(System.TimeSpan.Zero).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    ActivateFetchedData(remoteConfig);
                }
                else
                {
                    _fetchStatus = RequestStatus.Failed;
                    _isAsyncLoadFinished = true;
                }
            });
        }

        private void ActivateFetchedData(FirebaseRemoteConfig remoteConfig)
        {
            remoteConfig.ActivateAsync().ContinueWith(task =>
            {
                if (!task.IsCompleted)
                {
                    _fetchStatus = RequestStatus.Failed;
                    _isAsyncLoadFinished = true;
                    Debug.LogError("Firebase Remote Config data fetch error " + task.Exception);
                }
                else
                {
                    _fetchStatus = RequestStatus.Success;
                    _isAsyncLoadFinished = true;
                }
            });
        }
#endif

        //Do not run this function inside Firebase Tasks, it works only in unity thread/ no error message get printed
        private Dictionary<string, object> CreateDefaultDict()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<RemoteVariable> localRemoteVariables = _Settings.RemoteVariableList;
            for (int i = 0; i < localRemoteVariables.Count; i++)
            {
                if (localRemoteVariables[i].IsEnabled)
                {
                    dictionary.Add(localRemoteVariables[i].Key, localRemoteVariables[i].GetRemoteValue());
                }
            }
            return dictionary;
        }

        private void WaitForDataFetch()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForDoneRealtime(_Settings.TimeOutInSeconds, () => _isAsyncLoadFinished);

                if (_fetchStatus == RequestStatus.Success)
                {

#if ACTIVATE_REMOTE_CONFIG
                    SetFetchedValues();
#endif
                }
                else
                {
                    LockAllVariable();
                }

                PlayerPrefs.Save();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();

                IsLoadFinished = true;
                OnLoadFinishedEvent?.Invoke();
            }
        }

        private void LockAllVariable()
        {
            List<RemoteVariable> localRemoteVariables = _Settings.RemoteVariableList;
            foreach (RemoteVariable remoteVariable in localRemoteVariables)
            {
                remoteVariable.IsActivationEventSent = true;
                remoteVariable.IsDataValueAssigned = true;
            }
        }

        public static List<StringTuple> GetStringFetchedValuesFromPrefix(string prefix)
        {
            if (Instance == null)
            {
                Debug.LogError("RemoteConfigManager is not initialized");
                return new List<StringTuple>();
            }

            var remoteVariables = Instance._allFetchedVariables.Where(x => x.typeCode == System.TypeCode.String).Where(x => x.Key.StartsWith(prefix)).Select(x => (RemoteStringVariable)x).ToList();
            List<StringTuple> result = new List<StringTuple>();
            foreach (RemoteStringVariable remoteVariable in remoteVariables)
            {
                result.Add(new StringTuple(remoteVariable.Key, remoteVariable.GetRemoteValue().ToString()));
            }
            return result;
        }


#if ACTIVATE_REMOTE_CONFIG
        void SetFetchedValues()
        {
            try
            {
                IDictionary<string, ConfigValue> fetchedData = FirebaseRemoteConfig.DefaultInstance.AllValues;
                List<RemoteVariable> localRemoteVariables = _Settings.RemoteVariableList;
                ProcessAllFetchedData(fetchedData);
                LogFetchedVariables();

                foreach (RemoteVariable remoteVariable in localRemoteVariables)
                {
                    if (!remoteVariable.IsEnabled) continue;
                    if (fetchedData.TryGetValue(remoteVariable.Key, out ConfigValue value))
                    {

                        object resultValueAsObject = GetValueFromConfig(value, remoteVariable.typeCode);
#if !UNITY_EDITOR
                        UpdateRemoteVariable(remoteVariable, value.Source, resultValueAsObject);
#else

                        if (remoteVariable.UpdateInEditor)
                        {
                            UpdateRemoteVariable(remoteVariable, value.Source, resultValueAsObject);
                        }
#endif
                    }
                    else
                    {
                        SetDefaultValue(remoteVariable);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Firebase Fetched Data Load Error Into Remote Config Fail " + e);
            }
        }

        private void ProcessAllFetchedData(IDictionary<string, ConfigValue> fetchedData)
        {
            foreach (KeyValuePair<string, ConfigValue> item in fetchedData)
            {
                if (!ConfigValuePrinter.TryGetRemoteVariableFromConfig(item.Key, item.Value, out RemoteVariable variable))
                {
                    continue;
                }

                if (variable == null)
                {
                    Debug.LogError("Variable is null");
                    continue;
                }

                _allFetchedVariables.Add(variable);
                variable.IsRemoteValue = item.Value.Source == ValueSource.RemoteValue;
            }
        }

        private void LogFetchedVariables()
        {
            try
            {
                string log = "Remote Config Variables:\n";
                int charLength = _allFetchedVariables.Max(x => x.Key.Length);
                foreach (RemoteVariable variable in _allFetchedVariables)
                {
                    string key = variable.Key.SubstringOrFill(charLength);
                    string value = variable.GetRemoteValue().ToString().SubstringOrFill(10);
                    string source = (variable.IsRemoteValue ? "Remote" : "Default").SubstringOrFill(7);
                    string type = variable.typeCode.ToString().SubstringOrFill(10);
                    log += $"Key: {key} Value: {value} Source: {source} Type: {type}\n";
                }
                SafeLogNoStack.Log(log, "Remote Config Variables");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable To Fetch Data, Error " + e);
            }
        }

        private void UpdateRemoteVariable(RemoteVariable variable, ValueSource source, object result)
        {
            if (variable.IsUnique)
            {
                if (!variable.IsDataValueAssigned)
                {
                    variable.IsRemoteValue = source == ValueSource.RemoteValue;
                    variable.IsDataValueAssigned = true;
                    variable.SetValue(result);
                }
            }
            else
            {
                variable.IsRemoteValue = source == ValueSource.RemoteValue;
                variable.IsDataValueAssigned = true;
                variable.SetValue(result); ;
            }
        }

        private void SetDefaultValue(RemoteVariable remoteVariable)
        {
            remoteVariable.IsDataValueAssigned = true;
            remoteVariable.IsRemoteValue = false;
        }


#endif

#if ACTIVATE_REMOTE_CONFIG
        private void DeleteUserData()
        {
            var installation = Firebase.Installations.FirebaseInstallations.DefaultInstance;
            installation.DeleteAsync().ContinueWith(task =>
            {
                Debug.LogError("Deleted User Data");
            });
        }
#endif

#if ACTIVATE_REMOTE_CONFIG
        private void UpdateUserTokenAndID()
        {
            var installation = Firebase.Installations.FirebaseInstallations.DefaultInstance;
            installation.GetTokenAsync(true).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    UserToken = task.Result;
                    Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Installations Token {0}", task.Result);
                }
            });

            installation.GetIdAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    UserInstallationID = task.Result;
                    Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Installations ID {0}", task.Result);
                }
            });
        }
#endif

#if ACTIVATE_REMOTE_CONFIG
        private object GetValueFromConfig(ConfigValue value, System.TypeCode varType)
        {
            return varType switch
            {
                System.TypeCode.Boolean => (bool)value.BooleanValue,
                System.TypeCode.Double => (float)value.DoubleValue,
                System.TypeCode.Int16 or System.TypeCode.Int32 or System.TypeCode.Int64 => (int)value.LongValue,
                System.TypeCode.String => (string)value.StringValue,
                _ => null,
            };
        }
#endif

        internal static void SetDataRemoteVariable(RemoteConfigSettings dataRemoteVariables)
        {
            _Settings = dataRemoteVariables;
        }
    }
}


