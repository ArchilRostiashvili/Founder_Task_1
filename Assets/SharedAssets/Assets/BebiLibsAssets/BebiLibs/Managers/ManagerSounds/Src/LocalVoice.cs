using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BebiLibs
{
    public class LocalVoice : ScriptableObject
    {
        public string langID;
        public DataSound[] arraySounds;
        private AudioClip[] _arrayClips;

        public void UnloadLocal()
        {
            if (_arrayClips != null)
            {
                for (int i = 0; i < _arrayClips.Length; i++)
                {
                    if (_arrayClips[i] != null)
                    {
                        _arrayClips[i].UnloadAudioData();
                    }
                }
            }

            _arrayClips = null;
        }

        public void LoadIn()
        {
            _arrayClips = new AudioClip[this.arraySounds.Length];
            for (int i = 0; i < this.arraySounds.Length; i++)
            {
                _arrayClips[i] = this.arraySounds[i].sound;
            }
        }

        public void InitContent()
        {
#if UNITY_EDITOR

            AudioClip[] array = Resources.LoadAll<AudioClip>("voice_" + this.langID);
            this.arraySounds = new DataSound[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                this.arraySounds[i] = DataSound.Create(array[i], 1.0f, true, false);
            }
            EditorUtility.SetDirty(this);
#endif
        }

        public DataSound GetSound(string soundID)
        {
            for (int i = 0; i < this.arraySounds.Length; i++)
            {
                if (this.arraySounds[i].soundName == soundID)
                {
                    return this.arraySounds[i];
                }
            }
            return null;
        }
    }
}


