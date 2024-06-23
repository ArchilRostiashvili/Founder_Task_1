using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class PersistentInteger : PersistentVariable
    {
        [HideInInspector] private int _runtimeValue = 0;

        public PersistentInteger(string parameterID, int defaultValue) : base(parameterID)
        {
            _runtimeValue = defaultValue;
            base._isValueUpdated = false;
        }

        public int value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public void SetValue(int value, bool writesPrefsToDisk = false)
        {
            if (IsLocked) return;

            PlayerPrefs.SetInt(_parameterID, value);
            _runtimeValue = value;
            base._isValueUpdated = true;

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

        public void Increment(int value = 1)
        {
            SetValue(GetValue() + value);
        }

        public int GetValue()
        {
            if (base._isValueUpdated) return _runtimeValue;
            base._isValueUpdated = true;
            InitializeDefault();
            _runtimeValue = PlayerPrefs.GetInt(_parameterID, _runtimeValue);
            return _runtimeValue;
        }

        public static implicit operator int(PersistentInteger d) => d.GetValue();

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersistentInteger))]
    public class PersistentIntegerDrawerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            GUI.enabled = false;
            EditorGUI.TextField(position, property.FindPropertyRelative("_runtimeValue").intValue.ToString());
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
    }
#endif
}
