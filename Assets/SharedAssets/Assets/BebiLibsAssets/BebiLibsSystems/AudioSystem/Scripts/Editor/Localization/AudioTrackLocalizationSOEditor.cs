using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System;
using BebiLibs.Localization.Core;
using System.Linq;
using BebiLibs.Localization;
using UnityEngine.AddressableAssets;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiLibs.AudioSystem
{
    [CustomEditor(typeof(LocalizedAudioTrackSO), true)]
    public class AudioTrackLocalizationSOEditor : Editor
    {
        private LocalizedAudioTrackSO _audioTrackLocalizationSO;
        private SerializedProperty _localizedTrackDataList;

        private SmartReorderableList<LanguageIdentifier> _smartReorderableList;
        private string _localizedTrackDataListKey;
        private bool _valuesChanged = false;

        private void OnEnable()
        {
            _localizedTrackDataListKey = LocalizedAudioTrackSO.localizationTrackListKey;
            _audioTrackLocalizationSO = (LocalizedAudioTrackSO)target;
            _localizedTrackDataList = serializedObject.FindProperty(_localizedTrackDataListKey);

            List<LanguageIdentifier> excludeList = new List<LanguageIdentifier> { LocalizationManager.English };
            //List<LanguageIdentifier> excludeList = new List<LanguageIdentifier>();
            List<LanguageIdentifier> usedIdentifiers = _audioTrackLocalizationSO.LocalizedTrackDataList.Select(x => x.LanguageIdentifier).ToList();
            string userPropertyNameKey = LocalizedAudioTrackData.LanguageIdentifierKey;
            string audioTrackReference = LocalizedAudioTrackData.AudioTrackReferenceKey;

            _smartReorderableList = new SmartReorderableList<LanguageIdentifier>(serializedObject, _localizedTrackDataList, usedIdentifiers, excludeList, userPropertyNameKey, audioTrackReference)
            {
                OnNewElementCreate = OnCreate,
                OnNewElementCreateEnds = OnCreateEnds,
                OnRemoveEnds = OnRemoveEnd,
                OnRemoveStart = OnRemoveStart
            };

            UpdateContextPanel();
            ValidateAddressableGroups();
        }

        private void OnRemoveStart(int index)
        {

        }

        public void UpdateContextPanel()
        {
            List<LanguageIdentifier> usedIdentifiers = _audioTrackLocalizationSO.LocalizedTrackDataList.Select(x => x.LanguageIdentifier).ToList();
            _smartReorderableList.UpdateList(usedIdentifiers);
        }

        private void OnRemoveEnd()
        {
            UpdateContextPanel();
        }

        private void OnCreate(LanguageIdentifier obj, SerializedProperty addedElementProperty)
        {
            addedElementProperty.FindPropertyRelative(LocalizedAudioTrackData.LanguageIdentifierKey).objectReferenceValue = (LanguageIdentifier)obj;
        }


        private void OnCreateEnds(LanguageIdentifier arg1, SerializedProperty arg2)
        {
            UpdateContextPanel();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawPropertiesExcluding(serializedObject, _localizedTrackDataListKey);

            if (_audioTrackLocalizationSO.AudioTrackGroup == null)
            {
                EditorGUILayout.HelpBox("AudioTrackGroup is null, Set it before Assigning Values", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (_audioTrackLocalizationSO.AudioTrackGroup.IsNullOrEmpty())
            {
                EditorGUILayout.HelpBox("AudioTrackGroup is not valid, it's null or empty", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (_valuesChanged)
            {
                ValidateAddressableGroups();
                _valuesChanged = false;
            }

            EditorGUI.BeginChangeCheck();
            _localizedTrackDataList.isExpanded = EditorGUILayout.Foldout(_localizedTrackDataList.isExpanded, _localizedTrackDataList.displayName);
            if (_localizedTrackDataList.isExpanded)
            {
                _smartReorderableList.DrawList();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _valuesChanged = true;
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateAddressableGroups()
        {
            LocalizedTrackFixer.ValidateAddressableGroups(_audioTrackLocalizationSO);
        }
    }
}

#endif


