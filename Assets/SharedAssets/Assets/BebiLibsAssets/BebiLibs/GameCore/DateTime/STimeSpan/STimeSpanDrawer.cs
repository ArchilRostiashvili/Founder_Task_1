using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace BebiLibs
{
    [CustomPropertyDrawer(typeof(STimeSpan))]
    public class STimeSpanDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return base.GetPropertyHeight(property, label) * 3 + 3;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        private static int _maxSizeRecalculationIntensity = 5;
        private static int _sizeRecalculationIntensity = 5;

        public static float maxLabelWidth;

        public static GUIContent dayContent = new GUIContent("Day");

        public static GUIContent hoursContent = new GUIContent("Hours");
        public static GUIContent minutesContent = new GUIContent("Minutes");
        public static GUIContent secondsContent = new GUIContent("Seconds");

        public static List<GUIContent> arrayContents = new List<GUIContent>();

        public void CalculateContentSizes()
        {
            if (_sizeRecalculationIntensity >= _maxSizeRecalculationIntensity)
            {
                if (arrayContents.Count == 0)
                {
                    arrayContents.Add(dayContent);
                    arrayContents.Add(hoursContent);
                    arrayContents.Add(minutesContent);
                    arrayContents.Add(secondsContent);
                }

                foreach (var item in arrayContents)
                {
                    float newMaxWidth = GUI.skin.toggle.CalcSize(item).x;
                    if (newMaxWidth > maxLabelWidth)
                    {
                        maxLabelWidth = newMaxWidth;
                    }
                }
                _sizeRecalculationIntensity = 0;
            }
            _sizeRecalculationIntensity++;
        }

        public void DisplayPopUp(Rect position, SerializedProperty property)
        {
            Rect indentedRect = EditorGUI.IndentedRect(position);
            float indentSize = indentedRect.x - position.x;
            float contentPart = Mathf.Clamp(position.width - EditorGUIUtility.labelWidth + indentSize, 200, 100000);
            float mainLabelPart = Mathf.Clamp(position.width - contentPart, 10, 100000);
            float timeSize = mainLabelPart * 0.9f;
            float kindSize = mainLabelPart * 0.5f;
            float valueStartPos = position.x + position.width - contentPart;

            float halfHeight = property.isExpanded ? (position.height - 3) / 3 : position.height;
            SerializedProperty displayTime = property.FindPropertyRelative("_displayTime");
            Rect foldOutRect = new Rect(position.x, position.y, timeSize, halfHeight);
            Rect displayTimeRect = new Rect(valueStartPos, position.y, timeSize, halfHeight);
            Rect kindRect = new Rect(valueStartPos + timeSize, position.y, kindSize, halfHeight);

            EditorGUI.LabelField(displayTimeRect, displayTime.stringValue);

            property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, property.displayName);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.CalculateContentSizes();
            this.DisplayPopUp(position, property);

            if (property.isExpanded)
            {
                SerializedProperty day = property.FindPropertyRelative("_day");

                SerializedProperty hours = property.FindPropertyRelative("_hours");
                SerializedProperty minutes = property.FindPropertyRelative("_minutes");
                SerializedProperty seconds = property.FindPropertyRelative("_seconds");

                float part = position.width * 0.33333f;
                float halfHeight = (position.height - 3) / 3;

                float valueStartPosX = position.x;
                float valueStartPosY = position.y + halfHeight + 1;

                Rect dayVRect = new Rect(valueStartPosX, valueStartPosY, part, halfHeight);

                Rect hoursRect = new Rect(valueStartPosX, valueStartPosY + halfHeight + 1, part, halfHeight);
                Rect minutesRect = new Rect(valueStartPosX + part, valueStartPosY + halfHeight + 1, part, halfHeight);
                Rect secondsRect = new Rect(valueStartPosX + part * 2, valueStartPosY + halfHeight + 1, part, halfHeight);

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = maxLabelWidth;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(dayVRect, day, dayContent);
                EditorGUI.PropertyField(hoursRect, hours, hoursContent);
                EditorGUI.PropertyField(minutesRect, minutes, minutesContent);
                EditorGUI.PropertyField(secondsRect, seconds, secondsContent);
                EditorGUIUtility.labelWidth = oldLabelWidth;

                if (EditorGUI.EndChangeCheck())
                {
                    SerializedProperty serializedTime = property.FindPropertyRelative("_timeSpanInSeconds");
                    try
                    {
                        TimeSpan time = new TimeSpan(day.intValue, hours.intValue, minutes.intValue, seconds.intValue);
                        serializedTime.doubleValue = time.TotalSeconds;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
#endif