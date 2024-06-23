using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace BebiAnimations.Libs.Core.Settings.Editor
{
    [CustomPropertyDrawer(typeof(EaseSetting))]
    public class EaseSettingPropertyDrawer : PropertyDrawer
    {
        private static string _EaseTypeName = "_easeType";
        private static string _TweenEasingName = "_tweenEasing";
        private static string _CurveEasingName = "_curveEasing";
        private static string _OvershootName = "_overshoot";
        private static string _AmplitudeName = "_amplitude";
        private static string _PeriodName = "_period";

        private SerializedProperty _activeProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _activeProperty = property;
            EaseType easeType = GetEaseType(property);

            float lastHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var EaseTypeProperty = property.FindPropertyRelative(_EaseTypeName);
                float easyTypeHeight = EditorGUI.GetPropertyHeight(EaseTypeProperty);
                return lastHeight + easyTypeHeight + GetHeight(easeType, property);
            }
            return lastHeight;
        }

        private float GetHeight(EaseType easeType, SerializedProperty property)
        {
            float standardYSpacing = EditorGUIUtility.standardVerticalSpacing;
            return easeType switch
            {
                EaseType.Default => GetPropertyHeight(_TweenEasingName) + standardYSpacing,
                EaseType.AnimationCurve => GetPropertyHeight(_CurveEasingName) + standardYSpacing,
                EaseType.WithOvershoot => GetPropertyHeight(_TweenEasingName) + GetPropertyHeight(_OvershootName) + (2 * standardYSpacing),
                EaseType.WithAmplitude => GetPropertyHeight(_TweenEasingName) + GetPropertyHeight(_AmplitudeName) + GetPropertyHeight(_PeriodName) + (3 * standardYSpacing),
                _ => 0,
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _activeProperty = property;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardYSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect foldoutRect = position;
            if (property.isExpanded)
            {
                foldoutRect = new Rect(position.x, position.y, position.width, singleLineHeight);
            }

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, property.displayName);


            float lastElementY = singleLineHeight + standardYSpacing;

            if (property.isExpanded)
            {
                Rect easeTypeRect = new Rect(position.x, position.y + lastElementY, position.width, EditorGUIUtility.singleLineHeight);
                var easeTypeProperty = property.FindPropertyRelative(_EaseTypeName);
                lastElementY += singleLineHeight + standardYSpacing;

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(easeTypeRect, easeTypeProperty);

                EaseType easeType = (EaseType)easeTypeProperty.intValue;
                switch (easeType)
                {
                    case EaseType.Default:
                        DrawProperty(_TweenEasingName, position, lastElementY, out _);
                        break;
                    case EaseType.AnimationCurve:
                        DrawProperty(_CurveEasingName, position, lastElementY, out _);
                        break;
                    case EaseType.WithOvershoot:
                        DrawProperty(_TweenEasingName, position, lastElementY, out float tweenEaseHeight);
                        lastElementY += tweenEaseHeight + standardYSpacing;
                        DrawProperty(_OvershootName, position, lastElementY, out _);
                        break;
                    case EaseType.WithAmplitude:
                        DrawProperty(_TweenEasingName, position, lastElementY, out tweenEaseHeight);
                        lastElementY += tweenEaseHeight + standardYSpacing;
                        DrawProperty(_AmplitudeName, position, lastElementY, out float amplitudeHeight);
                        lastElementY += amplitudeHeight + standardYSpacing;
                        DrawProperty(_PeriodName, position, lastElementY, out tweenEaseHeight);
                        break;
                }

                EditorGUI.indentLevel--;
            }
        }


        private void DrawProperty(string propertyName, Rect rect, float lastElementY, out float height)
        {
            var newProperty = _activeProperty.FindPropertyRelative(propertyName);
            height = EditorGUI.GetPropertyHeight(newProperty);
            Rect newRect = new Rect(rect.x, rect.y + lastElementY, rect.width, height);
            EditorGUI.PropertyField(newRect, newProperty);
        }

        private EaseType GetEaseType(SerializedProperty property)
        {
            return (EaseType)property.FindPropertyRelative(_EaseTypeName).intValue;
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