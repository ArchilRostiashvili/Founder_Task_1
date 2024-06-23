using BebiLibs.Localization.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.AudioSystem
{
    public class LocalizedAudioTrackInspectorConfig : ScriptableObject
    {
        public List<AudioTrackGroup> AudioGroupFilter = new List<AudioTrackGroup>();
        public bool CheckForLanguageMatch = true;
        public List<LanguageIdentifier> LanguageFilter = new List<LanguageIdentifier>();
    }

}
