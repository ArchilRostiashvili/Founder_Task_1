using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ModulesGameSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ModuleCollection", menuName = "BebiLibs/GameModules/ModuleCollectionData", order = 0)]
    public class ModuleCollectionData : ScriptableObject, IList<ModuleData>
    {
        [ObjectInspector(false)]
        [SerializeField] private List<ModuleData> _moduleCollectionList = new List<ModuleData>();

        public ModuleData this[int index]
        {
            get => _moduleCollectionList[index];
            set => _moduleCollectionList[index] = value;
        }

        public int Count => _moduleCollectionList.Count;
        public bool IsReadOnly => false;
        public void Add(ModuleData item) => _moduleCollectionList.Add(item);
        public void Clear() => _moduleCollectionList.Clear();
        public bool Contains(ModuleData moduleGameKind) => _moduleCollectionList.Contains(moduleGameKind);
        public void CopyTo(ModuleData[] array, int arrayIndex) => _moduleCollectionList.CopyTo(array, arrayIndex);
        public IEnumerator<ModuleData> GetEnumerator() => _moduleCollectionList.GetEnumerator();
        public int IndexOf(ModuleData item) => _moduleCollectionList.IndexOf(item);
        public void Insert(int index, ModuleData item) => _moduleCollectionList.Insert(index, item);
        public bool Remove(ModuleData item) => _moduleCollectionList.Remove(item);
        public void RemoveAt(int index) => _moduleCollectionList.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _moduleCollectionList.GetEnumerator();
    }
}
