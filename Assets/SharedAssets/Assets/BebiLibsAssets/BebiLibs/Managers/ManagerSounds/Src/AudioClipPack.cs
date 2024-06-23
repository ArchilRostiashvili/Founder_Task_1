using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AudioClipPack", menuName = "BebiLibs/ManagerSound/AudioClipPack", order = 0)]
    public class AudioClipPack : ScriptableObject, IList<AudioClip>, ICollection<AudioClip>
    {
        public List<AudioClip> arrayAudioClips = new List<AudioClip>();

        public AudioClip this[int index] { get => this.arrayAudioClips[index]; set => this.arrayAudioClips[index] = value; }

        public int Count => this.arrayAudioClips.Count;

        public bool IsReadOnly => false;

        public void Add(AudioClip item)
        {
            this.arrayAudioClips.Add(item);
        }

        public void Clear()
        {
            this.arrayAudioClips.Clear();
        }

        public bool Contains(AudioClip item)
        {
            return this.arrayAudioClips.Contains(item);
        }

        public void CopyTo(AudioClip[] array, int arrayIndex)
        {
            this.arrayAudioClips.CopyTo(array, arrayIndex);
        }

        public IEnumerator<AudioClip> GetEnumerator()
        {
            return this.arrayAudioClips.GetEnumerator();
        }

        public int IndexOf(AudioClip item)
        {
            return this.arrayAudioClips.IndexOf(item);
        }

        public void Insert(int index, AudioClip item)
        {
            this.arrayAudioClips.Insert(index, item);
        }

        public bool Remove(AudioClip item)
        {
            return this.arrayAudioClips.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.arrayAudioClips.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.arrayAudioClips.GetEnumerator();
        }
    }

}
