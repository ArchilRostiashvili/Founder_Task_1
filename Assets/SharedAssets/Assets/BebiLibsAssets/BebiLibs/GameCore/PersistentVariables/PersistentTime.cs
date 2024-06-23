using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs
{
    public class PersistentTime : PersistentVariable
    {
        public const string FMT = "O";
        [SerializeField]
        private DateTime _runtimeValue;

        public PersistentTime(string parameterID, DateTime defaultTime) : base(parameterID)
        {
            _runtimeValue = defaultTime;
            base._isValueUpdated = false;
        }

        public DateTime value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public void SetValue(DateTime value, bool writesPrefsToDisk = false)
        {
            if (!IsLocked)
            {
                PlayerPrefs.SetString(_parameterID, TimeToString(value));
                _runtimeValue = value;
                base._isValueUpdated = true;
            }

            if (writesPrefsToDisk)
            {
                PlayerPrefs.Save();
            }
        }

        public void InitializeDefault()
        {
            if (!PlayerPrefs.HasKey(_parameterID))
            {
                SetValue(_runtimeValue);
            }
        }

        public DateTime GetValue()
        {
            if (base._isValueUpdated) return _runtimeValue;
            base._isValueUpdated = true;
            InitializeDefault();
            string data = PlayerPrefs.GetString(_parameterID, TimeToString(_runtimeValue));
            if (TryLoadTime(data, out DateTime time))
            {
                _runtimeValue = time;
                return time;
            }
            else
            {
                return _runtimeValue;
            }
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        public static string TimeToString(DateTime time)
        {
            return time.ToString(FMT, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static bool TryLoadTime(string timeString, out DateTime time)
        {
            if (DateTime.TryParseExact(timeString, FMT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dataTime))
            {
                time = dataTime;
                return true;
            }
            else
            {
                time = DateTime.Now;
                return false;
            }
        }

        public static implicit operator DateTime(PersistentTime d) => d.GetValue();
        //public static explicit operator Persistentstring(string value) => new Digit(b);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersistentTime))]
    public class PersistentTimeDrawerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            GUI.enabled = false;
            EditorGUI.TextField(position, property.FindPropertyRelative("_runtimeValue").stringValue);
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
    }
#endif
}

