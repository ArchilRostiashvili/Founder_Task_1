using UnityEngine;

namespace BebiLibs.RemoteConfigSystem
{
    [System.Serializable]
    public class RemoteLongVariable : RemoteVariable
    {
        [SerializeField]
        internal long value;

        [SerializeField]
        internal int testDeviceValue;

        public long Value => value;

        internal RemoteLongVariable()
        {
            typeCode = System.TypeCode.Int32;
        }

        internal override void SaveValueToPref()
        {
            SetValue(value);
        }

        internal override object GetRemoteValue()
        {
            int storedValue = PlayerPrefs.GetInt(PrefKey, (int)value);
            value = storedValue;
            return storedValue;
        }

        internal override void SetValue(object value)
        {
            this.value = (int)value;
            PlayerPrefs.SetInt(PrefKey, (int)this.value);
        }

        internal override void UpdateValueFromPref()
        {
            value = (int)GetRemoteValue();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        internal override object GetTestDeviceValue()
        {
            return testDeviceValue;
        }

        internal static RemoteLongVariable Create(string key, long value)
        {
            RemoteLongVariable variable = ScriptableConfig.CreateInstance<RemoteLongVariable>();
            variable.dataVariableKey = key;
            variable.isDynamicVariable = true;
            variable.eventName = key;
            variable.value = value;
            return variable;
        }
    }

}