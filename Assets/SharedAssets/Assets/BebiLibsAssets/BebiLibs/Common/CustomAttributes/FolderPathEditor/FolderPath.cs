using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
using CustomEditorUtilities;
#endif


namespace BebiLibs
{
    [System.Serializable]
    public class FolderPath : ISerializationCallbackReceiver
    {
        [SerializeField] internal string _folderPath;
        [SerializeField] internal string _folderAbsolutePath;
        [SerializeField] internal string _folderGuid;
        [SerializeField] internal FolderPathType _folderPathType;

        public string Value => _folderPath;
        public bool Exists => !string.IsNullOrEmpty(_folderPath) && System.IO.Directory.Exists(_folderPath);

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_folderPathType == FolderPathType.GUID)
            {
                if (!string.IsNullOrEmpty(_folderGuid))
                    _folderPath = AssetDatabase.GUIDToAssetPath(_folderGuid);
            }
#endif
        }

        public static implicit operator string(FolderPath folderPathObject)
        {
            return folderPathObject.Value;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FolderPath))]
    public class FolderPathEditorDraw : PropertyDrawer
    {
        private const string _FOLDER_ICON_KEY = "d_Folder Icon";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float buttonWidth = 40;
            const float popupWidth = 70;
            const float offset = 5;

            float propertyWidth = position.width - buttonWidth - popupWidth - (2 * offset);
            float height = position.height;

            Rect propertyRect = new Rect(position.x, position.y, propertyWidth, height);
            Rect popupRect = new Rect(propertyRect.xMax + offset, position.y, popupWidth, height);
            Rect buttonRect = new Rect(popupRect.xMax + offset, position.y, buttonWidth, height);

            SerializedProperty folderPathProp = property.FindPropertyRelative(nameof(FolderPath._folderPath));
            SerializedProperty folderGUID = property.FindPropertyRelative(nameof(FolderPath._folderGuid));
            SerializedProperty folderAbsolutePathProp = property.FindPropertyRelative(nameof(FolderPath._folderAbsolutePath));
            SerializedProperty folderPathTypeProp = property.FindPropertyRelative(nameof(FolderPath._folderPathType));
            FolderPathType folderPathType = (FolderPathType)folderPathTypeProp.enumValueIndex;

            bool wasEnabled = GUI.enabled;
            GUI.enabled = folderPathType == FolderPathType.Absolute;
            EditorGUI.PropertyField(propertyRect, folderPathProp, label);
            GUI.enabled = wasEnabled;


            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(popupRect, folderPathTypeProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                //:TODO: Implement Change Validator
            }


            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent(_FOLDER_ICON_KEY)))
            {
                string newPath = EditorUtility.OpenFolderPanel("Select Folder", folderPathProp.stringValue, "");
                ValidateSelectedPath(newPath, folderPathProp, folderGUID, folderPathTypeProp, folderAbsolutePathProp);
                property.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
        }

        private static void ValidateSelectedPath(string newPath, SerializedProperty folderPathProp, SerializedProperty folderGUIDProp, SerializedProperty folderPathTypeProp, SerializedProperty folderAbsolutePathProp)
        {
            FolderPathType folderPathType = (FolderPathType)folderPathTypeProp.enumValueIndex;

            folderAbsolutePathProp.stringValue = newPath;
            switch (folderPathType)
            {
                case FolderPathType.Relative:
                    if (!UnityFileUtils.TryConvertToProjectRelativePath(newPath, out newPath))
                    {
                        Debug.LogError($"You Should Assign Project Relative Path, {newPath} is not a valid path");
                    }
                    break;
                case FolderPathType.Absolute:
                    break;
                case FolderPathType.GUID:
                    if (UnityFileUtils.TryConvertToProjectRelativePath(newPath, out newPath))
                    {
                        folderGUIDProp.stringValue = AssetDatabase.AssetPathToGUID(newPath);
                    }
                    else
                    {
                        Debug.LogError($"You Should Assign Project Relative Path, {newPath} is not a valid path");
                    }
                    break;
                default:
                    Debug.Log("Not Implemented");
                    break;
            }

            folderPathProp.stringValue = newPath;
        }

    }
#endif
}
