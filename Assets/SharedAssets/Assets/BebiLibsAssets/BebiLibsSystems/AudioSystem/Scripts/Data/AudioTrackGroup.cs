using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEditorUtilities;
using BebiLibs.Localization.Core;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.AudioSystem
{
    [CreateAssetMenu(menuName = "BebiLibs/Audio System/New Module")]
    public class AudioTrackGroup : ScriptableObject
    {
        [SerializeField] private string _groupName;
        [SerializeField] private FolderPath _folderPath;
        [SerializeField] private AudioTrackSettingsSO _audioTrackSetting;

        public FolderPath FolderPath => _folderPath;
        public AudioTrackSettingsSO AudioTrackSetting => _audioTrackSetting;

        public virtual string GetGroupName()
        {
            return _groupName;
        }

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(GetGroupName());
        }

        public virtual string GetAudioFileFolder(LanguageIdentifier identifier)
        {
            return Path.Combine(FolderPath, GetGroupName() + "_" + identifier.LanguageName);
        }

        public virtual string GetAudioTrackFolder(LanguageIdentifier identifier)
        {
            return Path.Combine(GetAudioFileFolder(identifier), GetGroupName() + "_" + identifier.LanguageName + "_AudioTracks");
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(AudioTrackGroup))]
    public class AudioTrackGroupEditor : Editor
    {
        private AudioTrackGroup _audioTrackGroup;

        private void OnEnable()
        {
            _audioTrackGroup = (AudioTrackGroup)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Folder Structure"))
            {
                CreateFolderStructure();
            }

            if (GUILayout.Button("Update Audio Track Sounds"))
            {
                UpdateOrCreateAudioTracks();
            }
        }

        public List<LanguageIdentifier> CreateFolderStructure()
        {
            List<LanguageIdentifier> locAudioList = UnityFileUtils.FindScriptableOBjectByType<LanguageIdentifier>();
            foreach (var locAudio in locAudioList)
            {
                var languageFolderPath = _audioTrackGroup.GetAudioFileFolder(locAudio);
                if (!Directory.Exists(languageFolderPath))
                {
                    Directory.CreateDirectory(languageFolderPath);
                }

                var trackPath = _audioTrackGroup.GetAudioTrackFolder(locAudio);
                if (!Directory.Exists(trackPath))
                {
                    Directory.CreateDirectory(trackPath);
                }
            }
            AssetDatabase.Refresh();
            return locAudioList;
        }

        public void UpdateOrCreateAudioTracks()
        {
            List<LanguageIdentifier> locAudioList = CreateFolderStructure();

            foreach (var locAudio in locAudioList)
            {
                string languageFolderPath = _audioTrackGroup.GetAudioFileFolder(locAudio);
                string trackPath = _audioTrackGroup.GetAudioTrackFolder(locAudio);
                CreateAudioTracks(languageFolderPath, trackPath);
            }
            AssetDatabase.Refresh();
        }

        public void CreateAudioTracks(string languageFolderPath, string trackPath)
        {
            List<AudioClip> audioClips = GetAssetAtPath<AudioClip>(languageFolderPath);
            List<AudioTrackSO> audioTracks = GetAssetAtPath<AudioTrackSO>(trackPath);

            List<AudioClip> unassignedAudioClips = new List<AudioClip>();

            foreach (AudioClip clip in audioClips)
            {
                AudioTrackSO track = audioTracks.Find(x => x.AudioClip == clip);
                if (track == null)
                {
                    unassignedAudioClips.Add(clip);
                }
            }

            foreach (AudioClip clip in unassignedAudioClips)
            {
                AudioTrackSO audioTrack = ConfigCreator.LoadOrCreateConfig<AudioTrackSO>(clip.name + "_track", trackPath);
                audioTrack.SetAudioClip(clip);
                audioTrack.SetSettings(_audioTrackGroup.AudioTrackSetting);
                EditorUtility.SetDirty(audioTrack);
            }
        }

        public List<T> GetAssetAtPath<T>(string path) where T : UnityEngine.Object
        {
            string[] audioClipsGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            List<T> assets = new List<T>();
            foreach (var item in audioClipsGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(item);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                assets.Add(asset);
            }
            return assets;
        }

    }
#endif
}
