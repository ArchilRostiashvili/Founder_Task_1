using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BebiLibs
{
    [System.Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        [SerializeField] private string _sceneName;

#if UNITY_EDITOR
        [SerializeField] private string _guid;
        [System.NonSerialized] private string _lastGuid = string.Empty;
#endif

        public string SceneName => _sceneName;
        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference.SceneName;
        }

#if UNITY_EDITOR

        public SceneAsset GetSceneAsset()
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(_guid));
        }


        public void SetSceneAsset(SceneAsset sceneAsset)
        {
            _guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset));
        }
#endif

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR        
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (string.IsNullOrEmpty(_guid)) return;
            if (!string.Equals(_guid, _lastGuid))
            {
                SceneAsset sceneAsset = GetSceneAsset();
                if (sceneAsset != null)
                {
                    _sceneName = sceneAsset.name;
                }
                _lastGuid = _guid;
            }
#endif
        }

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private string _activeGUID;
        private SceneAsset _activeSceneAsset;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //SerializedProperty sceneAssetProperty = property.FindPropertyRelative("_sceneAsset");
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("_guid");
            if (sceneAssetProperty != null)
            {
                return EditorGUI.GetPropertyHeight(sceneAssetProperty);
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("_guid");

            SceneAsset sceneAsset = LoadSceneAsset(sceneAssetProperty.stringValue);
            SceneAsset newSceneAsset = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false) as SceneAsset;
            if (sceneAsset != newSceneAsset)
            {
                sceneAssetProperty.stringValue = GetGUID(newSceneAsset);
                _activeGUID = sceneAssetProperty.stringValue;
                _activeSceneAsset = newSceneAsset;
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }

        private string GetGUID(SceneAsset newSceneAsset)
        {
            if (newSceneAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(newSceneAsset);
                return AssetDatabase.AssetPathToGUID(path);
            }
            else
            {
                return string.Empty;
            }
        }

        private SceneAsset LoadSceneAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            if (guid == _activeGUID)
            {
                return _activeSceneAsset;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (sceneAsset == null)
            {
                return null;
            }

            _activeGUID = guid;
            _activeSceneAsset = sceneAsset;
            return sceneAsset;
        }
    }
#endif
}
