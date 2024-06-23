using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BebiLibs
{
    [System.Serializable]
    public class DataSoundsLib
    {
        public string[] arrayClipsFromRes;

        [Header("You can user scene specific audio files in this array, but it's better to use AudioClipPacks instead")]
        public List<AudioClip> arraySimpleClips;

        /// <summary>
        /// Use this to add sound to scenes; if necessary use ManagerSoundHalper component class to copy existing arraySimpleClips to AudioClipContainer  
        /// </summary>
        [Header("Create scene specific audio pack and share it between scenes")]
        public List<AudioClipPack> arrayAudioClipsPack = new List<AudioClipPack>();
        public List<DataSound> arrayDataSounds;

        public static DataSoundsLib Create()
        {
            DataSoundsLib dataSoundsLib = new DataSoundsLib();
            dataSoundsLib.arraySimpleClips = new List<AudioClip>();
            dataSoundsLib.arrayDataSounds = new List<DataSound>();

            return dataSoundsLib;
        }

        public static DataSoundsLib Create(int count)
        {
            DataSoundsLib dataSoundsLib = new DataSoundsLib();
            dataSoundsLib.arraySimpleClips = new List<AudioClip>(count);
            dataSoundsLib.arrayClipsFromRes = new string[count];

            return dataSoundsLib;
        }


        public List<AudioClip> GetAllAudioName()
        {
            List<AudioClip> arrayAudioClips = new List<AudioClip>();

            arrayAudioClips.AddRange(SampleClipNames(arraySimpleClips, "fx_tx"));

            foreach (AudioClipPack audioPack in arrayAudioClipsPack)
            {
                arrayAudioClips.AddRange(SampleClipNames(audioPack.arrayAudioClips, "fx_tx"));
            }

            return arrayAudioClips;
        }

        private IEnumerable<AudioClip> SampleClipNames(IList<AudioClip> audioClips, string startWith)
        {
            List<AudioClip> arrayAudioClips = new List<AudioClip>(audioClips.Count);
            for (int i = 0; i < audioClips.Count; i++)
            {
                if (audioClips[i] != null && audioClips[i].name.StartsWith(startWith))
                {
                    arrayAudioClips.Add(audioClips[i]);
                }
            }
            return arrayAudioClips;
        }

    }
}
