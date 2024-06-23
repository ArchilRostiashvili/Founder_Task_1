using BebiLibs.EditorExtensions;
using BebiLibs.EditorExtensions.AssetRenaming;
using CustomEditorUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BebiLibs.AudioSystem.AudioSystemEditor
{
    public class AudioTrackCreatorMenu
    {
        private static List<string> _PrefixList = new List<string>() { "fx_tx_", "fx_" };
        private static List<string> _SuffixList = new List<string>() { "_en", "_es", "_fr", "_de", "_it", "_pt", "_ru", "_ka" };

        private static List<ReplaceStringData> _RemoveStringsList = new List<ReplaceStringData>()
        {
            new ReplaceStringData("-", "_", true),
            new ReplaceStringData(" ", "_", true),
            new ReplaceStringData("'", "_", true),
            new ReplaceStringData("`", "_", true),
        };


        [MenuItem("Assets/Audio System/Create Audio Track", false)]
        static void CreateAudioTrack()
        {
            AssetRenamingConfig assetRenamingConfig = AssetRenamingConfig.Create(_PrefixList, _SuffixList, _RemoveStringsList, true);
            ContextMenuHelper.CreateObjectFromAssets<AudioClip>(CreateAudioTrackSO, assetRenamingConfig);
        }

        [MenuItem("Assets/Audio System/Create Audio Track", true)]
        static bool CreateAudioTrackValidate()
        {
            return ContextMenuHelper.IfSelectionIsValidType<AudioClip>();
        }

        private static string ReplaceMultipleSpaces(string stringToReplace, string replacementString)
        {
            return string.Join(replacementString, stringToReplace.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void CreateAudioTrackSO(string folderPath, List<AudioClip> audioClips)
        {
            foreach (var item in audioClips)
            {
                CreateAudioTrack(item, folderPath);
            }
            AssetDatabase.SaveAssets();
        }

        private static void CreateAudioTrack(AudioClip audioClip, string folder)
        {
            string path = AssetDatabase.GetAssetPath(audioClip);
            var audioTrack = ScriptableObject.CreateInstance<AudioTrackSO>();
            audioTrack.SetAudioClip(audioClip);
            EditorUtility.SetDirty(audioTrack);
            string fileExtension = Path.GetFileNameWithoutExtension(path);
            string newPath = Path.Combine(folder, fileExtension + ".asset");

            if (File.Exists(newPath))
            {
                int val = EditorUtility.DisplayDialogComplex("File already exists", $"{Path.GetFileName(newPath)} File already exists at {Path.GetDirectoryName(newPath)}", "Overwrite", "Cancel", "Keep both");

                switch (val)
                {
                    case 0:
                        AssetDatabase.CreateAsset(audioTrack, newPath);
                        AssetDatabase.SaveAssets();
                        break;
                    case 1:
                        return;
                    case 2:
                        newPath = UnityFileUtils.DuplicatePathFixer(newPath);
                        AssetDatabase.CreateAsset(audioTrack, newPath);
                        AssetDatabase.SaveAssets();
                        break;
                }
                return;
            }
            AssetDatabase.CreateAsset(audioTrack, newPath);
        }


    }
}
