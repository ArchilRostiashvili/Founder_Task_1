using UnityEngine;

namespace BebiLibs.RemoteConfigSystem
{
    [System.Serializable]
    public class RemoteStringVariable : RemoteVariable
    {
        [SerializeField]
        internal string value;

        [SerializeField]
        internal string testDeviceValue;

        public string Value => value;

        internal RemoteStringVariable()
        {
            typeCode = System.TypeCode.String;
        }

        internal override void SaveValueToPref()
        {
            SetValue(value);
        }

        internal override object GetRemoteValue()
        {
            string storedValue = PlayerPrefs.GetString(PrefKey, value);
            value = storedValue;
            return storedValue;
        }

        internal override void SetValue(object value)
        {
            this.value = (string)value;
            PlayerPrefs.SetString(PrefKey, this.value);
        }

        internal override void UpdateValueFromPref()
        {
            value = (string)GetRemoteValue();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        internal override object GetTestDeviceValue()
        {
            return testDeviceValue;
        }

        internal static RemoteStringVariable Create(string key, string defaultValue)
        {
            RemoteStringVariable variable = ScriptableConfig.CreateInstance<RemoteStringVariable>();
            variable.dataVariableKey = key;
            variable.isDynamicVariable = true;
            variable.eventName = key;
            variable.value = defaultValue;
            return variable;
        }
    }
}