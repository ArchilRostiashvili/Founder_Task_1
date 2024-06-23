using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    [System.Serializable]
    public abstract class Referenceable
    {
        [SerializeField] protected string elementType;
        [SerializeField] protected string assetResourcePath;

        public string assetPath => assetResourcePath;

#if UNITY_EDITOR
        public static string GetElementPath(UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.Contains("/Resources/"))
            {
                string resPathName = "/Resources/";
                string[] paths = path.Split(resPathName.ToCharArray());
                if (paths.Length == 0 || paths.Length > 2)
                {
                    Debug.LogWarning($"Something went wrong when spliting Path {path}");
                }
                string pathNextResource = paths[paths.Length - 1];
                return Path.Combine(Path.GetDirectoryName(pathNextResource), Path.GetFileNameWithoutExtension(pathNextResource));
            }
            else
            {
                Debug.LogWarning($"Path {path} is not Resource Path");
                return path;
            }
        }
#endif
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Referenceable), true)]
    public class ReferenceableEditor : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetObject = property.FindPropertyRelative("assetReference");

            if (assetObject.objectReferenceValue != null)
            {
                SerializedProperty text = property.FindPropertyRelative("assetResourcePath");
                text.stringValue = Referenceable.GetElementPath(assetObject.objectReferenceValue);
                string filename = Path.GetFileNameWithoutExtension(text.stringValue);
                EditorGUI.ObjectField(position, assetObject, assetObject.objectReferenceValue.GetType(), new GUIContent(filename, text.stringValue));
            }
            else
            {
                SerializedProperty type = property.FindPropertyRelative("elementType");
                System.Type elementType = System.Type.GetType(type.stringValue);
                EditorGUI.ObjectField(position, assetObject, elementType, new GUIContent(property.displayName));
            }
        }
    }
#endif
}
