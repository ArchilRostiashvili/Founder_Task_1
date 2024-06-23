// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif


// namespace BebiLibs
// {
// #if UNITY_EDITOR
//     [CustomPropertyDrawer(typeof(RemoteVariable), true)]
//     public class RemoteVariableDrawer : PropertyDrawer
//     {
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return base.GetPropertyHeight(property, label) * 2.5f;
//         }

//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);

//             float width = position.width;
//             Rect variableNameRect = new Rect(position.x, position.y, width * 0.4f, position.height);
//             Rect eventRect = new Rect(position.x, position.y + position.height / 2, width, position.height);
//             Rect typeRect = new Rect(position.x + width * 0.41f, position.y - position.height / 3, width * 0.2f, position.height);
//             Rect valueRect = new Rect(position.x + width * 0.55f, position.y, (width - 30) * 0.40f, position.height);
//             Rect toggleRect = new Rect(position.x + width - 30, position.y, 30, position.height);

//             SerializedProperty ID = property.FindPropertyRelative("dataVariableKey");
//             SerializedProperty IDEventName = property.FindPropertyRelative("eventName");

//             SerializedProperty value = this.DisplayValue(property);

//             SerializedProperty isPresistent = property.FindPropertyRelative("_isEnabled");
//             if (ID != null)
//             {
//                 EditorGUI.PropertyField(variableNameRect, ID, GUIContent.none, true);
//             }

//             if (IDEventName != null)
//             {
//                 EditorGUI.PropertyField(eventRect, IDEventName, new GUIContent("Event Name"), true);
//             }

//             if (value != null)
//             {
//                 EditorGUI.LabelField(typeRect, value.propertyType.ToString());
//             }

//             if (value != null)
//             {
//                 EditorGUI.PropertyField(valueRect, value, GUIContent.none, true);
//             }

//             if (isPresistent != null)
//             {
//                 EditorGUI.PropertyField(toggleRect, isPresistent, GUIContent.none, true);
//             }

//             EditorGUI.EndProperty();
//         }


//         public SerializedProperty DisplayValue(SerializedProperty property)
//         {
//             return property.FindPropertyRelative("value");
//         }
//     }
// #endif
// }
