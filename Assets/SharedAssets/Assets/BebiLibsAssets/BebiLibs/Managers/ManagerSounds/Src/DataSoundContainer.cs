using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DataSoundContainer", menuName = "BebiLibs/OldSoundSystem/DataSoundContainer", order = 0)]
    public class DataSoundContainer : ScriptableObject, IList<DataSound>
    {
        [SerializeField] private List<DataSound> _dataSoundsList = new List<DataSound>();

        public DataSound this[int index]
        {
            get => _dataSoundsList[index];
            set => _dataSoundsList[index] = value;
        }
        public int Count => _dataSoundsList.Count;
        public bool IsReadOnly => false;
        public void Add(DataSound item) => _dataSoundsList.Add(item);
        public void Clear() => _dataSoundsList.Clear();
        public bool Contains(DataSound moduleGameKind) => _dataSoundsList.Contains(moduleGameKind);
        public DataSound Find(string name) => _dataSoundsList.Find(x => x.soundName == name);
        public void CopyTo(DataSound[] array, int arrayIndex) => _dataSoundsList.CopyTo(array, arrayIndex);
        public IEnumerator<DataSound> GetEnumerator() => _dataSoundsList.GetEnumerator();
        public int IndexOf(DataSound item) => _dataSoundsList.IndexOf(item);
        public void Insert(int index, DataSound item) => _dataSoundsList.Insert(index, item);
        public bool Remove(DataSound item) => _dataSoundsList.Remove(item);
        public void RemoveAt(int index) => _dataSoundsList.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _dataSoundsList.GetEnumerator();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(DataSoundContainer))]
    public class DataSoundContainerEditor : Editor
    {
        private DataSoundContainer _dataSoundContainer;

        private void OnEnable()
        {
            _dataSoundContainer = (DataSoundContainer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create From Selection"))
            {
                foreach (string assetGUIDS in Selection.assetGUIDs)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDS);
                    if (CheckIfAudioClip(assetPath, out AudioClip clip))
                    {
                        DataSound dataSound = DataSound.Create(clip, 1, false, false);
                        _dataSoundContainer.Add(dataSound);
                    }
                }
            }
        }

        public static bool CheckIfAudioClip(string path, out AudioClip clip)
        {
            clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            return clip != null;
        }

    }
#endif
}
