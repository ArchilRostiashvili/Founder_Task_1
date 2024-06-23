using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.RemoteConfigSystem
{
    [System.Serializable]
    public abstract class RemoteVariable : ScriptableObject
    {
        [SerializeField] protected string dataVariableKey = "RemoteValue";
        [SerializeField] internal string eventName;
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private bool _updateInEditor = false;
        [SerializeField] private bool _isUnique = false;

        internal System.TypeCode typeCode;
        protected bool isDynamicVariable = false;

        public string Key => dataVariableKey;
        public string PrefKey => isDynamicVariable ? Key + "_dynamic" : Key;
        internal bool UpdateInEditor => _updateInEditor;
        internal bool IsEnabled => _isEnabled;
        internal bool IsUnique => _isUnique;

        internal bool IsDataValueAssigned
        {
            get => PlayerPrefs.GetInt(PrefKey + "_isDataAssigned", 0) == 1;
            set => PlayerPrefs.SetInt(PrefKey + "_isDataAssigned", BoolToInt(value));
        }

        internal bool IsRemoteValue
        {
            get => PlayerPrefs.GetInt(PrefKey + "_isRemote", 0) == 1;
            set => PlayerPrefs.SetInt(PrefKey + "_isRemote", BoolToInt(value));
        }

        internal bool IsActivationEventSent
        {
            get => PlayerPrefs.GetInt(PrefKey + "_isSent", 0) == 1;
            set => PlayerPrefs.SetInt(PrefKey + "_isSent", BoolToInt(value));
        }

        private void OnEnable() => UpdateValueFromPref();
        private void OnValidate() => SaveValueToPref();

        internal abstract void UpdateValueFromPref();
        internal abstract void SaveValueToPref();

        internal abstract void SetValue(System.Object value);

        internal abstract System.Object GetRemoteValue();
        internal abstract System.Object GetTestDeviceValue();

        protected int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        internal void SetVariableKey(string newKey)
        {
            dataVariableKey = newKey;
            eventName = newKey;
        }
    }
}
