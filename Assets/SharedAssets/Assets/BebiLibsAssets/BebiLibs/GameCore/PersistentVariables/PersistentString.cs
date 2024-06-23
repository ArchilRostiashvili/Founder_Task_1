using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs
{
    [System.Serializable]
    public class PersistentString : PersistentVariable
    {
        [SerializeField]
        private string _runtimeValue;

        public PersistentString(string parameterID, string defaultValue) : base(parameterID)
        {
            _runtimeValue = defaultValue;
        }

        public string value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public void SetValue(string value, bool writesPrefsToDisk = false)
        {
            if (!IsLocked)
            {
                PlayerPrefs.SetString(_parameterID, value);
                _runtimeValue = value;

                if (writesPrefsToDisk)
                {
                    PlayerPrefs.Save();
                }
            }
        }

        public void InitializeDefault()
        {
            if (!PlayerPrefs.HasKey(_parameterID))
            {
                SetValue(_runtimeValue);
            }
        }

        public string GetValue()
        {
            if (_isValueUpdated) return _runtimeValue;
            _isValueUpdated = true;
            InitializeDefault();
            _runtimeValue = PlayerPrefs.GetString(_parameterID, _runtimeValue);
            return _runtimeValue;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        public static implicit operator string(PersistentString d) => d.GetValue();
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PersistentString))]
    public class PersistentStringDrawerEditor : PropertyDrawer
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
