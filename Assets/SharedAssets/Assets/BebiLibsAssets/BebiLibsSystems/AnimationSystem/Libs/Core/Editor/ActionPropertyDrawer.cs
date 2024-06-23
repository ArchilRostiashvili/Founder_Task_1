using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiAnimations.Libs.Core
{
    // [CustomPropertyDrawer(typeof(Action))]
    // public class ActionPropertyDrawer : PropertyDrawer
    // {
    //     public override float GetPropertyHeight(SerializedProperty actionProperty, GUIContent label)
    //     {
    //         bool hasObjectReference = actionProperty.objectReferenceValue != null;
    //         var elementHeight = EditorGUI.GetPropertyHeight(actionProperty);
    //         var margin = EditorGUIUtility.standardVerticalSpacing;

    //         if (actionProperty.isExpanded && hasObjectReference)
    //         {
    //             elementHeight = (elementHeight * 2) + margin;
    //             SerializedObject actionIterator = new SerializedObject(actionProperty.objectReferenceValue);
    //             SerializedProperty property = actionIterator.GetIterator();
    //             SerializedProperty nextSiblingProperty = property.Copy();
    //             {
    //                 nextSiblingProperty.Next(true);
    //             }

    //             if (property.NextVisible(true))
    //             {
    //                 do
    //                 {
    //                     if (SerializedProperty.EqualContents(property, nextSiblingProperty)) break;
    //                     if (property.propertyPath == "m_Script") continue;
    //                     elementHeight += EditorGUI.GetPropertyHeight(property) + margin;
    //                 }
    //                 while (property.NextVisible(false));
    //             }
    //             return elementHeight + margin;
    //         }

    //         return elementHeight;
    //     }

    //     public override void OnGUI(Rect rect, SerializedProperty actionProperty, GUIContent label)
    //     {
    //         bool hasObjectReference = actionProperty.objectReferenceValue != null;
    //         bool canDisplayContent = actionProperty.isExpanded && hasObjectReference;

    //         string propertyName = hasObjectReference ? actionProperty.objectReferenceValue.name.Split('.')[^1] : actionProperty.displayName;

    //         float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
    //         float actionPropertyHeight = EditorGUI.GetPropertyHeight(actionProperty);

    //         float lastElementY = verticalSpacing + 2 * actionPropertyHeight;
    //         float actionRectY = canDisplayContent ? rect.y + actionPropertyHeight : rect.y;

    //         Rect actionPropertyRect = new Rect(rect.x, actionRectY, rect.width, actionPropertyHeight);
    //         Rect foldoutRect = new Rect(rect.x, rect.y, rect.width, actionPropertyHeight);

    //         if (hasObjectReference)
    //         {
    //             string nameToDisplay = actionProperty.isExpanded ? propertyName : string.Empty;
    //             actionProperty.isExpanded = EditorGUI.Foldout(foldoutRect, actionProperty.isExpanded, nameToDisplay);
    //         }

    //         EditorGUI.indentLevel += canDisplayContent ? 1 : 0;

    //         EditorGUI.BeginChangeCheck();
    //         string displayName = canDisplayContent ? actionProperty.objectReferenceValue.GetType().Name : propertyName;
    //         EditorGUI.PropertyField(actionPropertyRect, actionProperty, new GUIContent(displayName), true);
    //         if (EditorGUI.EndChangeCheck())
    //         {
    //             actionProperty.serializedObject.ApplyModifiedProperties();
    //         }

    //         EditorGUI.indentLevel -= canDisplayContent ? 1 : 0;


    //         if (canDisplayContent)
    //         {
    //             SerializedObject actionObject = new SerializedObject(actionProperty.objectReferenceValue);
    //             SerializedProperty localProperty = actionObject.GetIterator();
    //             SerializedProperty nextSiblingProperty = localProperty.Copy();
    //             {
    //                 nextSiblingProperty.Next(true);
    //             }

    //             if (localProperty.NextVisible(true))
    //             {
    //                 do
    //                 {
    //                     if (SerializedProperty.EqualContents(localProperty, nextSiblingProperty)) break;
    //                     if (localProperty.propertyPath == "m_Script") continue;

    //                     float localPropertyHeight = EditorGUI.GetPropertyHeight(localProperty);
    //                     Rect localPropertyRect = new Rect(rect.x, rect.y + lastElementY, rect.width, localPropertyHeight);
    //                     lastElementY += verticalSpacing + localPropertyHeight;
    //                     EditorGUI.indentLevel++;
    //                     EditorGUI.BeginChangeCheck();
    //                     EditorGUI.PropertyField(localPropertyRect, localProperty);

    //                     if (EditorGUI.EndChangeCheck())
    //                     {
    //                         actionObject.ApplyModifiedProperties();
    //                     }
    //                     EditorGUI.indentLevel--;
    //                 }
    //                 while (localProperty.NextVisible(false));
    //             }
    //         }
    //     }
    // }
}

#endif


