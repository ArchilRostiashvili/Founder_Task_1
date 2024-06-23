
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomEditorUtilities
{
    public static class PropertyDrawerExtensions
    {
        public static SerializedProperty GetParent(this SerializedProperty aProperty)
        {
            var path = aProperty.propertyPath;
            int i = path.LastIndexOf('.');
            if (i < 0)
                return null;
            return aProperty.serializedObject.FindProperty(path.Substring(0, i));
        }

        public static SerializedProperty FindSiblingProperty(this SerializedProperty aProperty, string aPath)
        {
            var parent = GetParent(aProperty);
            if (parent == null)
                return aProperty.serializedObject.FindProperty(aPath);
            return parent.FindPropertyRelative(aPath);
        }


        public static List<System.Type> GetChildType<T>()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract).ToList();
        }


        public static List<System.Type> GetChildType(System.Type classType)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => classType.IsAssignableFrom(type) && !type.IsAbstract).ToList();
        }

        public static void DrawSerializedObject(this SerializedObject serializedObject, Rect baseRect, float spacing = 0, bool showScript = false, int indentLevel = 0)
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

                rect.Set(baseRect.x, baseRect.y += height + spacing, baseRect.width, height);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static float GetSerializedObjectSize(this SerializedObject serializedObject, float spacing = 0, bool showScript = false)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            float size = 0;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.propertyPath == "m_Script" && !showScript) continue;
                size += EditorGUI.GetPropertyHeight(iterator, true) + spacing;
            }
            return size;
        }
    }
}


#endif
