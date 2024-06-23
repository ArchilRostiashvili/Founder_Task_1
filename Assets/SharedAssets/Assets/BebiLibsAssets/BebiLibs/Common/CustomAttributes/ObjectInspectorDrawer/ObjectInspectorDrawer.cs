using UnityEngine;
using CustomEditorUtilities;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiLibs
{
    [CustomPropertyDrawer(typeof(ObjectInspectorAttribute))]
    public class ObjectInspectorDrawer : PropertyDrawer
    {
        private Object _targetObject;
        private SerializedObject _targetSerializedObject;

        private const float Spacing = 1f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null && property.isExpanded)
            {
                GetSerializedObject(property);
                return EditorGUI.GetPropertyHeight(property) + _targetSerializedObject.GetSerializedObjectSize(Spacing);
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        private void GetSerializedObject(SerializedProperty property)
        {
            if (_targetObject != property.objectReferenceValue && property.objectReferenceValue != null)
            {
                _targetObject = property.objectReferenceValue;
                _targetSerializedObject = new SerializedObject(_targetObject);
            }
        }



        public void DrawSerializedObject(Rect baseRect, SerializedObject serializedObject, bool showScript = false, int indentLevel = 0)
        {
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                float height = EditorGUI.GetPropertyHeight(iterator);
                Rect rect = new Rect(baseRect.x, baseRect.y, baseRect.width, height);
                if (iterator.propertyPath == "m_Script" && !showScript) continue;

                EditorGUI.indentLevel += indentLevel;
                EditorGUI.PropertyField(rect, iterator, true);
                EditorGUI.indentLevel -= indentLevel;

                rect.Set(baseRect.x, baseRect.y += height + Spacing, baseRect.width, height);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ObjectInspectorAttribute oia = (ObjectInspectorAttribute)this.attribute;

            if (property.objectReferenceValue != null)
            {
                float height = base.GetPropertyHeight(property, label);
                Rect startRect = new Rect(position.x, position.y, position.width, height);
                property.isExpanded = EditorGUI.Foldout(startRect, property.isExpanded, string.Empty);
                EditorGUI.PropertyField(startRect, property, new GUIContent(property.displayName), true);

                if (property.isExpanded)
                {
                    GUI.enabled = !oia.IsReadOnly;
                    GetSerializedObject(property);
                    startRect = new Rect(position.x, position.y += height + Spacing, position.width, height);
                    DrawSerializedObject(startRect, _targetSerializedObject, false, 1);
                }
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}

#endif

