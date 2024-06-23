using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using CustomEditorUtilities;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs.Localization.LocalizationEditor
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalAudioData), true)]
    public class LocalAudioDataEditorEditor : Editor
    {
        private LocalAudioData _localAudioDataEditor;
        public delegate void ProcessLocalAudioDataDelegate(LocalAudioData localAudioData);

        private void OnEnable()
        {
            _localAudioDataEditor = (LocalAudioData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Regenerate Addressable Groups") && EditorUtility.DisplayDialog("Warning", "This Will Regenerate Addressable Groups", "Ok", "Cancel"))
            {
                RegenerateAddressableGroups(_localAudioDataEditor);
            }

            if (GUILayout.Button("Split into Modules") && EditorUtility.DisplayDialog("Warning", "This Will Create New Folder Structure from module path and remove Addressable References from Object", "Ok", "Cancel"))
            {
                SplitIntoModuleAssets(_localAudioDataEditor);
            }

            if (GUILayout.Button("Regenerate ALL Addressable Groups") && EditorUtility.DisplayDialog("Warning", "This Will Regenerate ALL Addressable Groups", "Ok", "Cancel"))
            {
                ProcessAllAddressableGroups(RegenerateAddressableGroups);
            }

            if (GUILayout.Button("Remove All Localized Assets") && EditorUtility.DisplayDialog("Warning", "This Will Regenerate ALL Addressable Groups", "Ok", "Cancel"))
            {
                ProcessAllAddressableGroups(RemoveNonLocalizedAssets);
            }
        }

        private void RemoveNonLocalizedAssets(LocalAudioData localAudioData)
        {
            var audioAssetReferences = localAudioData.AudioAssetReferenceList;
            var audioAssetReferencesToRemove = new List<AudioAssetReference>();

            foreach (AudioAssetReference reference in audioAssetReferences)
            {
                if (reference.HasAssetReference && reference.AddressableAudioReference != null && reference.AddressableAudioReference.editorAsset != null)
                {
                    if (!reference.AddressableAudioReference.editorAsset.name.EndsWith(localAudioData.Identifier.LocalizationAudioSuffix))
                    {
                        audioAssetReferencesToRemove.Add(reference);
                    }
                }
            }

            foreach (var audioAssetReference in audioAssetReferencesToRemove)
            {
                audioAssetReferences.Remove(audioAssetReference);
            }
            EditorUtility.SetDirty(localAudioData);
        }

        private void ProcessAllAddressableGroups(ProcessLocalAudioDataDelegate dataDelegate)
        {
            var assetPath = AssetDatabase.FindAssets($"t:{typeof(LocalAudioData).Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            List<LocalAudioData> localAudioData = assetPath.Select(x => AssetDatabase.LoadAssetAtPath<LocalAudioData>(x)).ToList();
            foreach (var item in localAudioData)
            {
                if (item != null)
                {
                    dataDelegate(item);
                }
            }
        }

        private void RegenerateAddressableGroups(LocalAudioData localAudioData)
        {
            List<AudioClip> arrayClips = new List<AudioClip>(localAudioData.AudioAssetReferenceList.Count);
            List<string> labels = new List<string>();
            foreach (var item in localAudioData.AudioAssetReferenceList)
            {

                if (item.HasAssetReference && item.AddressableAudioReference != null && item.AddressableAudioReference.editorAsset != null)
                {
                    arrayClips.Add((AudioClip)item.AddressableAudioReference.editorAsset);
                    labels.Add(localAudioData.name);
                }
            }

            AddressableExtensions.SetAddressableGroupButch<AudioClip>(arrayClips, localAudioData.AddressableGroupName, arrayClips.Count, labels);
            EditorUtility.SetDirty(localAudioData);
        }

        private void SplitIntoModuleAssets(LocalAudioData localAudioData)
        {
            string[] directories = Directory.GetDirectories(_localAudioDataEditor.ModulesPath);
            string[] modules = directories.Select(x => new DirectoryInfo(x).Name).ToArray();
            string directoryRoot = Path.GetDirectoryName(AssetDatabase.GetAssetPath(localAudioData));

            for (int i = 0; i < directories.Length; i++)
            {
                string directory = directories[i];
                string[] audioFilesPath = AssetDatabase.FindAssets($"t:{typeof(AudioClip).Name}", new string[] { directory }).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

                var moduleAudio = localAudioData.AudioAssetReferenceList.Where(x => x.HasAssetReference && audioFilesPath.Contains(AssetDatabase.GetAssetPath(x.AddressableAudioReference.editorAsset)));

                string assetRootName = modules[i] + "_" + localAudioData.Identifier.LanguageName;
                LocalAudioData newLocalAudio = ConfigCreator.LoadOrCreateConfig<LocalAudioData>(assetRootName + "_AudioLocalData", Path.Combine(directoryRoot, modules[i] + "_AudioLocalData"));

                newLocalAudio.AudioAssetReferenceList.Clear();
                newLocalAudio.AudioAssetReferenceList.AddRange(moduleAudio);
                localAudioData.AudioAssetReferenceList.RemoveAll(x => moduleAudio.Contains(x));
                newLocalAudio.CopyDataFrom(localAudioData);
                EditorUtility.SetDirty(newLocalAudio);
                EditorUtility.SetDirty(localAudioData);
            }
            AssetDatabase.Refresh();
        }
    }
#endif
}
