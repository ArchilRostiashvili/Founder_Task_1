using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiLibs.PlayerPreferencesSystem
{
    [CustomPropertyDrawer(typeof(Preference))]
    public class PreferenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            string keyPathName = nameof(Preference.Key);
            SerializedProperty keyProp = property.FindPropertyRelative(keyPathName);

            if (!property.isExpanded)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
                EditorGUI.PropertyField(position, keyProp, label);
                EditorGUI.EndProperty();
                return;
            }

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float height = singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var keyRect = new Rect(position.x, position.y, position.width, singleLineHeight);
            var typeRect = new Rect(position.x, position.y + height, position.width, singleLineHeight);
            var sourceRect = new Rect(position.x, position.y + height * 2, position.width, singleLineHeight);
            var valueRect = new Rect(position.x, position.y + height * 3, position.width, singleLineHeight);


            string propertyTypePathName = nameof(Preference.PreferenceType);
            string SourceTypePathName = nameof(Preference.SourceType);

            string boolValuePathName = nameof(Preference.BoolValue);
            string longValuePathName = nameof(Preference.LongValue);
            string doubleValuePathName = nameof(Preference.DoubleValue);
            string stringValuePathName = nameof(Preference.StringValue);

            SerializedProperty typeProp = property.FindPropertyRelative(propertyTypePathName);
            SerializedProperty sourceProp = property.FindPropertyRelative(SourceTypePathName);

            property.isExpanded = EditorGUI.Foldout(keyRect, property.isExpanded, GUIContent.none);
            EditorGUI.PropertyField(keyRect, keyProp, label);
            EditorGUI.PropertyField(typeRect, typeProp, new GUIContent(propertyTypePathName));
            EditorGUI.PropertyField(sourceRect, sourceProp, new GUIContent(SourceTypePathName));

            GUIContent content = new GUIContent("Value");

            var type = (PreferenceDataType)typeProp.enumValueIndex;
            switch (type)
            {
                case PreferenceDataType.Bool:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(boolValuePathName), content);
                    break;
                case PreferenceDataType.Long:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(longValuePathName), content);
                    break;
                case PreferenceDataType.Double:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(doubleValuePathName), content);
                    break;
                case PreferenceDataType.String:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(stringValuePathName), content);
                    break;
            }

            EditorGUI.EndProperty();
        }
    }

}
#endif