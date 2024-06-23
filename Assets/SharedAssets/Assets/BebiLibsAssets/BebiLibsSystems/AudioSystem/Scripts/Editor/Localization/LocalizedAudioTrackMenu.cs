using BebiLibs.EditorExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.AudioSystem.AudioSystemEditor
{
    public class LocalizedAudioTrackMenu : MonoBehaviour
    {
        [MenuItem("Assets/Audio System/Create Localized Audio Assets Collection", false)]
        static void CreateLocalizedAudioAsset()
        {
            ContextMenuHelper.CreateObjectFromAssets<PlayableAudioTrackSO>(CreateLocalizedAudioAssets, null);
        }

        [MenuItem("Assets/Audio System/Create Localized Audio Assets Collection", true)]
        static bool CreateLocalizedAudioAssetValidate()
        {
            return ContextMenuHelper.IfSelectionIsValidType<PlayableAudioTrackSO>();
        }

        private static void CreateLocalizedAudioAssets(string folderPath, List<PlayableAudioTrackSO> unityAssets)
        {
            AssetSearchWindow.ShowWindow($"Select {nameof(AudioTrackGroup)} for {nameof(LocalizedAudioTrackSO)}", "AudioTrackGroup", typeof(AudioTrackGroup), (obj) =>
            {
                foreach (var item in unityAssets)
                {
                    CreateLocalizedAudioAsset(folderPath, item, (AudioTrackGroup)obj);
                }
            }, null);

            AssetDatabase.SaveAssets();
        }

        private static void CreateLocalizedAudioAsset(string folderPath, PlayableAudioTrackSO audioTrack, AudioTrackGroup audioTrackGroup)
        {
            string path = AssetDatabase.GetAssetPath(audioTrack);
            var localizedAudioAsset = ScriptableObject.CreateInstance<LocalizedAudioTrackSO>();
            localizedAudioAsset.SetDefaultAudioTrack(audioTrack);
            localizedAudioAsset.SetAudioTrackGroup(audioTrackGroup);
            EditorUtility.SetDirty(localizedAudioAsset);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string newPath = Path.Combine(folderPath, fileNameWithoutExtension + "_local.asset");
            AssetDatabase.CreateAsset(localizedAudioAsset, newPath);
        }
    }
}
