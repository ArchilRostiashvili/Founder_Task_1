using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEditorUtilities;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(DisableFieldAttribute))]
public class DisableField : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisableFieldAttribute hideIf = (DisableFieldAttribute)attribute;
        SerializedProperty targetProperty = property.FindSiblingProperty(hideIf.PropertyPath);

        if (targetProperty.boolValue == hideIf.CompareValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

}
#endif

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public class DisableFieldAttribute : PropertyAttribute
{
    public string PropertyPath { get; private set; }
    public bool CompareValue { get; protected set; }

    public DisableFieldAttribute(string propertyPath, bool value)
    {
        PropertyPath = propertyPath;
        CompareValue = value;
    }
}


