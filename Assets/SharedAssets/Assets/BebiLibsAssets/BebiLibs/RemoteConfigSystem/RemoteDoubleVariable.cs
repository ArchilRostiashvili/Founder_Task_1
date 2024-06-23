using UnityEngine;

namespace BebiLibs.RemoteConfigSystem
{
    [System.Serializable]
    public class RemoteDoubleVariable : RemoteVariable
    {
        [SerializeField]
        internal double value;

        [SerializeField]
        internal float testDeviceValue;

        public double Value => value;

        internal RemoteDoubleVariable()
        {
            typeCode = System.TypeCode.Double;
        }

        internal override void SaveValueToPref()
        {
            SetValue(value);
        }

        internal override object GetRemoteValue()
        {
            double storedValue = PlayerPrefs.GetFloat(PrefKey, (float)value);
            value = storedValue;
            return storedValue;
        }


        internal override void SetValue(object value)
        {
            this.value = (float)value;
            PlayerPrefs.SetFloat(PrefKey, (float)this.value);
        }

        internal override void UpdateValueFromPref()
        {
            value = (float)GetRemoteValue();
        }

        public override string ToString()
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        internal override object GetTestDeviceValue()
        {
            return testDeviceValue;
        }

        internal static RemoteDoubleVariable Create(string key, double value)
        {
            RemoteDoubleVariable variable = ScriptableConfig.CreateInstance<RemoteDoubleVariable>();
            variable.dataVariableKey = key;
            variable.isDynamicVariable = true;
            variable.eventName = key;
            variable.value = value;
            return variable;
        }
    }
}