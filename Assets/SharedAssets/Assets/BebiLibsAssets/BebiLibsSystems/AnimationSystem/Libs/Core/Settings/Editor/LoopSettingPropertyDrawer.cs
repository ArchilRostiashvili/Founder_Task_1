using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiAnimations.Libs.Core.Settings.Editor
{
    [CustomPropertyDrawer(typeof(LoopSetting))]
    public class LoopSettingPropertyDrawer : PropertyDrawer
    {
        private static string _EnableLoopingName = "_enableLooping";
        private static string _LoopCountName = "_loopCount";
        private static string _LoopTypeName = "_loopType";

        private SerializedProperty _activeProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _activeProperty = property;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardYSpacing = EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty loopToggle = GetPropertyData(_EnableLoopingName);

            if (loopToggle.boolValue)
            {
                return (2 * standardYSpacing) + singleLineHeight + GetPropertyHeight(_LoopCountName) + GetPropertyHeight(_LoopTypeName);
            }
            return base.GetPropertyHeight(loopToggle, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _activeProperty = property;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardYSpacing = EditorGUIUtility.standardVerticalSpacing;
            float lastElementY = 0;
            SerializedProperty loopToggle = GetPropertyData(_EnableLoopingName);

            if (loopToggle.boolValue)
            {
                DrawProperty(_EnableLoopingName, position, 0, out float newHeight, new GUIContent("Loop Enabled"));
                lastElementY += newHeight + standardYSpacing;

                EditorGUI.indentLevel++;

                DrawProperty(_LoopCountName, position, lastElementY, out newHeight);
                lastElementY += newHeight + standardYSpacing;
                DrawProperty(_LoopTypeName, position, lastElementY, out newHeight);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.PropertyField(position, loopToggle, new GUIContent("Loop Disabled"));
            }
        }



        private void DrawProperty(string propertyName, Rect rect, float lastElementY, out float height, GUIContent content = null)
        {
            var newProperty = _activeProperty.FindPropertyRelative(propertyName);
            height = EditorGUI.GetPropertyHeight(newProperty);
            Rect newRect = new Rect(rect.x, rect.y + lastElementY, rect.width, height);
            if (content == null)
            {
                EditorGUI.PropertyField(newRect, newProperty);
            }
            else
            {
                EditorGUI.PropertyField(newRect, newProperty, content);
            }
        }

        public SerializedProperty GetPropertyData(string propertyName)
        {
            var newProperty = _activeProperty.FindPropertyRelative(propertyName);
            return newProperty;
        }

        public SerializedProperty GetPropertyData(string propertyName, out float propertyHeight)
        {
            var newProperty = _activeProperty.FindPropertyRelative(propertyName);
            propertyHeight = EditorGUI.GetPropertyHeight(newProperty);
            return newProperty;
        }

        public float GetPropertyHeight(string propertyName, out SerializedProperty newProperty)
        {
            newProperty = _activeProperty.FindPropertyRelative(propertyName);
            return EditorGUI.GetPropertyHeight(newProperty);
        }

        public float GetPropertyHeight(string propertyName)
        {
            return EditorGUI.GetPropertyHeight(_activeProperty.FindPropertyRelative(propertyName));
        }
    }
}
#endif