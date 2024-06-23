using BebiLibs.AudioSystem;
using BebiLibs.EditorExtensions;
using CustomEditorUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.AudioSystem.AudioSystemEditor
{
    public class AudioTrackMenu
    {

        [MenuItem("Assets/Audio System/Create Audio Sequence", false)]
        static void CreateAudioSequence()
        {
            ContextMenuHelper.CreateObjectFromAssets<AudioTrackSO>(CreateAudioSequences, null);
        }

        [MenuItem("Assets/Audio System/Create Audio Sequence", true)]
        static bool CreateAudioSequenceValidate()
        {
            return ContextMenuHelper.IfSelectionIsValidType<AudioTrackSO>();
        }

        private static void CreateAudioSequences(string folderPath, List<AudioTrackSO> unityAssets)
        {
            var audioSequence = ScriptableObject.CreateInstance<AudioTrackSequenceSO>();
            audioSequence.ClearAudioTrackList();

            foreach (var item in unityAssets)
            {
                audioSequence.AddAudioTrack(item);
            }

            string newPath = Path.Combine(folderPath, "AudioTrackSequenceSO.asset");
            AssetDatabase.CreateAsset(audioSequence, newPath);
            EditorUtility.SetDirty(audioSequence);
            AssetDatabase.SaveAssets();
        }



        [MenuItem("Assets/Audio System/Create Collection", false)]
        static void CreateAudioCollection()
        {
            ContextMenuHelper.CreateObjectFromAssets<AudioTrackSO>(CreateAudioCollections, null);
        }

        [MenuItem("Assets/Audio System/Create Collection", true)]
        static bool CreateAudioCollectionValidate()
        {
            return ContextMenuHelper.IfSelectionIsValidType<AudioTrackSO>();
        }


        private static void CreateAudioCollections(string folderPath, List<AudioTrackSO> unityAssets)
        {
            var audioCollection = ScriptableObject.CreateInstance<AudioTrackCollectionSO>();
            audioCollection.ClearAudioTrackList();
            foreach (var item in unityAssets)
            {
                audioCollection.AddAudioTrack(item);
            }

            string newPath = Path.Combine(folderPath, "AudioTrackCollectionSO.asset");
            AssetDatabase.CreateAsset(audioCollection, newPath);
            EditorUtility.SetDirty(audioCollection);
            AssetDatabase.SaveAssets();
        }



    }
}
