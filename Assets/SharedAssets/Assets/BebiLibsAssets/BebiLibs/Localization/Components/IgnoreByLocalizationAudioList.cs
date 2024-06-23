using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Localization
{
    public class IgnoreByLocalizationAudioList : MonoBehaviour
    {
        public List<AudioClip> _arrayAudioClipsToIgnore = new List<AudioClip>();

        private void Awake()
        {
            AddressableAudioManager.SetIgnoreAudioList(_arrayAudioClipsToIgnore);
        }

        private void OnDestroy()
        {
            AddressableAudioManager.RemoveFromIgnoreAudioList(_arrayAudioClipsToIgnore);
        }
    }
}
