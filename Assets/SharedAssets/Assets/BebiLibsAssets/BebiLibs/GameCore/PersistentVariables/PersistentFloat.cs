using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    [System.Serializable]
    public class PersistentFloat : PersistentVariable
    {
        [SerializeField]
        private float _runtimeValue;

        public PersistentFloat(string parameterID, float defaultValue) : base(parameterID)
        {
            _runtimeValue = defaultValue;
            base._isValueUpdated = false;
        }

        public float value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public void InitializeDefault()
        {
            if (!PlayerPrefs.HasKey(_parameterID))
            {
                SetValue(_runtimeValue);
            }
        }

        public void SetValue(float value, bool writesPrefsToDisk = false)
        {
            if (!IsLocked)
            {
                PlayerPrefs.SetFloat(_parameterID, value);
                _runtimeValue = value;
                base._isValueUpdated = true;
            }

            if (writesPrefsToDisk)
            {
                PlayerPrefs.Save();
            }
        }

        public float GetValue()
        {
            if (base._isValueUpdated) return _runtimeValue;
            base._isValueUpdated = true;
            InitializeDefault();
            _runtimeValue = PlayerPrefs.GetFloat(_parameterID, _runtimeValue);
            return _runtimeValue;
        }

        public static implicit operator float(PersistentFloat d) => d.GetValue();
        //public static explicit operator PersistentFloat(string value) => new Digit(b);

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersistentFloat))]
    public class PersistentFloatDrawerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            GUI.enabled = false;
            EditorGUI.TextField(position, property.FindPropertyRelative("_runtimeValue").floatValue.ToString());
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
    }
#endif
}
