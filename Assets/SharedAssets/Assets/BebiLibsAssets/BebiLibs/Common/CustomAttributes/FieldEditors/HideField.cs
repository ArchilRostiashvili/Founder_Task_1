using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using CustomEditorUtilities;

[CustomPropertyDrawer(typeof(HideFieldAttribute))]
public class HideField : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HideFieldAttribute hideIf = (HideFieldAttribute)attribute;
        SerializedProperty targetProperty = property.FindSiblingProperty(hideIf.PropertyPath);

        if (targetProperty.boolValue == hideIf.CompareValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HideFieldAttribute hideIf = (HideFieldAttribute)attribute;
        SerializedProperty targetProperty = property.FindSiblingProperty(hideIf.PropertyPath);

        if (targetProperty.boolValue == hideIf.CompareValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

}
#endif

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public class HideFieldAttribute : PropertyAttribute
{
    public string PropertyPath { get; private set; }
    public bool CompareValue { get; protected set; }

    public HideFieldAttribute(string propertyPath, bool value = true)
    {
        PropertyPath = propertyPath;
        CompareValue = value;
    }
}


