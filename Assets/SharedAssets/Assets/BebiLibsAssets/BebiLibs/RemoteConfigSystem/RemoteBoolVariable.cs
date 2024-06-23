using System;
using UnityEngine;

namespace BebiLibs.RemoteConfigSystem
{
    [System.Serializable]
    public class RemoteBoolVariable : RemoteVariable
    {
        [SerializeField]
        internal bool value;

        [SerializeField]
        internal bool testDeviceValue;

        public bool Value => value;

        internal RemoteBoolVariable()
        {
            base.typeCode = TypeCode.Boolean;
        }

        internal override void SaveValueToPref()
        {
            SetValue(value);
        }

        internal override object GetRemoteValue()
        {
            bool storedValue = PlayerPrefs.GetInt(PrefKey, BoolToInt(value)) == 1;
            value = storedValue;
            return storedValue;
        }

        internal override void SetValue(object value)
        {
            this.value = (bool)value;
            PlayerPrefs.SetInt(PrefKey, BoolToInt(this.value));
        }

        internal override void UpdateValueFromPref()
        {
            value = (bool)GetRemoteValue();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        internal override object GetTestDeviceValue()
        {
            return testDeviceValue;
        }

        internal static RemoteBoolVariable Create(string key, bool value)
        {
            RemoteBoolVariable variable = ScriptableConfig.CreateInstance<RemoteBoolVariable>();
            variable.dataVariableKey = key;
            variable.isDynamicVariable = true;
            variable.eventName = key;
            variable.value = value;
            return variable;
        }
    }

}