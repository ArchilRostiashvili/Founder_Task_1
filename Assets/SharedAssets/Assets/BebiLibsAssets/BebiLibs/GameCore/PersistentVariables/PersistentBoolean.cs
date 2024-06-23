using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    [System.Serializable]
    public class PersistentBoolean : PersistentVariable
    {
        [System.NonSerialized] private bool _runtimeValue = false;
#pragma warning disable IDE0052
        [SerializeField] private bool _editorValue = false;

        public PersistentBoolean(string parameterID, bool defaultValue) : base(parameterID)
        {
            _runtimeValue = defaultValue;
            _editorValue = defaultValue;
            _isValueUpdated = false;
        }

        public bool value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public void SetValue(bool value, bool writesPrefsToDisk = false)
        {
            if (IsLocked) return;

            PlayerPrefs.SetInt(_parameterID, BoolToInt(value));
            _runtimeValue = value;
            _editorValue = value;
            _isValueUpdated = true;

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

        public bool GetValue()
        {
            if (_isValueUpdated) return _runtimeValue;
            _isValueUpdated = true;
            InitializeDefault();
            int val = BoolToInt(_runtimeValue);
            _runtimeValue = PlayerPrefs.GetInt(_parameterID, val) == 1;
            _editorValue = _runtimeValue;
            return _runtimeValue;
        }

        public static implicit operator bool(PersistentBoolean d) => d.GetValue();
        //public static explicit operator PersistentBool(bool value) => new Digit(b);

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersistentBoolean))]
    public class PersistentBooleanDrawerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //EditorGUI.PropertyField(position, property.FindPropertyRelative("_boolValue"), label);
            GUI.enabled = false;
            EditorGUI.TextField(position, label, property.FindPropertyRelative("_editorValue").boolValue.ToString());
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
    }
#endif
}
