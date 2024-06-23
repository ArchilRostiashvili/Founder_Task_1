using System;
using System.Collections.Generic;
using UnityEditor;

namespace BebiLibs.AudioSystem
{
    public static class LocalizedTrackFixer
    {
        public static void FixTracks(List<LocalizedAudioTrackSO> locAudioList)
        {
            foreach (var locAudio in locAudioList)
            {
                SetDefaultFromEnglishList(locAudio);
                ValidateAddressableGroups(locAudio);
            }
        }

        public static void SetDefaultFromEnglishList(LocalizedAudioTrackSO locAudio)
        {
            var track = locAudio.GetLocalizedAudioTrackData("English");
            if (track != null)
            {
                locAudio.SetDefaultAudioTrack(track.AudioTrackReference.editorAsset);
                locAudio.RemoveLocalizedAudioTrackData(track);
                EditorUtility.SetDirty(locAudio);
            }
        }

        public static void ValidateAddressableGroups(LocalizedAudioTrackSO trackSO)
        {
            foreach (LocalizedAudioTrackData item in trackSO.LocalizedTrackDataList)
            {
                if (item.AudioTrackReference != null && item.AudioTrackReference.editorAsset != null)
                {
                    string audioTrackGroup = trackSO.AudioTrackGroup.GetGroupName();
                    AddressableExtensions.SetAddressableGroup<PlayableAudioTrackSO>(item.AudioTrackReference.editorAsset, audioTrackGroup, item.LanguageIdentifier.LanguageName, true);
                }
            }
        }
    }
}
