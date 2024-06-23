using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AnimationCurveObject", menuName = "BebiToddlers/AnimationCurveObject", order = 0)]
    public class AnimationCurveObject : ScriptableObject
    {
        public AnimationCurve animationCurve;
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(AnimationCurveObject))]
    public class AnimationCurveObjectDrawer : PropertyDrawer
    {

        public static Dictionary<UnityEngine.Object, SerializedObject> properties = new Dictionary<Object, SerializedObject>();

        public AnimationCurveObjectDrawer()
        {
            properties.Clear();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float minButtonWidth = 25f;
            float width = position.width;
            float leftWidth = width - (minButtonWidth * 2);

            Rect propertyPosition = new Rect(position.x, position.y, leftWidth, position.height);
            Rect highlightButtonPosition = new Rect(position.x + leftWidth, position.y, minButtonWidth, position.height);
            Rect removeButtonPosition = new Rect(position.x + leftWidth + minButtonWidth, position.y, minButtonWidth, position.height);


            if (property.objectReferenceValue == null)
            {
                EditorGUI.ObjectField(position, property, label);
            }
            else
            {
                if (properties.TryGetValue(property.objectReferenceValue, out SerializedObject obj) == false)
                {
                    obj = new SerializedObject(property.objectReferenceValue);
                    properties.Add(property.objectReferenceValue, obj);
                }
                else if (obj == null)
                {
                    obj = new SerializedObject(property.objectReferenceValue);
                    properties[property.objectReferenceValue] = obj;
                    Debug.Log("Fuck");
                }

                SerializedProperty newProperty = obj.FindProperty("animationCurve");

                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(propertyPosition, newProperty, new GUIContent(property.displayName), false);
                if (EditorGUI.EndChangeCheck())
                {
                    obj.ApplyModifiedProperties();
                }


                if (GUI.Button(highlightButtonPosition, "H"))
                {
                    EditorGUIUtility.PingObject(property.objectReferenceValue);
                }

                if (GUI.Button(removeButtonPosition, "-"))
                {
                    property.objectReferenceValue = null;
                }
            }

        }
    }
#endif
}
