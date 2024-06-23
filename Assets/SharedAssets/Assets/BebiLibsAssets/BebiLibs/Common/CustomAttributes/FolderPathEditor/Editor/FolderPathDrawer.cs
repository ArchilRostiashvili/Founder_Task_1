
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CustomEditorUtilities;

namespace BebiLibs
{
    [CustomPropertyDrawer(typeof(FolderPathViewer))]
    public class FolderPathDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float buttonWidth = 40;
            float offset = 5;
            float width = position.width - buttonWidth;
            float height = position.height;

            Rect elementPosition = new Rect(position.x, position.y, width, height);
            Rect buttonRect = new Rect(position.x + width + offset, position.y, buttonWidth - offset, height);

            FolderPathViewer labelAttribute = attribute as FolderPathViewer;

            EditorGUI.PropertyField(elementPosition, property, label, true);

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_Folder Icon")))
            {
                SelectPath(property, labelAttribute);
            }
        }

        private void SelectPath(SerializedProperty property, FolderPathViewer labelAttribute)
        {
            string defaultPath = string.IsNullOrEmpty(property.stringValue) ? labelAttribute.ParentFolder : property.stringValue;
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder", defaultPath, "");
            folderPath = ValidateSelectedPath(labelAttribute, defaultPath, folderPath);

            property.stringValue = folderPath;
            property.serializedObject.ApplyModifiedProperties();
            GUIUtility.ExitGUI();
        }

        private static string ValidateSelectedPath(FolderPathViewer labelAttribute, string defaultPath, string folderPath)
        {
            switch (labelAttribute.FolderPathType)
            {
                case FolderPathType.Relative:
                    if (!UnityFileUtils.TryConvertToProjectRelativePath(folderPath, out folderPath))
                    {
                        folderPath = defaultPath;
                        Debug.LogWarning("You Should Assign Project Relative Path");
                    }
                    break;
                case FolderPathType.Absolute:
                    break;
                default:
                    Debug.Log("Not Implemented");
                    break;
            }

            return folderPath;
        }


    }
}
#endif